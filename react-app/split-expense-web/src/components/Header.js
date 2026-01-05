import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getImageUrl } from '../utils/imageUtils';
import {
    AppBar,
    Toolbar,
    IconButton,
    Typography,
    Menu,
    MenuItem,
    Badge,
    Avatar,
    Box,
    Divider,
    ListItemIcon,
    ListItemText,
    Popover
} from '@mui/material';
import {
    Menu as MenuIcon,
    Notifications as NotificationsIcon,
    Settings as SettingsIcon,
    Person as PersonIcon,
    Login as LoginIcon,
    Logout as LogoutIcon,
    CurrencyExchange as CurrencyIcon
} from '@mui/icons-material';
import { useAuth } from '../context/AuthContext';
import NotificationList from './NotificationList';

const Header = ({ onMenuClick }) => {
    const navigate = useNavigate();
    const { user, logout } = useAuth();
    const [profileAnchor, setProfileAnchor] = useState(null);
    const [imagePreview, setImagePreview] = useState(null);
    const [notificationAnchorEl, setNotificationAnchorEl] = useState(null);
    const [hasNewNotifications, setHasNewNotifications] = useState(false);

    useEffect(() => {
        const userData = JSON.parse(localStorage.getItem('user'));
        if (userData) {
            if (userData.thumbProfilePicture) {
                setImagePreview(getImageUrl(userData.thumbProfilePicture ?? userData.profilePicture));
            }
        }
        checkNewNotifications();
    }, []);

    const checkNewNotifications = () => {
        const lastRead = localStorage.getItem('lastNotificationRead');
        if (!lastRead) {
            setHasNewNotifications(true);
            return;
        }

        // Check if there are new notifications since last read
        // You might want to call an API endpoint here
        const lastReadDate = new Date(lastRead);
        // For now, we'll just check if it's been more than an hour
        const hasNew = new Date() - lastReadDate > 3600000;
        setHasNewNotifications(hasNew);
    };

    const handleProfileClick = (event) => {
        setProfileAnchor(event.currentTarget);
    };

    const handleNotificationClick = (event) => {
        setNotificationAnchorEl(event.currentTarget);
    };

    const handleNotificationClose = () => {
        setNotificationAnchorEl(null);
    };

    const handleClose = () => {
        setProfileAnchor(null);
        handleNotificationClose();
    };

    const handleLogout = () => {
        logout();
        handleClose();
        navigate('/login');
    };

    const handleLogin = () => {
        navigate('/login');
        handleClose();
    };

    return (
        <AppBar position="static">
            <Toolbar sx={{ display: 'flex', justifyContent: 'space-between', minHeight: { xs: 56, sm: 64 }, px: { xs: 1, sm: 2 } }}>
                <IconButton
                    edge="start"
                    color="inherit"
                    aria-label="menu"
                    onClick={onMenuClick}
                    sx={{ mr: 1 }}
                >
                    <MenuIcon />
                </IconButton>

                <Typography variant="h6" component="div" sx={{ flexGrow: 1, fontSize: { xs: '1.1rem', sm: '1.25rem' } }}>
                    Split Expense
                </Typography>

                <IconButton color="inherit" onClick={handleNotificationClick} size="large">
                    <Badge color="error" variant="dot" invisible={!hasNewNotifications}>
                        <NotificationsIcon />
                    </Badge>
                </IconButton>

                <IconButton color="inherit" onClick={handleProfileClick} size="large">
                    {user ? (
                        <Avatar sx={{ width: { xs: 32, sm: 40 }, height: { xs: 32, sm: 40 } }}
                            src={imagePreview}
                        >
                            {user.name?.charAt(0) || 'U'}
                        </Avatar>
                    ) : (
                        <PersonIcon />
                    )}
                </IconButton>

                {/* Profile Menu */}
                <Menu
                    anchorEl={profileAnchor}
                    open={Boolean(profileAnchor)}
                    onClose={handleClose}
                    transformOrigin={{ horizontal: 'right', vertical: 'top' }}
                    anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
                    PaperProps={{
                        sx: {
                            width: { xs: '100vw', sm: 300 },
                            maxHeight: '80vh'
                        }
                    }}
                >
                    {user ? (
                        <>
                            <Box sx={{ px: 2, py: 1 }}>
                                <Typography variant="subtitle1">{user.name}</Typography>
                                <Typography variant="body2" color="textSecondary">
                                    {user.email}
                                </Typography>
                                {user.mobile && (
                                    <Typography variant="body2" color="textSecondary">
                                        {user.mobile}
                                    </Typography>
                                )}
                            </Box>
                            <Divider />
                            <MenuItem onClick={() => {
                                navigate('/profile');
                                handleClose();
                            }}>
                                <ListItemIcon>
                                    <PersonIcon fontSize="small" />
                                </ListItemIcon>
                                <ListItemText>Profile</ListItemText>
                            </MenuItem>
                            <MenuItem>
                                <ListItemIcon>
                                    <CurrencyIcon fontSize="small" />
                                </ListItemIcon>
                                <ListItemText>Currency</ListItemText>
                            </MenuItem>
                            <MenuItem>
                                <ListItemIcon>
                                    <SettingsIcon fontSize="small" />
                                </ListItemIcon>
                                <ListItemText>Settings</ListItemText>
                            </MenuItem>
                            <Divider />
                            <MenuItem onClick={handleLogout}>
                                <ListItemIcon>
                                    <LogoutIcon fontSize="small" />
                                </ListItemIcon>
                                <ListItemText>Logout</ListItemText>
                            </MenuItem>
                        </>
                    ) : (
                        <MenuItem onClick={handleLogin}>
                            <ListItemIcon>
                                <LoginIcon fontSize="small" />
                            </ListItemIcon>
                            <ListItemText>Login</ListItemText>
                        </MenuItem>
                    )}
                </Menu>

                {/* Notifications Menu */}
                <Popover
                    open={Boolean(notificationAnchorEl)}
                    anchorEl={notificationAnchorEl}
                    onClose={handleNotificationClose}
                    anchorOrigin={{
                        vertical: 'bottom',
                        horizontal: 'right',
                    }}
                    transformOrigin={{
                        vertical: 'top',
                        horizontal: 'right',
                    }}
                    PaperProps={{
                        sx: {
                            width: { xs: '100vw', sm: 360 },
                            maxHeight: { xs: '100vh', sm: '80vh' }
                        }
                    }}
                >
                    <NotificationList onClose={handleNotificationClose} />
                </Popover>
            </Toolbar>
        </AppBar>
    );
};

export default Header; 