import { useState } from 'react';
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
    Tooltip
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { getGroupIcon } from '../utils/groupIcons';
import CreateGroupDialog from '../components/CreateGroupDialog';

const Groups = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [openCreateDialog, setOpenCreateDialog] = useState(false);
    
    // This would typically come from your API/state management
    const groups = [
        {
            id: 1,
            name: 'Weekend Trip',
            members: [
                { id: 1, name: 'John Doe', email: 'john@example.com' },
                { id: 2, name: 'Jane Smith', email: 'jane@example.com' }
            ],
            totalExpenses: 450
        },
        {
            id: 2,
            name: 'Apartment Rent',
            members: [
                { id: 1, name: 'John Doe', email: 'john@example.com' },
                { id: 3, name: 'Mike Johnson', email: 'mike@example.com' }
            ],
            totalExpenses: 1200
        }
    ];

    const handleCreateGroup = (groupData) => {
        console.log('Creating group:', groupData);
        // Implement group creation logic here
        setOpenCreateDialog(false);
    };

    return (
        <Box sx={{ p: { xs: 2, sm: 3 } }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h5" component="h1">
                    Groups
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
                                            {group.members.map(member => (
                                                <Tooltip key={member.id} title={member.name}>
                                                    <Avatar>{member.name[0]}</Avatar>
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