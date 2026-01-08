import { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    Grid,
    Card,
    CardContent,
    IconButton,
    useTheme,
    useMediaQuery,
    Button,
    CircularProgress,
    Avatar,
    AvatarGroup,
    Tooltip,
    Divider,
    Chip,
    Paper
} from '@mui/material';
import {
    ArrowBack as ArrowBackIcon,
    AttachMoney as AttachMoneyIcon,
    Group as GroupIcon,
    Person as PersonIcon,
    CalendarToday as CalendarIcon
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { DASHBOARD_PATHS, GROUP_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { toast } from 'react-toastify';
import { useAuth } from '../context/AuthContext';
import CurrencyIcon from '../components/CurrencyIcon';
import dayjs from 'dayjs';

const EXPENSES_PER_PAGE = 10;

const ExpenseBreakdown = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { type } = useParams(); // 'owe' or 'owed'
    const { user } = useAuth();
    const [expenses, setExpenses] = useState([]);
    const [loading, setLoading] = useState(false);
    const [page, setPage] = useState(1);
    const [hasMore, setHasMore] = useState(true);
    const [totalExpenses, setTotalExpenses] = useState(0);
    const [groups, setGroups] = useState([]);
    const [summary, setSummary] = useState({ total: 0 });

    const isOwePage = type === 'owe';
    const pageTitle = isOwePage ? 'You Owe' : 'You Are Owed';
    const pageColor = isOwePage ? 'error' : 'success';

    useEffect(() => {
        if (type && (type === 'owe' || type === 'owed')) {
            setExpenses([]);
            setPage(1);
            setHasMore(true);
            fetchExpenses(1);
            fetchGroups();
            fetchSummary();
        }
    }, [type]);

    const fetchSummary = async () => {
        try {
            const response = await apiService.get(DASHBOARD_PATHS.GET_SUMMARY);
            if (response) {
                setSummary({
                    total: isOwePage ? response.youOwe : response.youAreOwed
                });
            }
        } catch (error) {
            console.error('Error fetching summary:', error);
        }
    };

    const fetchGroups = async () => {
        try {
            const response = await apiService.get(GROUP_PATHS.LIST, {
                pageNo: 1,
                pageSize: 100
            });
            setGroups(response?.data || []);
        } catch (error) {
            console.error('Error fetching groups:', error);
        }
    };

    const fetchExpenses = async (pageNum = 1) => {
        try {
            setLoading(true);
            const endpoint = isOwePage 
                ? DASHBOARD_PATHS.GET_EXPENSES_YOU_OWE 
                : DASHBOARD_PATHS.GET_EXPENSES_YOU_ARE_OWED;

            const response = await apiService.get(endpoint, {
                pageNo: pageNum,
                pageSize: EXPENSES_PER_PAGE
            });

            if (response?.data) {
                if (pageNum === 1) {
                    setExpenses(response.data || []);
                } else {
                    setExpenses(prev => [...prev, ...(response.data || [])]);
                }

                setTotalExpenses(response.recordCounts || 0);
                setHasMore((response.recordCounts - (response.pageNo * response.pageSize)) > 0);
            }
        } catch (error) {
            console.error('Error fetching expenses:', error);
            toast.error('Failed to load expenses');
        } finally {
            setLoading(false);
        }
    };

    const handleLoadMore = () => {
        const nextPage = page + 1;
        setPage(nextPage);
        fetchExpenses(nextPage);
    };

    const getGroupName = (groupId) => {
        if (!groupId) return null;
        const group = groups.find(g => g.id === groupId);
        return group?.name || null;
    };

    const getMyShareAmount = (expense) => {
        if (!expense.expenseShares || !user) return 0;
        const userId = user?.userId || user?.id;
        const myShare = expense.expenseShares.find(share => share.userId === userId);
        return myShare?.amountOwed || 0;
    };

    const getTotalOwedToMe = (expense) => {
        if (!expense.expenseShares || !user) return 0;
        const userId = user?.userId || user?.id;
        return expense.expenseShares
            .filter(share => share.userId !== userId)
            .reduce((sum, share) => sum + (share.amountOwed || 0), 0);
    };

    const stringToColor = (string) => {
        let hash = 0;
        for (let i = 0; i < string.length; i += 1) {
            hash = string.charCodeAt(i) + ((hash << 5) - hash);
        }
        let color = '#';
        for (let i = 0; i < 3; i += 1) {
            const value = (hash >> (i * 8)) & 0xff;
            color += `00${value.toString(16)}`.slice(-2);
        }
        return color;
    };

    const stringAvatar = (name) => {
        return {
            sx: {
                bgcolor: stringToColor(name),
            },
            children: name ? `${name.split(' ')[0]?.[0] || ''}${name.split(' ')[1]?.[0] || ''}` : '?',
        };
    };

    const getPaidByName = (paidByUserId) => {
        if (paidByUserId === user?.userId || paidByUserId === user?.id) {
            return 'You';
        }
        return `User ${paidByUserId}`;
    };

    return (
        <Box sx={{ p: { xs: 1.5, sm: 2, md: 3 } }}>
            {/* Header */}
            <Box sx={{ display: 'flex', alignItems: 'center', mb: { xs: 2, sm: 3 }, gap: 2 }}>
                <IconButton onClick={() => navigate('/')} size="large">
                    <ArrowBackIcon />
                </IconButton>
                <Box sx={{ flex: 1 }}>
                    <Typography variant="h5" component="h1" sx={{ fontSize: { xs: '1.25rem', sm: '1.5rem' } }}>
                        {pageTitle}
                    </Typography>
                    <Typography variant="body2" color="textSecondary" sx={{ mt: 0.5 }}>
                        {totalExpenses} {totalExpenses === 1 ? 'expense' : 'expenses'}
                    </Typography>
                </Box>
                <Box sx={{ textAlign: 'right' }}>
                    <Typography variant="h6" color={pageColor + '.main'} sx={{ fontSize: { xs: '1.25rem', sm: '1.5rem' } }}>
                        <CurrencyIcon 
                            fontSize="inherit" 
                            amount={summary.total}
                        />
                    </Typography>
                    <Typography variant="caption" color="textSecondary">
                        Total {pageTitle.toLowerCase()}
                    </Typography>
                </Box>
            </Box>

            {/* Expenses List */}
            {loading && expenses.length === 0 ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                    <CircularProgress />
                </Box>
            ) : expenses.length === 0 ? (
                <Card>
                    <CardContent sx={{ textAlign: 'center', py: 4 }}>
                        <Typography variant="h6" color="textSecondary" gutterBottom>
                            No expenses found
                        </Typography>
                        <Typography variant="body2" color="textSecondary">
                            {isOwePage 
                                ? "You don't owe anything to anyone" 
                                : "No one owes you anything"}
                        </Typography>
                    </CardContent>
                </Card>
            ) : (
                <Grid container spacing={{ xs: 1.5, sm: 2 }}>
                    {expenses.map(expense => {
                        const groupName = getGroupName(expense.groupId);
                        const paidByName = getPaidByName(expense.paidByUserId);
                        const amount = isOwePage ? getMyShareAmount(expense) : getTotalOwedToMe(expense);

                        return (
                            <Grid item xs={12} key={expense.id}>
                                <Card
                                    sx={{
                                        transition: 'all 0.3s ease',
                                        '&:hover': { boxShadow: { xs: 3, sm: 6 } }
                                    }}
                                >
                                    <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                                            <Box sx={{ flex: 1, minWidth: 0 }}>
                                                <Typography 
                                                    variant="h6" 
                                                    sx={{ 
                                                        fontSize: { xs: '0.95rem', sm: '1.1rem' },
                                                        mb: 0.5,
                                                        fontWeight: 600
                                                    }}
                                                >
                                                    {expense.description || 'Untitled Expense'}
                                                </Typography>
                                                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mt: 1 }}>
                                                    <Chip
                                                        icon={<CalendarIcon sx={{ fontSize: 16 }} />}
                                                        label={dayjs(expense.expenseDate).format('MMM DD, YYYY')}
                                                        size="small"
                                                        variant="outlined"
                                                        sx={{ fontSize: { xs: '0.7rem', sm: '0.75rem' } }}
                                                    />
                                                    <Chip
                                                        icon={<PersonIcon sx={{ fontSize: 16 }} />}
                                                        label={`Paid by: ${paidByName}`}
                                                        size="small"
                                                        variant="outlined"
                                                        sx={{ fontSize: { xs: '0.7rem', sm: '0.75rem' } }}
                                                    />
                                                    {groupName && (
                                                        <Chip
                                                            icon={<GroupIcon sx={{ fontSize: 16 }} />}
                                                            label={groupName}
                                                            size="small"
                                                            variant="outlined"
                                                            sx={{ fontSize: { xs: '0.7rem', sm: '0.75rem' } }}
                                                        />
                                                    )}
                                                </Box>
                                            </Box>
                                            <Box sx={{ textAlign: 'right', ml: 2 }}>
                                                <Typography 
                                                    variant="h5" 
                                                    color={pageColor + '.main'}
                                                    sx={{ 
                                                        fontSize: { xs: '1.25rem', sm: '1.5rem' },
                                                        fontWeight: 600
                                                    }}
                                                >
                                                    <CurrencyIcon 
                                                        fontSize="inherit" 
                                                        amount={amount}
                                                    />
                                                </Typography>
                                                <Typography variant="caption" color="textSecondary" sx={{ fontSize: { xs: '0.7rem', sm: '0.75rem' } }}>
                                                    {isOwePage ? 'You owe' : 'Owed to you'}
                                                </Typography>
                                            </Box>
                                        </Box>

                                        <Divider sx={{ my: 1.5 }} />

                                        {/* Expense Details */}
                                        <Box sx={{ mb: 1.5 }}>
                                            <Typography variant="body2" color="textSecondary" sx={{ mb: 1, fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                Total Expense Amount:
                                            </Typography>
                                            <Typography variant="h6" sx={{ fontSize: { xs: '1rem', sm: '1.1rem' } }}>
                                                <CurrencyIcon 
                                                    fontSize="small" 
                                                    amount={expense.amount || 0}
                                                />
                                            </Typography>
                                        </Box>

                                        {/* Participants */}
                                        {expense.expenseShares && expense.expenseShares.length > 0 && (
                                            <Box>
                                                <Typography variant="body2" color="textSecondary" sx={{ mb: 1, fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                    {isOwePage ? 'Split with:' : 'Owed by:'}
                                                </Typography>
                                                <Paper variant="outlined" sx={{ p: 1.5, borderRadius: 1 }}>
                                                    <Grid container spacing={1}>
                                                        {expense.expenseShares
                                                            .filter(share => {
                                                                const userId = user?.userId || user?.id;
                                                                return isOwePage 
                                                                    ? share.userId === userId 
                                                                    : share.userId !== userId;
                                                            })
                                                            .map((share, index) => (
                                                                <Grid item xs={12} sm={6} key={share.id || index}>
                                                                    <Box sx={{ 
                                                                        display: 'flex', 
                                                                        justifyContent: 'space-between', 
                                                                        alignItems: 'center',
                                                                        p: 1,
                                                                        backgroundColor: 'action.hover',
                                                                        borderRadius: 1
                                                                    }}>
                                                                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                                                            <Avatar {...stringAvatar(`User ${share.userId}`)} sx={{ width: 32, height: 32, fontSize: '0.75rem' }}>
                                                                                {share.userId}
                                                                            </Avatar>
                                                                            <Typography variant="body2" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                                                User {share.userId}
                                                                            </Typography>
                                                                        </Box>
                                                                        <Typography variant="body2" fontWeight={600} sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                                            <CurrencyIcon 
                                                                                fontSize="small" 
                                                                                amount={share.amountOwed || 0}
                                                                            />
                                                                        </Typography>
                                                                    </Box>
                                                                </Grid>
                                                            ))}
                                                    </Grid>
                                                </Paper>
                                            </Box>
                                        )}
                                    </CardContent>
                                </Card>
                            </Grid>
                        );
                    })}
                </Grid>
            )}

            {hasMore && !loading && (
                <Box sx={{ mt: { xs: 2, sm: 3 }, textAlign: 'center' }}>
                    <Button
                        variant="outlined"
                        onClick={handleLoadMore}
                        disabled={loading}
                        startIcon={loading && <CircularProgress size={20} />}
                        sx={{ py: { xs: 1, sm: 1.25 } }}
                    >
                        {loading ? 'Loading...' : 'Load More'}
                    </Button>
                </Box>
            )}
        </Box>
    );
};

export default ExpenseBreakdown;

