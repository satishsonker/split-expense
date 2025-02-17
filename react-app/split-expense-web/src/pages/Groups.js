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
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { getGroupIcon } from '../utils/groupIcons';
import CreateGroupDialog from '../components/CreateGroupDialog';
import { apiService } from '../utils/axios';
import { GROUP_PATHS } from '../constants/apiPaths';
import { toast } from 'react-toastify';
import { useAuth } from '../context/AuthContext';
import DeleteConfirmationDialog from '../components/DeleteConfirmationDialog';
import dayjs from 'dayjs';
import ImageViewDialog from '../components/ImageViewDialog';
import { getImageUrl } from '../utils/imageUtils';

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
    const [selectedGroup, setSelectedGroup] = useState(null);
    const { user } = useAuth();
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [groupToDelete, setGroupToDelete] = useState(null);
    const [deleteLoading, setDeleteLoading] = useState(false);
    const [selectedImage, setSelectedImage] = useState(null);

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
            console.log('Group Data:', groupData);
            setLoading(true);
            const formData = new FormData();
            
            // Required field
            formData.append('name', groupData.name);
            
            // Optional fields
            if (groupData.icon) {
                formData.append('icon', groupData.icon);
            }
            
            if (groupData.image) {
                formData.append('image', groupData.image);
            }
            
            // Optional members array
            if (groupData.members?.length > 0) {
                groupData.members.forEach(member => {
                    formData.append('members', member.contactId);
                });
            }
            
            // Optional group type
            if (groupData.groupTypeId) {
                formData.append('groupTypeId', groupData.groupTypeId);
            }
            
            // Optional group details
            if (groupData.groupDetail) {
                const groupDetail = {
                    enableGroupDate: groupData.groupDetail.enableGroupDate || false,
                    enableSettleUpReminders: groupData.groupDetail.enableSettleUpReminders || false,
                    enableBalanceAlert: groupData.groupDetail.enableBalanceAlert || false,
                    maxGroupBudget: groupData.groupDetail.maxGroupBudget || null,
                    startDate: groupData.groupDetail.startDate ? dayjs(groupData.groupDetail.startDate).toISOString() : null,
                    endDate: groupData.groupDetail.endDate ? dayjs(groupData.groupDetail.endDate).toISOString() : null
                };
                formData.append('groupDetail', JSON.stringify(groupDetail));
            }

            console.log('FormData entries:');
            for (let pair of formData.entries()) {
                console.log(pair[0] + ': ' + pair[1]);
            }
            
            await apiService.post(GROUP_PATHS.CREATE, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            
            toast.success('Group created successfully');
            setOpenCreateDialog(false);
            fetchGroups(1);
            setPage(1);
        } catch (error) {
            console.error('Error creating group:', error);
            toast.error(error.response?.data?.message || 'Failed to create group');
        } finally {
            setLoading(false);
        }
    };

    const handleLoadMore = () => {
        const nextPage = page + 1;
        setPage(nextPage);
        fetchGroups(nextPage);
    };

    const handleDeleteGroup = (group) => {
        setGroupToDelete(group);
        setDeleteDialogOpen(true);
    };

    const handleConfirmDelete = async () => {
        try {
            setDeleteLoading(true);
            await apiService.delete(`${GROUP_PATHS.DELETE}/${groupToDelete.id}`);
            toast.success('Group deleted successfully');
            fetchGroups(); // Refresh the list
        } catch (error) {
            console.error('Error deleting group:', error);
            toast.error(error.response?.data?.message || 'Failed to delete group');
        } finally {
            setDeleteLoading(false);
            setDeleteDialogOpen(false);
            setGroupToDelete(null);
        }
    };

    const handleCloseDeleteDialog = () => {
        setDeleteDialogOpen(false);
        setGroupToDelete(null);
    };

    const handleEditGroup = async (group) => {
        setSelectedGroup(group);
        setOpenCreateDialog(true);
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
      
    const isGroupCreator = (group) => {
        return group.createdBy === user.id;
    };

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
                    const canManageGroup = isGroupCreator(group);
                    const thumbUrl = getImageUrl(group.thumbImagePath);
                    const fullImageUrl = getImageUrl(group.imagePath);

                    return (
                        <Grid item xs={12} sm={6} md={4} key={group.id}>
                            <Card 
                                sx={{ 
                                    cursor: 'pointer',
                                    '&:hover': { boxShadow: 6 }
                                }}
                            >
                                <CardContent>
                                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 2, justifyContent: 'space-between' }}>
                                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                            {thumbUrl ? (
                                                <Avatar 
                                                    src={thumbUrl}
                                                    sx={{ 
                                                        width: 40, 
                                                        height: 40,
                                                        mr: 2,
                                                        cursor: group.imagePath ? 'pointer' : 'default',
                                                        '&:hover': {
                                                            opacity: group.imagePath ? 0.8 : 1
                                                        }
                                                    }}
                                                    onClick={() => fullImageUrl && setSelectedImage(fullImageUrl)}
                                                />
                                            ) : (
                                                <Avatar 
                                                    sx={{ 
                                                        bgcolor: 'primary.main',
                                                        mr: 2
                                                    }}
                                                >
                                                    <GroupIcon />
                                                </Avatar>
                                            )}
                                            <Typography variant="h6" noWrap>
                                                {group.name}
                                            </Typography>
                                        </Box>
                                        {canManageGroup && (
                                            <Box>
                                                <IconButton 
                                                    size="small" 
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        handleEditGroup(group);
                                                    }}
                                                    title="Edit Group"
                                                >
                                                    <EditIcon fontSize="small" />
                                                </IconButton>
                                                <IconButton 
                                                    size="small" 
                                                    color="error"
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        handleDeleteGroup(group);
                                                    }}
                                                    title="Delete Group"
                                                >
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            </Box>
                                        )}
                                    </Box>
                                    
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                            <AvatarGroup max={3}>
                                                {group?.members?.map(member => (
                                                    <Tooltip 
                                                        key={member.id} 
                                                        title={`${member.addedUser.firstName} ${member.addedUser.lastName}${member.addedUser.id === group.createdBy ? ' (Creator)' : ''}`}
                                                    >
                                                        <Avatar 
                                                            {...stringAvatar(`${member.addedUser.firstName} ${member.addedUser.lastName}`)} 
                                                            src={member.addedUser.profilePicture}
                                                            sx={member.addedUser.userId === group.createdBy ? {
                                                                border: '2px solid gold'
                                                            } : undefined}
                                                        >
                                                            {`${member.addedUser.firstName[0]}`}
                                                        </Avatar>
                                                    </Tooltip>
                                                ))}
                                            </AvatarGroup>
                                        </Box>
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
                onClose={() => {
                    setOpenCreateDialog(false);
                    setSelectedGroup(null);
                }}
                onSubmit={handleCreateGroup}
                group={selectedGroup}
            />

            <DeleteConfirmationDialog
                open={deleteDialogOpen}
                onClose={handleCloseDeleteDialog}
                onConfirm={handleConfirmDelete}
                loading={deleteLoading}
                type="group"
                data={groupToDelete}
                title="Delete Group"
                warningMessage="This action cannot be undone. All group expenses and settlements will be deleted."
            />

            <ImageViewDialog 
                open={Boolean(selectedImage)}
                onClose={() => setSelectedImage(null)}
                imagePath={selectedImage}
            />
        </Box>
    );
};

export default Groups; 