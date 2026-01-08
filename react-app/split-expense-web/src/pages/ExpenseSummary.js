import { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    Card,
    CardContent,
    IconButton,
    useTheme,
    useMediaQuery,
    CircularProgress,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    Grid
} from '@mui/material';
import {
    ArrowBack as ArrowBackIcon,
    TrendingUp as TrendingUpIcon,
    TrendingDown as TrendingDownIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { DASHBOARD_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { toast } from 'react-toastify';
import CurrencyIcon from '../components/CurrencyIcon';
import {
    LineChart,
    Line,
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ResponsiveContainer
} from 'recharts';

const ExpenseSummary = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [monthlyData, setMonthlyData] = useState([]);
    const [months, setMonths] = useState(6);

    useEffect(() => {
        fetchMonthlySummary();
    }, [months]);

    const fetchMonthlySummary = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(DASHBOARD_PATHS.GET_MONTHLY_SUMMARY, { months });
            setMonthlyData(response || []);
        } catch (error) {
            console.error('Error fetching monthly summary:', error);
            toast.error('Failed to load expense summary');
        } finally {
            setLoading(false);
        }
    };

    const chartData = monthlyData.map(item => ({
        month: item.month,
        total: parseFloat(item.totalAmount || 0),
        youOwe: parseFloat(item.youOwe || 0),
        youAreOwed: parseFloat(item.youAreOwed || 0),
        netBalance: parseFloat(item.netBalance || 0)
    }));

    const totalExpenses = monthlyData.reduce((sum, item) => sum + parseFloat(item.totalAmount || 0), 0);
    const totalYouOwe = monthlyData.reduce((sum, item) => sum + parseFloat(item.youOwe || 0), 0);
    const totalYouAreOwed = monthlyData.reduce((sum, item) => sum + parseFloat(item.youAreOwed || 0), 0);
    const totalNetBalance = totalYouAreOwed - totalYouOwe;
    const totalExpenseCount = monthlyData.reduce((sum, item) => sum + (item.expenseCount || 0), 0);

    return (
        <Box sx={{ p: { xs: 1.5, sm: 2, md: 3 } }}>
            {/* Header */}
            <Box sx={{ display: 'flex', alignItems: 'center', mb: { xs: 2, sm: 3 }, gap: 2 }}>
                <IconButton onClick={() => navigate('/')} size="large">
                    <ArrowBackIcon />
                </IconButton>
                <Box sx={{ flex: 1 }}>
                    <Typography variant="h5" component="h1" sx={{ fontSize: { xs: '1.25rem', sm: '1.5rem' } }}>
                        Expense Summary
                    </Typography>
                    <Typography variant="body2" color="textSecondary" sx={{ mt: 0.5 }}>
                        Monthly expense overview
                    </Typography>
                </Box>
                <FormControl size="small" sx={{ minWidth: { xs: 100, sm: 120 } }}>
                    <InputLabel>Months</InputLabel>
                    <Select
                        value={months}
                        label="Months"
                        onChange={(e) => setMonths(e.target.value)}
                    >
                        <MenuItem value={3}>3 Months</MenuItem>
                        <MenuItem value={6}>6 Months</MenuItem>
                        <MenuItem value={12}>12 Months</MenuItem>
                    </Select>
                </FormControl>
            </Box>

            {/* Summary Cards */}
            <Grid container spacing={{ xs: 1.5, sm: 2 }} sx={{ mb: { xs: 2, sm: 3 } }}>
                <Grid item xs={6} sm={3}>
                    <Card>
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.7rem', sm: '0.875rem' } }}>
                                Total Expenses
                            </Typography>
                            <Typography variant="h6" sx={{ fontSize: { xs: '1rem', sm: '1.25rem' }, mt: 0.5 }}>
                                <CurrencyIcon fontSize="inherit" amount={totalExpenses} />
                            </Typography>
                            <Typography variant="caption" color="textSecondary" sx={{ fontSize: { xs: '0.65rem', sm: '0.75rem' } }}>
                                {totalExpenseCount} expenses
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>
                <Grid item xs={6} sm={3}>
                    <Card sx={{ bgcolor: '#ffebee' }}>
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.7rem', sm: '0.875rem' } }}>
                                You Owe
                            </Typography>
                            <Typography variant="h6" color="error" sx={{ fontSize: { xs: '1rem', sm: '1.25rem' }, mt: 0.5 }}>
                                <CurrencyIcon fontSize="inherit" amount={totalYouOwe} />
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>
                <Grid item xs={6} sm={3}>
                    <Card sx={{ bgcolor: '#e8f5e9' }}>
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.7rem', sm: '0.875rem' } }}>
                                You Are Owed
                            </Typography>
                            <Typography variant="h6" color="success.main" sx={{ fontSize: { xs: '1rem', sm: '1.25rem' }, mt: 0.5 }}>
                                <CurrencyIcon fontSize="inherit" amount={totalYouAreOwed} />
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>
                <Grid item xs={6} sm={3}>
                    <Card>
                        <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                            <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.7rem', sm: '0.875rem' } }}>
                                Net Balance
                            </Typography>
                            <Typography 
                                variant="h6" 
                                color={totalNetBalance > 0 ? 'success.main' : totalNetBalance < 0 ? 'error.main' : 'text.primary'}
                                sx={{ fontSize: { xs: '1rem', sm: '1.25rem' }, mt: 0.5 }}
                            >
                                {totalNetBalance > 0 ? '+' : ''}
                                <CurrencyIcon fontSize="inherit" amount={totalNetBalance} />
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>

            {/* Charts */}
            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                    <CircularProgress />
                </Box>
            ) : chartData.length > 0 ? (
                <Grid container spacing={{ xs: 1.5, sm: 2 }}>
                    {/* Monthly Total Expenses Chart */}
                    <Grid item xs={12}>
                        <Card>
                            <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                                <Typography variant="h6" gutterBottom sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                                    Monthly Total Expenses
                                </Typography>
                                <ResponsiveContainer width="100%" height={isMobile ? 250 : 300}>
                                    <BarChart data={chartData}>
                                        <CartesianGrid strokeDasharray="3 3" />
                                        <XAxis 
                                            dataKey="month" 
                                            angle={isMobile ? -45 : 0}
                                            textAnchor={isMobile ? 'end' : 'middle'}
                                            height={isMobile ? 80 : 60}
                                            tick={{ fontSize: isMobile ? 10 : 12 }}
                                        />
                                        <YAxis tick={{ fontSize: isMobile ? 10 : 12 }} />
                                        <Tooltip 
                                            formatter={(value) => <CurrencyIcon fontSize="small" amount={value} />}
                                            contentStyle={{ fontSize: isMobile ? '12px' : '14px' }}
                                        />
                                        <Legend wrapperStyle={{ fontSize: isMobile ? '12px' : '14px' }} />
                                        <Bar dataKey="total" fill={theme.palette.primary.main} name="Total Expenses" />
                                    </BarChart>
                                </ResponsiveContainer>
                            </CardContent>
                        </Card>
                    </Grid>

                    {/* Monthly Balance Chart */}
                    <Grid item xs={12}>
                        <Card>
                            <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                                <Typography variant="h6" gutterBottom sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                                    Monthly Balance Trend
                                </Typography>
                                <ResponsiveContainer width="100%" height={isMobile ? 250 : 300}>
                                    <LineChart data={chartData}>
                                        <CartesianGrid strokeDasharray="3 3" />
                                        <XAxis 
                                            dataKey="month" 
                                            angle={isMobile ? -45 : 0}
                                            textAnchor={isMobile ? 'end' : 'middle'}
                                            height={isMobile ? 80 : 60}
                                            tick={{ fontSize: isMobile ? 10 : 12 }}
                                        />
                                        <YAxis tick={{ fontSize: isMobile ? 10 : 12 }} />
                                        <Tooltip 
                                            formatter={(value) => <CurrencyIcon fontSize="small" amount={value} />}
                                            contentStyle={{ fontSize: isMobile ? '12px' : '14px' }}
                                        />
                                        <Legend wrapperStyle={{ fontSize: isMobile ? '12px' : '14px' }} />
                                        <Line 
                                            type="monotone" 
                                            dataKey="youOwe" 
                                            stroke={theme.palette.error.main} 
                                            name="You Owe"
                                            strokeWidth={2}
                                        />
                                        <Line 
                                            type="monotone" 
                                            dataKey="youAreOwed" 
                                            stroke={theme.palette.success.main} 
                                            name="You Are Owed"
                                            strokeWidth={2}
                                        />
                                        <Line 
                                            type="monotone" 
                                            dataKey="netBalance" 
                                            stroke={theme.palette.primary.main} 
                                            name="Net Balance"
                                            strokeWidth={2}
                                        />
                                    </LineChart>
                                </ResponsiveContainer>
                            </CardContent>
                        </Card>
                    </Grid>
                </Grid>
            ) : (
                <Card>
                    <CardContent sx={{ textAlign: 'center', py: 4 }}>
                        <Typography variant="h6" color="textSecondary" gutterBottom>
                            No data available
                        </Typography>
                        <Typography variant="body2" color="textSecondary">
                            No expenses found for the selected period
                        </Typography>
                    </CardContent>
                </Card>
            )}
        </Box>
    );
};

export default ExpenseSummary;

