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
    ListItemText
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
    const [notificationAnchor, setNotificationAnchor] = useState(null);
    useEffect(() => {
        const userData = JSON.parse(localStorage.getItem('user'));
        if (userData) {
            if (userData.thumbProfilePicture) {
                setImagePreview(getImageUrl(userData.thumbProfilePicture ?? userData.profilePicture));
            }
        }
    }, []);

    const handleProfileClick = (event) => {
        setProfileAnchor(event.currentTarget);
    };

    const handleNotificationClick = (event) => {
        setNotificationAnchor(event.currentTarget);
    };

    const handleClose = () => {
        setProfileAnchor(null);
        setNotificationAnchor(null);
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
            <Toolbar>
                <IconButton
                    edge="start"
                    color="inherit"
                    aria-label="menu"
                    onClick={onMenuClick}
                    sx={{ mr: 2 }}
                >
                    <MenuIcon />
                </IconButton>

                <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                    Split Expense
                </Typography>

                {user && (
                    <IconButton color="inherit" onClick={handleNotificationClick}>
                        <Badge badgeContent={3} color="error">
                            <NotificationsIcon />
                        </Badge>
                    </IconButton>
                )}

                <IconButton color="inherit" onClick={handleProfileClick}>
                    {user ? (
                        <Avatar sx={{ width: 32, height: 32 }}
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
                <NotificationList
                    anchorEl={notificationAnchor}
                    open={Boolean(notificationAnchor)}
                    onClose={handleClose}
                />
            </Toolbar>
        </AppBar>
    );
};

export default Header; 