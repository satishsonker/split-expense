import {
    Box,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Divider,
    Typography,
    IconButton
} from '@mui/material';
import {
    Dashboard as DashboardIcon,
    Group as GroupIcon,
    Receipt as ReceiptIcon,
    AccountBalance as AccountBalanceIcon,
    Settings as SettingsIcon,
    Close as CloseIcon,
    Contacts as ContactsIcon,
    BarChart as BarChartIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const LeftMenu = ({ onClose }) => {
    const navigate = useNavigate();
    const { user } = useAuth();

    const menuItems = [
        { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
        { text: 'Groups', icon: <GroupIcon />, path: '/groups' },
        { text: 'Expenses', icon: <ReceiptIcon />, path: '/expenses' },
        { text: 'Expense Summary', icon: <BarChartIcon />, path: '/expenses/summary' },
        { text: 'Settlements', icon: <AccountBalanceIcon />, path: '/settlements' },
        { text: 'Settings', icon: <SettingsIcon />, path: '/settings' },
        { text: 'Contacts', icon: <ContactsIcon />, path: '/contacts' },
    ];

    const handleNavigation = (path) => {
        navigate(path);
        onClose();
    };

    return (
        <Box sx={{ width: 280, p: 2 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Typography variant="h6">Menu</Typography>
                <IconButton onClick={onClose}>
                    <CloseIcon />
                </IconButton>
            </Box>

            {user && (
                <Box sx={{ mb: 2, p: 2, bgcolor: 'primary.light', borderRadius: 1 }}>
                    <Typography variant="subtitle1" color="white">
                        {user.name}
                    </Typography>
                    <Typography variant="body2" color="white" sx={{ opacity: 0.8 }}>
                        {user.email}
                    </Typography>
                </Box>
            )}

            <Divider sx={{ mb: 2 }} />

            <List>
                {menuItems.map((item) => (
                    <ListItem
                        button
                        key={item.text}
                        onClick={() => handleNavigation(item.path)}
                        sx={{
                            borderRadius: 1,
                            mb: 1,
                            '&:hover': {
                                bgcolor: 'primary.light',
                                '& .MuiListItemIcon-root, & .MuiListItemText-primary': {
                                    color: 'white',
                                },
                            },
                        }}
                    >
                        <ListItemIcon sx={{ minWidth: 40 }}>
                            {item.icon}
                        </ListItemIcon>
                        <ListItemText primary={item.text} />
                    </ListItem>
                ))}
            </List>
        </Box>
    );
};

export default LeftMenu; 