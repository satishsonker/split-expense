import {
    Menu,
    MenuItem,
    Typography,
    Box,
    List,
    ListItem,
    ListItemText,
    ListItemAvatar,
    Avatar,
    Divider
} from '@mui/material';
import { 
    NotificationsActive as NotificationsActiveIcon,
    Payment as PaymentIcon,
    Group as GroupIcon
} from '@mui/icons-material';

const NotificationList = ({ anchorEl, open, onClose }) => {
    // This would typically come from your backend
    const notifications = [
        {
            id: 1,
            type: 'payment',
            message: 'John Doe sent you $50',
            time: '5 minutes ago'
        },
        {
            id: 2,
            type: 'group',
            message: 'You were added to "Weekend Trip" group',
            time: '1 hour ago'
        },
        {
            id: 3,
            type: 'payment',
            message: 'You sent $30 to Jane Smith',
            time: '2 hours ago'
        }
    ];

    const getIcon = (type) => {
        switch (type) {
            case 'payment':
                return <PaymentIcon />;
            case 'group':
                return <GroupIcon />;
            default:
                return <NotificationsActiveIcon />;
        }
    };

    return (
        <Menu
            anchorEl={anchorEl}
            open={open}
            onClose={onClose}
            transformOrigin={{ horizontal: 'right', vertical: 'top' }}
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            PaperProps={{
                sx: {
                    width: 320,
                    maxHeight: 400,
                }
            }}
        >
            <Box sx={{ p: 2 }}>
                <Typography variant="h6">Notifications</Typography>
            </Box>
            <Divider />
            <List sx={{ p: 0 }}>
                {notifications.map((notification) => (
                    <ListItem key={notification.id} button>
                        <ListItemAvatar>
                            <Avatar>
                                {getIcon(notification.type)}
                            </Avatar>
                        </ListItemAvatar>
                        <ListItemText 
                            primary={notification.message}
                            secondary={notification.time}
                        />
                    </ListItem>
                ))}
            </List>
            {notifications.length === 0 && (
                <Box sx={{ p: 2, textAlign: 'center' }}>
                    <Typography color="textSecondary">
                        No new notifications
                    </Typography>
                </Box>
            )}
        </Menu>
    );
};

export default NotificationList; 