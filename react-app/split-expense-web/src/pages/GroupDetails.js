import React, { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    IconButton,
    Avatar,
    Card,
    CardContent,
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    ListItemSecondary,
    Divider,
    Fab,
    useTheme,
    useMediaQuery,
    CircularProgress,
    Grid
} from '@mui/material';
import {
    Settings as SettingsIcon,
    Add as AddIcon
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { GROUP_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { getImageUrl } from '../utils/imageUtils';
import { getGroupIcon } from '../utils/groupIcons';
import CurrencyIcon from '../components/CurrencyIcon';
import {
    PieChart,
    Pie,
    Cell,
    ResponsiveContainer,
    Legend,
    Tooltip
} from 'recharts';

const GroupDetails = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { id } = useParams();
    const [group, setGroup] = useState(null);
    const [loading, setLoading] = useState(true);
    const [breakdown, setBreakdown] = useState(null);
    const [breakdownLoading, setBreakdownLoading] = useState(false);

    useEffect(() => {
        fetchGroupDetails();
        fetchExpenseBreakdown();
    }, [id]);

    const fetchGroupDetails = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(`${GROUP_PATHS.GET}${id}`);
            setGroup(response);
        } catch (error) {
            console.error('Error fetching group details:', error);
        } finally {
            setLoading(false);
        }
    };

    const fetchExpenseBreakdown = async () => {
        try {
            setBreakdownLoading(true);
            const response = await apiService.get(`${GROUP_PATHS.GET_EXPENSE_BREAKDOWN}${id}/expense/breakdown`);
            setBreakdown(response);
        } catch (error) {
            console.error('Error fetching expense breakdown:', error);
        } finally {
            setBreakdownLoading(false);
        }
    };

    const handleSettingsClick = () => {
        navigate(`/groups/${id}/settings`);
    };

    const handleAddExpense = () => {
        // Navigate to add expense page
        navigate(`/groups/${id}/expenses/new`);
    };

    const GroupIcon = group?.name ? getGroupIcon(group.name) : null;

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (!group) {
        return (
            <Box sx={{ p: { xs: 2, sm: 3 } }}>
                <Typography variant="h6" color="textSecondary">
                    Group not found
                </Typography>
            </Box>
        );
    }

    return (
        <Box sx={{ p: { xs: 2, sm: 3 } }}>
            {/* Header */}
            <Box sx={{ 
                display: 'flex', 
                justifyContent: 'space-between',
                alignItems: 'center',
                mb: 3
            }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Avatar
                        src={group?.thumbImagePath ? getImageUrl(group.thumbImagePath) : undefined}
                        sx={{ 
                            width: { xs: 56, sm: 64 }, 
                            height: { xs: 56, sm: 64 }
                        }}
                    >
                        {!group?.thumbImagePath && GroupIcon && <GroupIcon />}
                    </Avatar>
                    <Typography variant={isMobile ? 'h6' : 'h5'}>
                        {group?.name}
                    </Typography>
                </Box>
                <IconButton onClick={handleSettingsClick}>
                    <SettingsIcon />
                </IconButton>
            </Box>

            {/* Balance Summary */}
            <Card sx={{ mb: 3 }}>
                <CardContent>
                    <Typography variant="h6" gutterBottom>
                        Group Balance
                    </Typography>
                    <Box sx={{ display: 'flex', gap: 3, flexWrap: 'wrap' }}>
                        <Box>
                            <Typography color="textSecondary" variant="body2" gutterBottom>
                                Total Expenses
                            </Typography>
                            <Typography variant="h6">
                                <CurrencyIcon amount={group?.totalExpenses || 0} />
                            </Typography>
                        </Box>
                        <Box>
                            <Typography color="textSecondary" variant="body2" gutterBottom>
                                You Owe
                            </Typography>
                            <Typography variant="h6" color="error.main">
                                <CurrencyIcon amount={group?.youOwe || 0} />
                            </Typography>
                        </Box>
                        <Box>
                            <Typography color="textSecondary" variant="body2" gutterBottom>
                                You Are Owed
                            </Typography>
                            <Typography variant="h6" color="success.main">
                                <CurrencyIcon amount={group?.youAreOwed || 0} />
                            </Typography>
                        </Box>
                        <Box>
                            <Typography color="textSecondary" variant="body2" gutterBottom>
                                Your Balance
                            </Typography>
                            <Typography 
                                variant="h6" 
                                color={group?.yourBalance > 0 ? 'success.main' : group?.yourBalance < 0 ? 'error.main' : 'text.primary'}
                            >
                                {group?.yourBalance > 0 ? '+' : ''}
                                <CurrencyIcon amount={group?.yourBalance || 0} />
                            </Typography>
                        </Box>
                    </Box>
                </CardContent>
            </Card>

            {/* Expense Breakdown Chart */}
            {breakdown && breakdown.totalSpending > 0 && (
                <Card sx={{ mb: 3 }}>
                    <CardContent>
                        <Typography variant="h6" gutterBottom sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                            Total Spending: <CurrencyIcon amount={breakdown.totalSpending} />
                        </Typography>
                        <Typography variant="body2" color="textSecondary" gutterBottom sx={{ mb: 2 }}>
                            Member-wise expense breakdown
                        </Typography>
                        {breakdownLoading ? (
                            <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                                <CircularProgress size={40} />
                            </Box>
                        ) : (
                            <Grid container spacing={{ xs: 2, sm: 3 }}>
                                <Grid item xs={12} sm={6}>
                                    <ResponsiveContainer width="100%" height={isMobile ? 250 : 300}>
                                        <PieChart>
                                            <Pie
                                                data={breakdown.memberBreakdown.map(m => ({
                                                    name: `${m.firstName} ${m.lastName}`,
                                                    value: parseFloat(m.totalPaid || 0),
                                                    percentage: m.percentage
                                                }))}
                                                cx="50%"
                                                cy="50%"
                                                labelLine={false}
                                                label={({ name, percentage }) => `${name}: ${percentage.toFixed(1)}%`}
                                                outerRadius={isMobile ? 80 : 100}
                                                fill="#8884d8"
                                                dataKey="value"
                                            >
                                                {breakdown.memberBreakdown.map((entry, index) => {
                                                    const colors = [
                                                        theme.palette.primary.main,
                                                        theme.palette.secondary.main,
                                                        theme.palette.success.main,
                                                        theme.palette.warning.main,
                                                        theme.palette.error.main,
                                                        theme.palette.info.main
                                                    ];
                                                    return (
                                                        <Cell 
                                                            key={`cell-${index}`} 
                                                            fill={colors[index % colors.length]} 
                                                        />
                                                    );
                                                })}
                                            </Pie>
                                            <Tooltip 
                                                formatter={(value) => <CurrencyIcon fontSize="small" amount={value} />}
                                                contentStyle={{ fontSize: isMobile ? '12px' : '14px' }}
                                            />
                                        </PieChart>
                                    </ResponsiveContainer>
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5, mt: { xs: 2, sm: 0 } }}>
                                        {breakdown.memberBreakdown.map((member, index) => {
                                            const colors = [
                                                theme.palette.primary.main,
                                                theme.palette.secondary.main,
                                                theme.palette.success.main,
                                                theme.palette.warning.main,
                                                theme.palette.error.main,
                                                theme.palette.info.main
                                            ];
                                            return (
                                                <Box 
                                                    key={member.userId}
                                                    sx={{ 
                                                        display: 'flex', 
                                                        alignItems: 'center', 
                                                        gap: 1.5,
                                                        p: 1.5,
                                                        borderRadius: 1,
                                                        bgcolor: 'action.hover'
                                                    }}
                                                >
                                                    <Box
                                                        sx={{
                                                            width: 16,
                                                            height: 16,
                                                            borderRadius: '50%',
                                                            bgcolor: colors[index % colors.length],
                                                            flexShrink: 0
                                                        }}
                                                    />
                                                    <Avatar
                                                        src={member.profilePicture ? getImageUrl(member.profilePicture) : undefined}
                                                        sx={{ width: 32, height: 32, fontSize: '0.75rem' }}
                                                    >
                                                        {member.firstName?.[0]}
                                                    </Avatar>
                                                    <Box sx={{ flex: 1, minWidth: 0 }}>
                                                        <Typography variant="body2" sx={{ fontWeight: 600, fontSize: { xs: '0.8rem', sm: '0.875rem' } }}>
                                                            {member.firstName} {member.lastName}
                                                        </Typography>
                                                        <Typography variant="caption" color="textSecondary" sx={{ fontSize: { xs: '0.7rem', sm: '0.75rem' } }}>
                                                            {member.percentage.toFixed(1)}% of total
                                                        </Typography>
                                                    </Box>
                                                    <Box sx={{ textAlign: 'right' }}>
                                                        <Typography variant="body2" sx={{ fontWeight: 600, fontSize: { xs: '0.8rem', sm: '0.875rem' } }}>
                                                            <CurrencyIcon fontSize="small" amount={member.totalPaid} />
                                                        </Typography>
                                                        <Typography variant="caption" color="textSecondary" sx={{ fontSize: { xs: '0.7rem', sm: '0.75rem' } }}>
                                                            Paid
                                                        </Typography>
                                                    </Box>
                                                </Box>
                                            );
                                        })}
                                    </Box>
                                </Grid>
                            </Grid>
                        )}
                    </CardContent>
                </Card>
            )}

            {/* Member List with Balances */}
            <Typography variant="h6" gutterBottom sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                Members
            </Typography>
            <Card>
                <List sx={{ p: 0 }}>
                    {group?.members?.map((member, index) => (
                        <React.Fragment key={member.id}>
                            <ListItem sx={{ px: { xs: 1.5, sm: 2 }, py: { xs: 1, sm: 1.5 } }}>
                                <ListItemAvatar>
                                    <Avatar
                                        src={member.addedUser?.profilePicture ? getImageUrl(member.addedUser.profilePicture) : undefined}
                                        sx={{ width: { xs: 40, sm: 48 }, height: { xs: 40, sm: 48 } }}
                                    >
                                        {member.addedUser?.firstName?.[0]}
                                    </Avatar>
                                </ListItemAvatar>
                                <ListItemText
                                    primary={
                                        <Typography variant="body1" sx={{ fontSize: { xs: '0.9rem', sm: '1rem' } }}>
                                            {`${member.addedUser?.firstName} ${member.addedUser?.lastName || ''}`}
                                        </Typography>
                                    }
                                    secondary={
                                        <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                            {member.addedUser?.email}
                                        </Typography>
                                    }
                                />
                                <Typography 
                                    variant="body1"
                                    color={member.balance > 0 ? 'success.main' : member.balance < 0 ? 'error.main' : 'text.primary'}
                                    sx={{ fontWeight: 600, fontSize: { xs: '0.9rem', sm: '1rem' } }}
                                >
                                    {member.balance > 0 ? '+' : ''}
                                    <CurrencyIcon amount={member.balance || 0} />
                                </Typography>
                            </ListItem>
                            {index < group.members.length - 1 && <Divider />}
                        </React.Fragment>
                    ))}
                </List>
            </Card>

            {/* Add Expense FAB */}
            <Fab
                color="primary"
                sx={{
                    position: 'fixed',
                    bottom: 16,
                    right: 16
                }}
                onClick={handleAddExpense}
            >
                <AddIcon />
            </Fab>
        </Box>
    );
};

export default GroupDetails; 