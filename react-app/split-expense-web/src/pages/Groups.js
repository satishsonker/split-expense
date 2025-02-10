import { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    Grid,
    Card,
    CardContent,
    IconButton,
    Fab,
    useTheme,
    useMediaQuery,
    Avatar,
    AvatarGroup,
    Tooltip,
    Button,
    CircularProgress
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { getGroupIcon } from '../utils/groupIcons';
import CreateGroupDialog from '../components/CreateGroupDialog';
import { apiService } from '../utils/axios';
import { GROUP_PATHS } from '../constants/apiPaths';
import { toast } from 'react-toastify';

const GROUPS_PER_PAGE = 10;

const Groups = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [openCreateDialog, setOpenCreateDialog] = useState(false);
    const [groups, setGroups] = useState([]);
    const [loading, setLoading] = useState(false);
    const [page, setPage] = useState(1);
    const [hasMore, setHasMore] = useState(true);
    const [totalGroups, setTotalGroups] = useState(0);

    const fetchGroups = async (pageNum = 1) => {
        try {
            setLoading(true);
            const response = await apiService.get(GROUP_PATHS.LIST, {
                pageNo: pageNum,
                pageSize: GROUPS_PER_PAGE
            });
            
            if (response?.data) {
                if (pageNum === 1) {
                    setGroups(response.data || []);
                } else {
                    setGroups(prev => [...prev, ...(response.data || [])]);
                }
                
                setTotalGroups(response.recordCounts || 0);
                setHasMore((response.recordCounts - (response.pageNo * response.pageSize)) > 0);
            }
        } catch (error) {
            console.error('Error fetching groups:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchGroups();
    }, []);

    const handleCreateGroup = async (groupData) => {
        try {
            setLoading(true);
            await apiService.post(GROUP_PATHS.CREATE, {
                name: groupData.name,
                members: groupData.members.map(m => m.id)
            });
            
            toast.success('Group created successfully');
            setOpenCreateDialog(false);
            fetchGroups(1);
            setPage(1);
        } catch (error) {
            console.error('Error creating group:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleLoadMore = () => {
        const nextPage = page + 1;
        setPage(nextPage);
        fetchGroups(nextPage);
    };

    function stringToColor(string) {
        let hash = 0;
        let i;
      
        /* eslint-disable no-bitwise */
        for (i = 0; i < string.length; i += 1) {
          hash = string.charCodeAt(i) + ((hash << 5) - hash);
        }
      
        let color = '#';
      
        for (i = 0; i < 3; i += 1) {
          const value = (hash >> (i * 8)) & 0xff;
          color += `00${value.toString(16)}`.slice(-2);
        }
        /* eslint-enable no-bitwise */
      
        return color;
      }
    function stringAvatar(name) {
        return {
          sx: {
            bgcolor: stringToColor(name),
          },
          children: `${name.split(' ')[0][0]}${name.split(' ')[1][0]}`,
        };
      }
      
    return (
        <Box sx={{ p: { xs: 2, sm: 3 } }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h5" component="h1">
                    Groups ({totalGroups})
                </Typography>
                {!isMobile && (
                    <IconButton
                        color="primary"
                        onClick={() => setOpenCreateDialog(true)}
                        sx={{ bgcolor: 'primary.light', '&:hover': { bgcolor: 'primary.main' } }}
                    >
                        <AddIcon />
                    </IconButton>
                )}
            </Box>

            <Grid container spacing={2}>
                {groups.map(group => {
                    const GroupIcon = getGroupIcon(group.name);
                    return (
                        <Grid item xs={12} sm={6} md={4} key={group.id}>
                            <Card 
                                sx={{ 
                                    cursor: 'pointer',
                                    '&:hover': { boxShadow: 6 }
                                }}
                            >
                                <CardContent>
                                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                        <Avatar 
                                            sx={{ 
                                                bgcolor: 'primary.main',
                                                mr: 2
                                            }}
                                        >
                                            <GroupIcon />
                                        </Avatar>
                                        <Typography variant="h6" noWrap>
                                            {group.name}
                                        </Typography>
                                    </Box>
                                    
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <AvatarGroup max={3}>
                                            {group?.members?.map(member => (
                                                <Tooltip key={member.id} title={`${member.addedUser.firstName} ${member.addedUser.lastName}`}>
                                                    <Avatar {...stringAvatar(`${member.addedUser.firstName} ${member.addedUser.lastName}`)} src={member.addedUser.profilePicture}>{`${member.addedUser.firstName[0]}`}</Avatar>
                                                </Tooltip>
                                            ))}
                                        </AvatarGroup>
                                        <Typography variant="subtitle1" color="primary">
                                            ${group.totalExpenses}
                                        </Typography>
                                    </Box>
                                </CardContent>
                            </Card>
                        </Grid>
                    );
                })}
            </Grid>

            {hasMore && (
                <Box sx={{ mt: 3, textAlign: 'center' }}>
                    <Button
                        variant="outlined"
                        onClick={handleLoadMore}
                        disabled={loading}
                        startIcon={loading && <CircularProgress size={20} />}
                    >
                        {loading ? 'Loading...' : 'Load More'}
                    </Button>
                </Box>
            )}

            {isMobile && (
                <Fab
                    color="primary"
                    sx={{ position: 'fixed', bottom: 16, right: 16 }}
                    onClick={() => setOpenCreateDialog(true)}
                >
                    <AddIcon />
                </Fab>
            )}

            <CreateGroupDialog
                open={openCreateDialog}
                onClose={() => setOpenCreateDialog(false)}
                onSubmit={handleCreateGroup}
            />
        </Box>
    );
};

export default Groups; 