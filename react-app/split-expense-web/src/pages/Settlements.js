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
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Divider,
    Chip,
    Tabs,
    Tab
} from '@mui/material';
import {
    ArrowBack as ArrowBackIcon,
    AttachMoney as AttachMoneyIcon,
    CheckCircle as CheckCircleIcon,
    Person as PersonIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { DASHBOARD_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { toast } from 'react-toastify';
import { useAuth } from '../context/AuthContext';
import CurrencyIcon from '../components/CurrencyIcon';
import { getImageUrl } from '../utils/imageUtils';

const Settlements = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { user } = useAuth();
    const [members, setMembers] = useState([]);
    const [loading, setLoading] = useState(false);
    const [settleDialogOpen, setSettleDialogOpen] = useState(false);
    const [selectedMember, setSelectedMember] = useState(null);
    const [settleAmount, setSettleAmount] = useState('');
    const [settleDescription, setSettleDescription] = useState('');
    const [settleLoading, setSettleLoading] = useState(false);
    const [tabValue, setTabValue] = useState(0); // 0 = all, 1 = you owe, 2 = you are owed

    useEffect(() => {
        fetchMemberBalances();
    }, []);

    const fetchMemberBalances = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(DASHBOARD_PATHS.GET_MEMBER_BALANCES);
            setMembers(response || []);
        } catch (error) {
            console.error('Error fetching member balances:', error);
            toast.error('Failed to load member balances');
        } finally {
            setLoading(false);
        }
    };

    const handleSettleClick = (member) => {
        const settleAmountValue = Math.abs(member.netBalance);
        setSelectedMember(member);
        setSettleAmount(settleAmountValue.toFixed(2));
        setSettleDescription('');
        setSettleDialogOpen(true);
    };

    const handleSettleConfirm = async () => {
        if (!selectedMember || !settleAmount || parseFloat(settleAmount) <= 0) {
            toast.error('Please enter a valid amount');
            return;
        }

        try {
            setSettleLoading(true);
            await apiService.post(DASHBOARD_PATHS.SETTLE_AMOUNT, {
                toUserId: selectedMember.userId,
                amount: parseFloat(settleAmount),
                description: settleDescription || `Settlement with ${selectedMember.firstName} ${selectedMember.lastName}`
            });
            toast.success('Amount settled successfully');
            setSettleDialogOpen(false);
            setSelectedMember(null);
            setSettleAmount('');
            setSettleDescription('');
            fetchMemberBalances();
        } catch (error) {
            console.error('Error settling amount:', error);
            toast.error(error.response?.data?.message || 'Failed to settle amount');
        } finally {
            setSettleLoading(false);
        }
    };

    const handleSettleCancel = () => {
        setSettleDialogOpen(false);
        setSelectedMember(null);
        setSettleAmount('');
        setSettleDescription('');
    };

    const getFilteredMembers = () => {
        if (tabValue === 0) return members;
        if (tabValue === 1) return members.filter(m => m.netBalance < 0); // You owe
        return members.filter(m => m.netBalance > 0); // You are owed
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

    const getMemberName = (member) => {
        return `${member.firstName || ''} ${member.lastName || ''}`.trim() || `User ${member.userId}`;
    };

    const filteredMembers = getFilteredMembers();

    return (
        <Box sx={{ p: { xs: 1.5, sm: 2, md: 3 } }}>
            {/* Header */}
            <Box sx={{ display: 'flex', alignItems: 'center', mb: { xs: 2, sm: 3 }, gap: 2 }}>
                <IconButton onClick={() => navigate('/')} size="large">
                    <ArrowBackIcon />
                </IconButton>
                <Box sx={{ flex: 1 }}>
                    <Typography variant="h5" component="h1" sx={{ fontSize: { xs: '1.25rem', sm: '1.5rem' } }}>
                        Settlements
                    </Typography>
                    <Typography variant="body2" color="textSecondary" sx={{ mt: 0.5 }}>
                        {filteredMembers.length} {filteredMembers.length === 1 ? 'member' : 'members'} with balances
                    </Typography>
                </Box>
            </Box>

            {/* Tabs */}
            <Box sx={{ mb: 3, borderBottom: 1, borderColor: 'divider' }}>
                <Tabs value={tabValue} onChange={(e, newValue) => setTabValue(newValue)}>
                    <Tab label="All" />
                    <Tab label="You Owe" />
                    <Tab label="You Are Owed" />
                </Tabs>
            </Box>

            {/* Members List */}
            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                    <CircularProgress />
                </Box>
            ) : filteredMembers.length === 0 ? (
                <Card>
                    <CardContent sx={{ textAlign: 'center', py: 4 }}>
                        <Typography variant="h6" color="textSecondary" gutterBottom>
                            No balances found
                        </Typography>
                        <Typography variant="body2" color="textSecondary">
                            {tabValue === 0 
                                ? "You don't have any outstanding balances with members"
                                : tabValue === 1
                                ? "You don't owe anything to anyone"
                                : "No one owes you anything"}
                        </Typography>
                    </CardContent>
                </Card>
            ) : (
                <Grid container spacing={{ xs: 1.5, sm: 2 }}>
                    {filteredMembers.map(member => {
                        const memberName = getMemberName(member);
                        const isOwing = member.netBalance < 0;
                        const balanceColor = isOwing ? 'error' : 'success';
                        const balanceAmount = Math.abs(member.netBalance);

                        return (
                            <Grid item xs={12} sm={6} md={4} key={member.userId}>
                                <Card
                                    sx={{
                                        height: '100%',
                                        transition: 'all 0.3s ease',
                                        '&:hover': { boxShadow: { xs: 3, sm: 6 } }
                                    }}
                                >
                                    <CardContent sx={{ p: { xs: 1.5, sm: 2 } }}>
                                        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                            <Avatar
                                                src={member.profilePicture ? getImageUrl(member.profilePicture) : undefined}
                                                {...stringAvatar(memberName)}
                                                sx={{ 
                                                    width: { xs: 48, sm: 56 }, 
                                                    height: { xs: 48, sm: 56 },
                                                    mr: 2
                                                }}
                                            >
                                                {memberName.charAt(0).toUpperCase()}
                                            </Avatar>
                                            <Box sx={{ flex: 1, minWidth: 0 }}>
                                                <Typography 
                                                    variant="h6" 
                                                    noWrap
                                                    sx={{ fontSize: { xs: '0.95rem', sm: '1.1rem' } }}
                                                >
                                                    {memberName}
                                                </Typography>
                                                <Typography 
                                                    variant="body2" 
                                                    color="textSecondary"
                                                    noWrap
                                                    sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}
                                                >
                                                    {member.email}
                                                </Typography>
                                            </Box>
                                        </Box>

                                        <Divider sx={{ my: 1.5 }} />

                                        <Box sx={{ mb: 2 }}>
                                            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                                                <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                    You Owe:
                                                </Typography>
                                                <Typography 
                                                    variant="body2" 
                                                    color="error"
                                                    sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}
                                                >
                                                    <CurrencyIcon 
                                                        fontSize="small" 
                                                        amount={member.youOwe || 0}
                                                    />
                                                </Typography>
                                            </Box>
                                            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                                                <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
                                                    You Are Owed:
                                                </Typography>
                                                <Typography 
                                                    variant="body2" 
                                                    color="success.main"
                                                    sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}
                                                >
                                                    <CurrencyIcon 
                                                        fontSize="small" 
                                                        amount={member.youAreOwed || 0}
                                                    />
                                                </Typography>
                                            </Box>
                                            <Divider sx={{ my: 1 }} />
                                            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                                <Typography variant="body2" fontWeight={600} sx={{ fontSize: { xs: '0.85rem', sm: '0.95rem' } }}>
                                                    Net Balance:
                                                </Typography>
                                                <Typography 
                                                    variant="h6" 
                                                    color={balanceColor + '.main'}
                                                    sx={{ fontSize: { xs: '1.1rem', sm: '1.25rem' }, fontWeight: 600 }}
                                                >
                                                    {isOwing ? '-' : '+'}
                                                    <CurrencyIcon 
                                                        fontSize="inherit" 
                                                        amount={balanceAmount}
                                                    />
                                                </Typography>
                                            </Box>
                                        </Box>

                                        <Button
                                            fullWidth
                                            variant="contained"
                                            color={balanceColor}
                                            startIcon={<CheckCircleIcon />}
                                            onClick={() => handleSettleClick(member)}
                                            sx={{ 
                                                fontSize: { xs: '0.8rem', sm: '0.9rem' },
                                                py: { xs: 0.75, sm: 1 }
                                            }}
                                        >
                                            {isOwing ? 'Settle Up' : 'Mark as Paid'}
                                        </Button>
                                    </CardContent>
                                </Card>
                            </Grid>
                        );
                    })}
                </Grid>
            )}

            {/* Settle Dialog */}
            <Dialog 
                open={settleDialogOpen} 
                onClose={handleSettleCancel}
                maxWidth="sm"
                fullWidth
            >
                <DialogTitle>
                    Settle Amount
                </DialogTitle>
                <DialogContent>
                    {selectedMember && (
                        <Box sx={{ mb: 2 }}>
                            <Typography variant="body2" color="textSecondary" gutterBottom>
                                Member:
                            </Typography>
                            <Typography variant="h6">
                                {getMemberName(selectedMember)}
                            </Typography>
                            <Box sx={{ mt: 2, p: 2, bgcolor: 'action.hover', borderRadius: 1 }}>
                                <Typography variant="body2" color="textSecondary" gutterBottom>
                                    Net Balance:
                                </Typography>
                                <Typography 
                                    variant="h5" 
                                    color={selectedMember.netBalance < 0 ? 'error' : 'success.main'}
                                >
                                    {selectedMember.netBalance < 0 ? '-' : '+'}
                                    <CurrencyIcon 
                                        fontSize="inherit" 
                                        amount={Math.abs(selectedMember.netBalance)}
                                    />
                                </Typography>
                            </Box>
                        </Box>
                    )}
                    <TextField
                        fullWidth
                        label="Amount"
                        type="number"
                        value={settleAmount}
                        onChange={(e) => setSettleAmount(e.target.value)}
                        inputProps={{ min: 0, step: 0.01 }}
                        sx={{ mb: 2 }}
                        InputProps={{
                            startAdornment: <AttachMoneyIcon sx={{ mr: 1, color: 'text.secondary' }} />
                        }}
                    />
                    <TextField
                        fullWidth
                        label="Description (Optional)"
                        multiline
                        rows={3}
                        value={settleDescription}
                        onChange={(e) => setSettleDescription(e.target.value)}
                        placeholder="Add a note about this settlement..."
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleSettleCancel} disabled={settleLoading}>
                        Cancel
                    </Button>
                    <Button 
                        onClick={handleSettleConfirm} 
                        variant="contained"
                        disabled={settleLoading || !settleAmount || parseFloat(settleAmount) <= 0}
                        startIcon={settleLoading ? <CircularProgress size={20} /> : <CheckCircleIcon />}
                    >
                        {settleLoading ? 'Settling...' : 'Settle'}
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
};

export default Settlements;

