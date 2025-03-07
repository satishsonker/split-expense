import { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    Grid,
    Card,
    CardContent,
    IconButton,
    Button,
    useTheme,
    useMediaQuery,
    Skeleton,
    Avatar
} from '@mui/material';
import {
    AccountBalance as AccountBalanceIcon,
    Group as GroupIcon,
    Receipt as ReceiptIcon,
    Add as AddIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { GROUP_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { getGroupIcon } from '../utils/groupIcons';
import { getImageUrl } from '../utils/imageUtils';
import CurrencyIcon from '../components/CurrencyIcon';

const Dashboard = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [recentGroups, setRecentGroups] = useState([]);

    const summaryData = {
        totalBalance: 1250.00,
        youOwe: 320.00,
        youAreOwed: 1570.00
    };

    useEffect(() => {
        fetchRecentGroups();
    }, []);

    const fetchRecentGroups = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(GROUP_PATHS.GET_RECENTS);
            setRecentGroups(response || []);
        } catch (error) {
            console.error('Error fetching recent groups:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleNavigateToGroup = (groupId) => {
        navigate(`/groups/${groupId}`);
    };

    const handleCreateGroup = () => {
        navigate('/groups');
    };

    return (
        <Box sx={{ p: { xs: 2, sm: 3 } }}>
            {/* Summary Cards */}
            <Grid container spacing={3}>
                <Grid item xs={12} sm={4}>
                    <Card>
                        <CardContent>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                <AccountBalanceIcon color="primary" sx={{ mr: 1 }} />
                                <Typography variant="h6">Total Balance</Typography>
                            </Box>
                            <Typography variant="h4" color="primary">
                                ${summaryData.totalBalance.toFixed(2)}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={4}>
                    <Card sx={{ bgcolor: '#ffebee' }}>
                        <CardContent>
                            <Typography variant="h6">You Owe</Typography>
                            <Typography variant="h4" color="error">
                                ${summaryData.youOwe.toFixed(2)}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={4}>
                    <Card sx={{ bgcolor: '#e8f5e9' }}>
                        <CardContent>
                            <Typography variant="h6">You are Owed</Typography>
                            <Typography variant="h4" color="success.main">
                                ${summaryData.youAreOwed.toFixed(2)}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>

            {/* Quick Actions */}
            <Box sx={{ mt: 4, mb: 3 }}>
                <Typography variant="h6" gutterBottom>
                    Quick Actions
                </Typography>
                <Grid container spacing={2}>
                    <Grid item xs={6} sm={3}>
                        <Button
                            fullWidth
                            variant="contained"
                            startIcon={<AddIcon />}
                            sx={{ height: '100%' }}
                        >
                            Add Expense
                        </Button>
                    </Grid>
                    <Grid item xs={6} sm={3}>
                        <Button
                            fullWidth
                            variant="outlined"
                            startIcon={<GroupIcon />}
                            onClick={handleCreateGroup}
                            sx={{ height: '100%' }}
                        >
                            New Group
                        </Button>
                    </Grid>
                </Grid>
            </Box>

            {/* Recent Groups */}
            <Box sx={{ mt: 4 }}>
                <Typography variant="h6" gutterBottom>
                    Recent Groups
                </Typography>
                <Grid container spacing={2}>
                    {loading ? (
                        // Loading skeletons
                        [...Array(2)].map((_, index) => (
                            <Grid item xs={12} sm={6} md={4} key={`skeleton-${index}`}>
                                <Card>
                                    <CardContent>
                                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                            <Skeleton variant="circular" width={40} height={40} sx={{ mr: 2 }} />
                                            <Skeleton variant="text" width={120} />
                                        </Box>
                                    </CardContent>
                                </Card>
                            </Grid>
                        ))
                    ) : recentGroups.length > 0 ? (
                        recentGroups.map(group => {
                            const GroupIconComponent = getGroupIcon(group.name);
                            return (
                                <Grid item xs={12} sm={6} md={4} key={group.id}>
                                    <Card 
                                        sx={{ 
                                            cursor: 'pointer',
                                            '&:hover': { bgcolor: 'action.hover' }
                                        }}
                                        onClick={() => handleNavigateToGroup(group.id)}
                                    >
                                        <CardContent>
                                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                                {group.thumbImagePath ? (
                                                    <Avatar
                                                        src={getImageUrl(group.thumbImagePath)}
                                                        sx={{ mr: 2 }}
                                                    />
                                                ) : (
                                                    <Avatar sx={{ mr: 2, bgcolor: 'primary.main' }}>
                                                        <GroupIconComponent />
                                                    </Avatar>
                                                )}
                                                <Typography variant="h6">
                                                    {group.name}
                                                </Typography>
                                            </Box>
                                        </CardContent>
                                    </Card>
                                </Grid>
                            )
                        })
                    ) : (
                        <Grid item xs={12}>
                            <Typography color="textSecondary" align="center">
                                No recent groups found
                            </Typography>
                        </Grid>
                    )}
                </Grid>
            </Box>
        </Box>
    );
};

export default Dashboard; 