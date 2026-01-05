import { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    Grid,
    Card,
    CardContent,
    IconButton,
    Fab,
    useTheme,
    useMediaQuery,
    Button,
    CircularProgress,
    TextField,
    InputAdornment,
    Chip,
    Avatar,
    AvatarGroup,
    Tooltip
} from '@mui/material';
import { 
    Add as AddIcon, 
    Delete as DeleteIcon,
    Search as SearchIcon,
    AttachMoney as AttachMoneyIcon,
    Group as GroupIcon,
    Person as PersonIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { EXPENSE_PATHS, GROUP_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { toast } from 'react-toastify';
import { useAuth } from '../context/AuthContext';
import DeleteConfirmationDialog from '../components/DeleteConfirmationDialog';
import CurrencyIcon from '../components/CurrencyIcon';
import dayjs from 'dayjs';

const EXPENSES_PER_PAGE = 10;

const Expenses = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { user } = useAuth();
    const [expenses, setExpenses] = useState([]);
    const [loading, setLoading] = useState(false);
    const [page, setPage] = useState(1);
    const [hasMore, setHasMore] = useState(true);
    const [totalExpenses, setTotalExpenses] = useState(0);
    const [searchTerm, setSearchTerm] = useState('');
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [expenseToDelete, setExpenseToDelete] = useState(null);
    const [deleteLoading, setDeleteLoading] = useState(false);
    const [groups, setGroups] = useState([]);

    useEffect(() => {
        fetchExpenses();
        fetchGroups();
    }, []);

    useEffect(() => {
        const debounceTimer = setTimeout(() => {
            if (searchTerm) {
                searchExpenses();
            } else {
                fetchExpenses();
            }
        }, 500);

        return () => clearTimeout(debounceTimer);
    }, [searchTerm]);

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
            const response = await apiService.get(EXPENSE_PATHS.LIST, {
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

    const searchExpenses = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(EXPENSE_PATHS.SEARCH, {
                searchTerm: searchTerm,
                pageNo: 1,
                pageSize: EXPENSES_PER_PAGE
            });

            if (response?.data) {
                setExpenses(response.data || []);
                setTotalExpenses(response.recordCounts || 0);
                setHasMore((response.recordCounts - (response.pageNo * response.pageSize)) > 0);
                setPage(1);
            }
        } catch (error) {
            console.error('Error searching expenses:', error);
            toast.error('Failed to search expenses');
        } finally {
            setLoading(false);
        }
    };

    const handleLoadMore = () => {
        const nextPage = page + 1;
        setPage(nextPage);
        fetchExpenses(nextPage);
    };

    const handleDeleteExpense = (expense) => {
        setExpenseToDelete(expense);
        setDeleteDialogOpen(true);
    };

    const handleConfirmDelete = async () => {
        try {
            setDeleteLoading(true);
            await apiService.delete(`${EXPENSE_PATHS.DELETE}${expenseToDelete.id}`);
            toast.success('Expense deleted successfully');
            fetchExpenses(1);
            setPage(1);
        } catch (error) {
            console.error('Error deleting expense:', error);
            toast.error(error.response?.data?.message || 'Failed to delete expense');
        } finally {
            setDeleteLoading(false);
            setDeleteDialogOpen(false);
            setExpenseToDelete(null);
        }
    };

    const handleCloseDeleteDialog = () => {
        setDeleteDialogOpen(false);
        setExpenseToDelete(null);
    };

    const getGroupName = (groupId) => {
        if (!groupId) return null;
        const group = groups.find(g => g.id === groupId);
        return group?.name || null;
    };

    const getPaidByName = (paidByUserId) => {
        // This would need to fetch user info or have it in the expense response
        // For now, just return a placeholder
        return paidByUserId === user?.userId || paidByUserId === user?.id ? 'You' : 'Unknown';
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

    const canDeleteExpense = (expense) => {
        return expense.paidByUserId === user?.userId || expense.paidByUserId === user?.id;
    };

    return (
        <Box sx={{ p: { xs: 1.5, sm: 2, md: 3 } }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: { xs: 2, sm: 3 }, gap: 1, flexWrap: 'wrap' }}>
                <Typography variant="h5" component="h1" sx={{ fontSize: { xs: '1.25rem', sm: '1.5rem' } }}>
                    Expenses ({totalExpenses})
                </Typography>
                {!isMobile && (
                    <IconButton
                        color="primary"
                        onClick={() => navigate('/expenses/new')}
                        sx={{ bgcolor: 'primary.light', '&:hover': { bgcolor: 'primary.main' }, flexShrink: 0 }}
                    >
                        <AddIcon />
                    </IconButton>
                )}
            </Box>

            {/* Search Bar */}
            <Box sx={{ mb: { xs: 2, sm: 3 } }}>
                <TextField
                    fullWidth
                    placeholder="Search expenses..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    InputProps={{
                        startAdornment: (
                            <InputAdornment position="start">
                                <SearchIcon />
                            </InputAdornment>
                        ),
                    }}
                    sx={{ maxWidth: { xs: '100%', sm: 400 } }}
                />
            </Box>

            {/* Expenses List */}
            <Grid container spacing={{ xs: 1.5, sm: 2 }}>
                {loading && expenses.length === 0 ? (
                    <Grid item xs={12}>
                        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                            <CircularProgress />
                        </Box>
                    </Grid>
                ) : expenses.length === 0 ? (
                    <Grid item xs={12}>
                        <Card>
                            <CardContent sx={{ textAlign: 'center', py: 4 }}>
                                <Typography variant="h6" color="textSecondary" gutterBottom>
                                    No expenses found
                                </Typography>
                                <Typography variant="body2" color="textSecondary" sx={{ mb: 2 }}>
                                    {searchTerm ? 'Try a different search term' : 'Start by adding your first expense'}
                                </Typography>
                                {!searchTerm && (
                                    <Button
                                        variant="contained"
                                        startIcon={<AddIcon />}
                                        onClick={() => navigate('/expenses/new')}
                                    >
                                        Add Expense
                                    </Button>
                                )}
                            </CardContent>
                        </Card>
                    </Grid>
                ) : (
                    expenses.map(expense => {
                        const groupName = getGroupName(expense.groupId);
                        const paidByName = getPaidByName(expense.paidByUserId);
                        const canDelete = canDeleteExpense(expense);

                        return (
                            <Grid item xs={12} sm={6} md={4} key={expense.id}>
                                <Card
                                    sx={{
                                        height: '100%',
                                        transition: 'all 0.3s ease',
                                        '&:hover': { boxShadow: { xs: 3, sm: 6 } }
                                    }}
                                >
                                    <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                                            <Box sx={{ flex: 1, minWidth: 0 }}>
                                                <Typography 
                                                    variant="h6" 
                                                    noWrap 
                                                    sx={{ 
                                                        fontSize: { xs: '0.95rem', sm: '1.1rem' },
                                                        mb: 0.5
                                                    }}
                                                >
                                                    {expense.description || 'Untitled Expense'}
                                                </Typography>
                                                <Typography 
                                                    variant="body2" 
                                                    color="textSecondary"
                                                    sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}
                                                >
                                                    {dayjs(expense.expenseDate).format('MMM DD, YYYY')}
                                                </Typography>
                                            </Box>
                                            {canDelete && (
                                                <IconButton
                                                    size="small"
                                                    color="error"
                                                    onClick={() => handleDeleteExpense(expense)}
                                                    title="Delete Expense"
                                                    sx={{ p: { xs: 0.5, sm: 1 }, flexShrink: 0 }}
                                                >
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            )}
                                        </Box>

                                        <Box sx={{ mb: 2 }}>
                                            <Typography 
                                                variant="h5" 
                                                color="primary"
                                                sx={{ 
                                                    fontSize: { xs: '1.25rem', sm: '1.5rem' },
                                                    fontWeight: 600
                                                }}
                                            >
                                                <CurrencyIcon 
                                                    fontSize="small" 
                                                    amount={expense.amount || 0}
                                                />
                                            </Typography>
                                        </Box>

                                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                                <PersonIcon sx={{ fontSize: { xs: 16, sm: 18 }, color: 'text.secondary' }} />
                                                <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                    Paid by: {paidByName}
                                                </Typography>
                                            </Box>
                                            {groupName && (
                                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                                    <GroupIcon sx={{ fontSize: { xs: 16, sm: 18 }, color: 'text.secondary' }} />
                                                    <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                        {groupName}
                                                    </Typography>
                                                </Box>
                                            )}
                                            {expense.expenseShares && expense.expenseShares.length > 0 && (
                                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mt: 0.5 }}>
                                                    <AvatarGroup max={3} sx={{ '& .MuiAvatar-root': { width: { xs: 20, sm: 24 }, height: { xs: 20, sm: 24 }, fontSize: { xs: '0.6rem', sm: '0.7rem' } } }}>
                                                        {expense.expenseShares.slice(0, 3).map((share, index) => (
                                                            <Tooltip key={index} title={`Owes ${share.amountOwed || 0}`}>
                                                                <Avatar {...stringAvatar(`User ${share.userId}`)}>
                                                                    {share.userId}
                                                                </Avatar>
                                                            </Tooltip>
                                                        ))}
                                                    </AvatarGroup>
                                                    {expense.expenseShares.length > 3 && (
                                                        <Typography variant="caption" color="textSecondary">
                                                            +{expense.expenseShares.length - 3} more
                                                        </Typography>
                                                    )}
                                                </Box>
                                            )}
                                        </Box>
                                    </CardContent>
                                </Card>
                            </Grid>
                        );
                    })
                )}
            </Grid>

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

            {isMobile && (
                <Fab
                    color="primary"
                    sx={{ position: 'fixed', bottom: { xs: 16, sm: 24 }, right: { xs: 16, sm: 24 } }}
                    onClick={() => navigate('/expenses/new')}
                >
                    <AddIcon />
                </Fab>
            )}

            <DeleteConfirmationDialog
                open={deleteDialogOpen}
                onClose={handleCloseDeleteDialog}
                onConfirm={handleConfirmDelete}
                loading={deleteLoading}
                type="expense"
                data={expenseToDelete}
                title="Delete Expense"
                warningMessage="This action cannot be undone. The expense will be permanently deleted."
            />
        </Box>
    );
};

export default Expenses;

