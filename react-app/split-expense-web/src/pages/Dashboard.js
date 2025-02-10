import { useState } from 'react';
import {
    Box,
    Typography,
    Grid,
    Card,
    CardContent,
    IconButton,
    Button,
    useTheme,
    useMediaQuery
} from '@mui/material';
import {
    AccountBalance as AccountBalanceIcon,
    Group as GroupIcon,
    Receipt as ReceiptIcon,
    Add as AddIcon
} from '@mui/icons-material';

const Dashboard = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const summaryData = {
        totalBalance: 1250.00,
        youOwe: 320.00,
        youAreOwed: 1570.00,
        recentGroups: [
            { id: 1, name: 'Weekend Trip', amount: 450 },
            { id: 2, name: 'Household', amount: 120 },
        ]
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
                    {summaryData.recentGroups.map(group => (
                        <Grid item xs={12} sm={6} md={4} key={group.id}>
                            <Card>
                                <CardContent>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                            <GroupIcon sx={{ mr: 1 }} />
                                            <Typography variant="h6">{group.name}</Typography>
                                        </Box>
                                        <Typography variant="subtitle1" color="primary">
                                            ${group.amount}
                                        </Typography>
                                    </Box>
                                </CardContent>
                            </Card>
                        </Grid>
                    ))}
                </Grid>
            </Box>
        </Box>
    );
};

export default Dashboard; 