import React, { useState, useEffect } from 'react';
import {
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    Avatar,
    Typography,
    Box,
    IconButton,
    Divider,
    Button,
    CircularProgress,
    useTheme
} from '@mui/material';
import {
    Notifications as NotificationsIcon,
    Group as GroupIcon,
    Person as PersonIcon,
    Receipt as ReceiptIcon,
    Close as CloseIcon,
    ArrowForward as ArrowForwardIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { ACTIVITY_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';

dayjs.extend(relativeTime);

const NotificationList = ({ onClose }) => {
    const theme = useTheme();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [notifications, setNotifications] = useState([]);

    useEffect(() => {
        fetchNotifications();
    }, []);

    const fetchNotifications = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(`${ACTIVITY_PATHS.LIST}?pageNo=1&pageSize=10`);
            setNotifications(response.data || []);
            localStorage.setItem('lastNotificationRead', new Date().toISOString());
        } catch (error) {
            console.error('Error fetching notifications:', error);
        } finally {
            setLoading(false);
        }
    };

    const getActivityIcon = (activityType, icon) => {
        if (icon) {
            const MuiIcon = require('@mui/icons-material')[icon];
            return MuiIcon ? <MuiIcon /> : <NotificationsIcon />;
        }

        switch (activityType) {
            case 1:
                return <GroupIcon />;
            case 2:
                return <PersonIcon />;
            case 3:
                return <ReceiptIcon />;
            default:
                return <NotificationsIcon />;
        }
    };

    const handleNotificationClick = (notification) => {
        onClose();
        navigate(`/notifications/${notification.id}`);
    };

    const handleViewAll = () => {
        onClose();
        navigate('/notifications');
    };

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                <CircularProgress size={24} />
            </Box>
        );
    }

    return (
        <Box>
            <Box sx={{ 
                p: 2, 
                display: 'flex', 
                alignItems: 'center', 
                justifyContent: 'space-between',
                borderBottom: 1,
                borderColor: 'divider'
            }}>
                <Typography variant="h6">Notifications</Typography>
                <IconButton size="small" onClick={onClose}>
                    <CloseIcon />
                </IconButton>
            </Box>

            {notifications.length > 0 ? (
                <>
                    <List sx={{ py: 0 }}>
                        {notifications.map((notification, index) => (
                            <React.Fragment key={notification.id}>
                                <ListItem 
                                    button 
                                    onClick={() => handleNotificationClick(notification)}
                                    sx={{
                                        '&:hover': {
                                            bgcolor: 'action.hover'
                                        }
                                    }}
                                >
                                    <ListItemAvatar>
                                        <Avatar sx={{ bgcolor: theme.palette.primary.main }}>
                                            {getActivityIcon(notification.activityType, notification.icon)}
                                        </Avatar>
                                    </ListItemAvatar>
                                    <ListItemText
                                        primary={notification.activity}
                                        secondary={dayjs(notification.createdAt).fromNow()}
                                        primaryTypographyProps={{
                                            variant: 'body1',
                                            sx: { fontWeight: 500 }
                                        }}
                                        secondaryTypographyProps={{
                                            variant: 'caption'
                                        }}
                                    />
                                </ListItem>
                                {index < notifications.length - 1 && <Divider />}
                            </React.Fragment>
                        ))}
                    </List>
                    <Box sx={{ p: 2, borderTop: 1, borderColor: 'divider' }}>
                        <Button
                            fullWidth
                            endIcon={<ArrowForwardIcon />}
                            onClick={handleViewAll}
                        >
                            View All Notifications
                        </Button>
                    </Box>
                </>
            ) : (
                <Box sx={{ p: 3, textAlign: 'center' }}>
                    <Typography color="textSecondary">
                        No notifications
                    </Typography>
                </Box>
            )}
        </Box>
    );
};

export default NotificationList; 