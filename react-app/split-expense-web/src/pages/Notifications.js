import React, { useState, useEffect, useCallback } from 'react';
import {
    Box,
    Typography,
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    Avatar,
    Divider,
    CircularProgress,
    useTheme
} from '@mui/material';
import {
    Notifications as NotificationsIcon,
    Group as GroupIcon,
    Person as PersonIcon,
    Receipt as ReceiptIcon
} from '@mui/icons-material';
import { ACTIVITY_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import useInfiniteScroll from '../hooks/useInfiniteScroll';

dayjs.extend(relativeTime);

const PAGE_SIZE = 10;

const Notifications = () => {
    const theme = useTheme();
    const [notifications, setNotifications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [hasMore, setHasMore] = useState(true);
    const [pageNo, setPageNo] = useState(1);

    const fetchNotifications = useCallback(async () => {
        try {
            setLoading(true);
            const response = await apiService.get(ACTIVITY_PATHS.LIST, {
                params: {
                    pageNo,
                    pageSize: PAGE_SIZE
                }
            });

            const newNotifications = response.data || [];
            setNotifications(prev => [...prev, ...newNotifications]);
            setHasMore(newNotifications.length === PAGE_SIZE);
        } catch (error) {
            console.error('Error fetching notifications:', error);
        } finally {
            setLoading(false);
        }
    }, [pageNo]);

    const loadMore = useCallback(() => {
        if (!loading && hasMore) {
            setPageNo(prev => prev + 1);
        }
    }, [loading, hasMore]);

    const [infiniteScrollRef] = useInfiniteScroll(loadMore);

    useEffect(() => {
        fetchNotifications();
    }, [pageNo]);

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

    return (
        <Box sx={{ p: { xs: 2, sm: 3 }, maxWidth: 'md', mx: 'auto' }}>
            <Typography variant="h5" gutterBottom>
                Notifications
            </Typography>

            <List sx={{ bgcolor: 'background.paper' }}>
                {notifications.map((notification, index) => (
                    <React.Fragment key={notification.id}>
                        <ListItem>
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

            <Box
                ref={infiniteScrollRef}
                sx={{
                    display: 'flex',
                    justifyContent: 'center',
                    p: 2
                }}
            >
                {loading && <CircularProgress size={24} />}
            </Box>
        </Box>
    );
};

export default Notifications; 