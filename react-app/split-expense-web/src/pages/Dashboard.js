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
import { GROUP_PATHS, DASHBOARD_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { getGroupIcon } from '../utils/groupIcons';
import { getImageUrl } from '../utils/imageUtils';
import { useAuth } from '../context/AuthContext';
import CurrencyIcon from '../components/CurrencyIcon';

const Dashboard = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { user } = useAuth();
    const [loading, setLoading] = useState(true);
    const [summaryLoading, setSummaryLoading] = useState(false);
    const [recentGroups, setRecentGroups] = useState([]);
    const [summaryData, setSummaryData] = useState({
        totalBalance: 0.00,
        youOwe: 0.00,
        youAreOwed: 0.00
    });

    useEffect(() => {
        fetchRecentGroups();
    }, []);

    useEffect(() => {
        if (user) {
            fetchExpenseSummary();
        }
    }, [user]);

    const fetchExpenseSummary = async () => {
        try {
            setSummaryLoading(true);
            console.log('Fetching dashboard summary from API');

            const response = await apiService.get(DASHBOARD_PATHS.GET_SUMMARY);
            
            console.log('Dashboard summary response:', response);

            if (response) {
                setSummaryData({
                    totalBalance: response.totalBalance || 0,
                    youOwe: response.youOwe || 0,
                    youAreOwed: response.youAreOwed || 0
                });
            }
        } catch (error) {
            console.error('Error fetching dashboard summary:', error);
            console.error('Error details:', error.response || error.message);
        } finally {
            setSummaryLoading(false);
        }
    };

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
        <Box sx={{ p: { xs: 1.5, sm: 2, md: 3 } }}>
            {/* Summary Cards */}
            <Grid container spacing={{ xs: 1.5, sm: 2, md: 3 }}>
                <Grid item xs={12} sm={6} md={4}>
                    <Card sx={{ height: '100%' }}>
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                <AccountBalanceIcon color="primary" sx={{ mr: 1, fontSize: { xs: 24, sm: 28 } }} />
                                <Typography variant="h6" sx={{ fontSize: { xs: '0.9rem', sm: '1rem' } }}>Total Balance</Typography>
                            </Box>
                            {summaryLoading ? (
                                <Skeleton variant="text" width={100} height={40} />
                            ) : (
                                <Typography variant="h4" color="primary" sx={{ fontSize: { xs: '1.8rem', sm: '2.125rem' } }}>
                                    <CurrencyIcon 
                                        fontSize="inherit" 
                                        amount={summaryData.totalBalance}
                                    />
                                </Typography>
                            )}
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={6} md={4}>
                    <Card 
                        sx={{ 
                            bgcolor: '#ffebee', 
                            height: '100%',
                            cursor: 'pointer',
                            transition: 'all 0.3s ease',
                            '&:hover': { 
                                boxShadow: 6,
                                transform: 'translateY(-2px)'
                            }
                        }}
                        onClick={() => navigate('/dashboard/breakdown/owe')}
                    >
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="h6" sx={{ fontSize: { xs: '0.9rem', sm: '1rem' } }}>You Owe</Typography>
                            {summaryLoading ? (
                                <Skeleton variant="text" width={100} height={40} />
                            ) : (
                                <Typography variant="h4" color="error" sx={{ fontSize: { xs: '1.8rem', sm: '2.125rem' } }}>
                                    <CurrencyIcon 
                                        fontSize="inherit" 
                                        amount={summaryData.youOwe}
                                    />
                                </Typography>
                            )}
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={6} md={4}>
                    <Card 
                        sx={{ 
                            bgcolor: '#e8f5e9', 
                            height: '100%',
                            cursor: 'pointer',
                            transition: 'all 0.3s ease',
                            '&:hover': { 
                                boxShadow: 6,
                                transform: 'translateY(-2px)'
                            }
                        }}
                        onClick={() => navigate('/dashboard/breakdown/owed')}
                    >
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="h6" sx={{ fontSize: { xs: '0.9rem', sm: '1rem' } }}>You are Owed</Typography>
                            {summaryLoading ? (
                                <Skeleton variant="text" width={100} height={40} />
                            ) : (
                                <Typography variant="h4" color="success.main" sx={{ fontSize: { xs: '1.8rem', sm: '2.125rem' } }}>
                                    <CurrencyIcon 
                                        fontSize="inherit" 
                                        amount={summaryData.youAreOwed}
                                    />
                                </Typography>
                            )}
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>

            {/* Quick Actions */}
            <Box sx={{ mt: { xs: 2, sm: 3, md: 4 }, mb: { xs: 2, sm: 3 } }}>
                <Typography variant="h6" gutterBottom sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                    Quick Actions
                </Typography>
                <Grid container spacing={{ xs: 1, sm: 2 }}>
                    <Grid item xs={6} sm={3}>
                        <Button
                            fullWidth
                            variant="contained"
                            startIcon={<AddIcon sx={{ fontSize: { xs: 20, sm: 24 } }} />}
                            onClick={() => navigate('/expenses/new')}
                            sx={{ height: '100%', py: { xs: 1.5, sm: 2 }, fontSize: { xs: '0.8rem', sm: '1rem' } }}
                        >
                            Add Expense
                        </Button>
                    </Grid>
                    <Grid item xs={6} sm={3}>
                        <Button
                            fullWidth
                            variant="outlined"
                            startIcon={<GroupIcon sx={{ fontSize: { xs: 20, sm: 24 } }} />}
                            onClick={handleCreateGroup}
                            sx={{ height: '100%', py: { xs: 1.5, sm: 2 }, fontSize: { xs: '0.8rem', sm: '1rem' } }}
                        >
                            New Group
                        </Button>
                    </Grid>
                    <Grid item xs={6} sm={3}>
                        <Button
                            fullWidth
                            variant="outlined"
                            startIcon={<ReceiptIcon sx={{ fontSize: { xs: 20, sm: 24 } }} />}
                            onClick={() => navigate('/expenses/summary')}
                            sx={{ height: '100%', py: { xs: 1.5, sm: 2 }, fontSize: { xs: '0.8rem', sm: '1rem' } }}
                        >
                            Summary
                        </Button>
                    </Grid>
                </Grid>
            </Box>

            {/* Recent Groups */}
            <Box sx={{ mt: { xs: 2, sm: 3, md: 4 } }}>
                <Typography variant="h6" gutterBottom sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                    Recent Groups
                </Typography>
                <Grid container spacing={{ xs: 1.5, sm: 2 }}>
                    {loading ? (
                        // Loading skeletons
                        [...Array(2)].map((_, index) => (
                            <Grid item xs={12} sm={6} md={4} key={`skeleton-${index}`}>
                                <Card>
                                    <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
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
                                            height: '100%',
                                            '&:hover': { bgcolor: 'action.hover' },
                                            transition: 'all 0.3s ease'
                                        }}
                                        onClick={() => handleNavigateToGroup(group.id)}
                                    >
                                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                                {group.thumbImagePath ? (
                                                    <Avatar
                                                        src={getImageUrl(group.thumbImagePath)}
                                                        sx={{ mr: 2, width: { xs: 40, sm: 48 }, height: { xs: 40, sm: 48 } }}
                                                    />
                                                ) : (
                                                    <Avatar sx={{ mr: 2, bgcolor: 'primary.main', width: { xs: 40, sm: 48 }, height: { xs: 40, sm: 48 } }}>
                                                        <GroupIconComponent />
                                                    </Avatar>
                                                )}
                                                <Box sx={{ flex: 1, minWidth: 0 }}>
                                                    <Typography variant="h6" sx={{ fontSize: { xs: '0.95rem', sm: '1.1rem' } }} noWrap>
                                                        {group.name}
                                                    </Typography>
                                                    <Typography 
                                                        variant="body2" 
                                                        color={group.yourBalance > 0 ? 'success.main' : group.yourBalance < 0 ? 'error.main' : 'text.secondary'}
                                                        sx={{ 
                                                            fontSize: { xs: '0.75rem', sm: '0.875rem' },
                                                            fontWeight: 600,
                                                            mt: 0.5,
                                                            display: 'flex',
                                                            alignItems: 'center',
                                                            gap: 0.5
                                                        }}
                                                    >
                                                        {group.yourBalance > 0 ? '+' : ''}
                                                        <CurrencyIcon 
                                                            fontSize="inherit" 
                                                            amount={group.yourBalance ?? 0}
                                                        />
                                                    </Typography>
                                                </Box>
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