import React, { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    IconButton,
    Avatar,
    Card,
    CardContent,
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    ListItemSecondary,
    Divider,
    Fab,
    useTheme,
    useMediaQuery
} from '@mui/material';
import {
    Settings as SettingsIcon,
    Add as AddIcon
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { GROUP_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { getImageUrl } from '../utils/imageUtils';
import { getGroupIcon } from '../utils/groupIcons';
import CurrencyIcon from '../components/CurrencyIcon';

const GroupDetails = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { id } = useParams();
    const [group, setGroup] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchGroupDetails();
    }, [id]);

    const fetchGroupDetails = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(`${GROUP_PATHS.GET}${id}`);
            setGroup(response);
        } catch (error) {
            console.error('Error fetching group details:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSettingsClick = () => {
        navigate(`/groups/${id}/settings`);
    };

    const handleAddExpense = () => {
        // Navigate to add expense page
        navigate(`/groups/${id}/expenses/new`);
    };

    const GroupIcon = group?.name ? getGroupIcon(group.name) : null;

    return (
        <Box sx={{ p: { xs: 2, sm: 3 } }}>
            {/* Header */}
            <Box sx={{ 
                display: 'flex', 
                justifyContent: 'space-between',
                alignItems: 'center',
                mb: 3
            }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Avatar
                        src={group?.thumbImagePath ? getImageUrl(group.thumbImagePath) : undefined}
                        sx={{ 
                            width: { xs: 56, sm: 64 }, 
                            height: { xs: 56, sm: 64 }
                        }}
                    >
                        {!group?.thumbImagePath && GroupIcon && <GroupIcon />}
                    </Avatar>
                    <Typography variant={isMobile ? 'h6' : 'h5'}>
                        {group?.name}
                    </Typography>
                </Box>
                <IconButton onClick={handleSettingsClick}>
                    <SettingsIcon />
                </IconButton>
            </Box>

            {/* Balance Summary */}
            <Card sx={{ mb: 3 }}>
                <CardContent>
                    <Typography variant="h6" gutterBottom>
                        Group Balance
                    </Typography>
                    <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
                        <Box>
                            <Typography color="textSecondary" variant="body2">
                                Total Expenses
                            </Typography>
                            <Typography variant="h6">
                                <CurrencyIcon amount={group?.totalExpenses || 0} />
                            </Typography>
                        </Box>
                        <Box>
                            <Typography color="textSecondary" variant="body2">
                                Your Balance
                            </Typography>
                            <Typography 
                                variant="h6" 
                                color={group?.yourBalance > 0 ? 'success.main' : 'error.main'}
                            >
                                <CurrencyIcon amount={group?.yourBalance || 0} />
                            </Typography>
                        </Box>
                    </Box>
                </CardContent>
            </Card>

            {/* Member List with Balances */}
            <Typography variant="h6" gutterBottom>
                Members
            </Typography>
            <List>
                {group?.members?.map((member, index) => (
                    <React.Fragment key={member.id}>
                        <ListItem>
                            <ListItemAvatar>
                                <Avatar
                                    src={member.addedUser?.profilePicture ? getImageUrl(member.addedUser.profilePicture) : undefined}
                                >
                                    {member.addedUser?.firstName?.[0]}
                                </Avatar>
                            </ListItemAvatar>
                            <ListItemText
                                primary={`${member.addedUser?.firstName} ${member.addedUser?.lastName || ''}`}
                                secondary={member.addedUser?.email}
                            />
                            <Typography 
                                variant="body1"
                                color={member.balance > 0 ? 'success.main' : 'error.main'}
                            >
                                <CurrencyIcon amount={member.balance || 0} />
                            </Typography>
                        </ListItem>
                        {index < group.members.length - 1 && <Divider />}
                    </React.Fragment>
                ))}
            </List>

            {/* Add Expense FAB */}
            <Fab
                color="primary"
                sx={{
                    position: 'fixed',
                    bottom: 16,
                    right: 16
                }}
                onClick={handleAddExpense}
            >
                <AddIcon />
            </Fab>
        </Box>
    );
};

export default GroupDetails; 