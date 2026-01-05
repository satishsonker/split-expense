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
                            <Typography variant="h4" color="primary" sx={{ fontSize: { xs: '1.8rem', sm: '2.125rem' } }}>
                                ${summaryData.totalBalance.toFixed(2)}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={6} md={4}>
                    <Card sx={{ bgcolor: '#ffebee', height: '100%' }}>
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="h6" sx={{ fontSize: { xs: '0.9rem', sm: '1rem' } }}>You Owe</Typography>
                            <Typography variant="h4" color="error" sx={{ fontSize: { xs: '1.8rem', sm: '2.125rem' } }}>
                                ${summaryData.youOwe.toFixed(2)}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid item xs={12} sm={6} md={4}>
                    <Card sx={{ bgcolor: '#e8f5e9', height: '100%' }}>
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="h6" sx={{ fontSize: { xs: '0.9rem', sm: '1rem' } }}>You are Owed</Typography>
                            <Typography variant="h4" color="success.main" sx={{ fontSize: { xs: '1.8rem', sm: '2.125rem' } }}>
                                ${summaryData.youAreOwed.toFixed(2)}
                            </Typography>
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
                                                <Typography variant="h6" sx={{ fontSize: { xs: '0.95rem', sm: '1.1rem' } }}>
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