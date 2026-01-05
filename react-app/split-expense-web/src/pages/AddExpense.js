import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import {
    Box,
    Typography,
    TextField,
    Button,
    Grid,
    Card,
    CardContent,
    MenuItem,
    Select,
    FormControl,
    InputLabel,
    Chip,
    Avatar,
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    IconButton,
    Paper,
    Divider,
    CircularProgress,
    useTheme,
    useMediaQuery
} from '@mui/material';
import {
    Close as CloseIcon,
    Person as PersonIcon,
    AttachMoney as AttachMoneyIcon
} from '@mui/icons-material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { toast } from 'react-toastify';
import { apiService } from '../utils/axios';
import { EXPENSE_PATHS, GROUP_PATHS, SPLIT_TYPE_PATHS, CONTACT_PATHS } from '../constants/apiPaths';
import { useAuth } from '../context/AuthContext';
import dayjs from 'dayjs';

const validationSchema = Yup.object({
    description: Yup.string()
        .required('Description is required')
        .max(255, 'Description must be less than 255 characters'),
    amount: Yup.number()
        .required('Amount is required')
        .positive('Amount must be positive')
        .min(0.01, 'Amount must be at least 0.01'),
    expenseDate: Yup.date()
        .required('Date is required'),
    paidByUserId: Yup.number()
        .required('Paid by is required'),
    groupId: Yup.number()
        .nullable(),
    splitTypeId: Yup.number()
        .required('Split type is required'),
    expenseShares: Yup.array()
        .min(1, 'At least one person must be included')
        .required('Expense shares are required')
});

const AddExpense = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { groupId } = useParams();
    const { user } = useAuth();
    const [loading, setLoading] = useState(false);
    const [groups, setGroups] = useState([]);
    const [splitTypes, setSplitTypes] = useState([]);
    const [contacts, setContacts] = useState([]);
    const [selectedContacts, setSelectedContacts] = useState([]);
    const [splitValues, setSplitValues] = useState({});

    useEffect(() => {
        fetchGroups();
        fetchSplitTypes();
        fetchContacts();
    }, []);

    useEffect(() => {
        if (groupId) {
            formik.setFieldValue('groupId', parseInt(groupId));
            fetchGroupMembers(groupId);
        }
    }, [groupId]);

    const fetchGroups = async () => {
        try {
            const response = await apiService.get(GROUP_PATHS.LIST, {
                pageNo: 1,
                pageSize: 100
            });
            setGroups(response.data || []);
        } catch (error) {
            console.error('Error fetching groups:', error);
            toast.error('Failed to load groups');
        }
    };

    const fetchSplitTypes = async () => {
        try {
            const response = await apiService.get(SPLIT_TYPE_PATHS.LIST, {
                pageNo: 1,
                pageSize: 100
            });
            setSplitTypes(response.data || []);
        } catch (error) {
            console.error('Error fetching split types:', error);
            toast.error('Failed to load split types');
        }
    };

    const fetchContacts = async () => {
        try {
            const response = await apiService.get(CONTACT_PATHS.LIST, {
                pageNo: 1,
                pageSize: 500
            });
            setContacts(response.data || []);
        } catch (error) {
            console.error('Error fetching contacts:', error);
            toast.error('Failed to load contacts');
        }
    };

    const fetchGroupMembers = async (id) => {
        try {
            const response = await apiService.get(`${GROUP_PATHS.GET}${id}`);
            if (response?.members) {
                const memberIds = response.members.map(m => m.friendId || m.userId);
                setSelectedContacts(memberIds);
                // Initialize split values for group members
                const initialSplitValues = {};
                memberIds.forEach(memberId => {
                    initialSplitValues[memberId] = 0;
                });
                setSplitValues(initialSplitValues);
            }
        } catch (error) {
            console.error('Error fetching group members:', error);
        }
    };

    const calculateShares = (splitTypeId, amount, selectedContacts) => {
        const shares = [];
        const splitType = splitTypes.find(st => st.id === splitTypeId);
        
        if (!splitType) return shares;

        switch (splitType.name?.toLowerCase()) {
            case 'equal':
                const equalAmount = amount / (selectedContacts.length + 1); // +1 for paid by user
                selectedContacts.forEach(contactId => {
                    shares.push({
                        userId: contactId,
                        splitTypeId: splitTypeId,
                        amountOwed: equalAmount,
                        exactAmount: equalAmount
                    });
                });
                break;
            case 'percentage':
                selectedContacts.forEach(contactId => {
                    const percentage = splitValues[contactId] || 0;
                    shares.push({
                        userId: contactId,
                        splitTypeId: splitTypeId,
                        percentage: percentage,
                        amountOwed: (amount * percentage) / 100
                    });
                });
                break;
            case 'exactamount':
                selectedContacts.forEach(contactId => {
                    const exactAmount = splitValues[contactId] || 0;
                    shares.push({
                        userId: contactId,
                        splitTypeId: splitTypeId,
                        exactAmount: exactAmount,
                        amountOwed: exactAmount
                    });
                });
                break;
            case 'shares':
                const totalShares = Object.values(splitValues).reduce((sum, val) => sum + (val || 0), 0);
                selectedContacts.forEach(contactId => {
                    const sharesValue = splitValues[contactId] || 0;
                    shares.push({
                        userId: contactId,
                        splitTypeId: splitTypeId,
                        shares: sharesValue,
                        amountOwed: totalShares > 0 ? (amount * sharesValue) / totalShares : 0
                    });
                });
                break;
            default:
                // Unequal or custom
                selectedContacts.forEach(contactId => {
                    const exactAmount = splitValues[contactId] || 0;
                    shares.push({
                        userId: contactId,
                        splitTypeId: splitTypeId,
                        exactAmount: exactAmount,
                        amountOwed: exactAmount
                    });
                });
        }
        
        return shares;
    };

    const formik = useFormik({
        initialValues: {
            description: '',
            amount: '',
            expenseDate: dayjs(),
            paidByUserId: user?.userId || user?.id || 0,
            groupId: groupId ? parseInt(groupId) : null,
            splitTypeId: '',
            expenseShares: []
        },
        validationSchema,
        onSubmit: async (values) => {
            try {
                setLoading(true);
                
                const shares = calculateShares(
                    values.splitTypeId,
                    parseFloat(values.amount),
                    selectedContacts
                );

                const expenseData = {
                    description: values.description,
                    amount: parseFloat(values.amount),
                    expenseDate: values.expenseDate.toISOString(),
                    paidByUserId: values.paidByUserId,
                    groupId: values.groupId,
                    splitTypeId: values.splitTypeId,
                    expenseShares: shares
                };

                await apiService.post(EXPENSE_PATHS.CREATE, expenseData);
                toast.success('Expense added successfully');
                
                if (groupId) {
                    navigate(`/groups/${groupId}`);
                } else {
                    navigate('/groups');
                }
            } catch (error) {
                console.error('Error creating expense:', error);
                toast.error(error.response?.data?.message || 'Failed to create expense');
            } finally {
                setLoading(false);
            }
        }
    });

    const handleContactToggle = (contactId) => {
        setSelectedContacts(prev => {
            if (prev.includes(contactId)) {
                const newList = prev.filter(id => id !== contactId);
                const newSplitValues = { ...splitValues };
                delete newSplitValues[contactId];
                setSplitValues(newSplitValues);
                return newList;
            } else {
                return [...prev, contactId];
            }
        });
    };

    const handleSplitValueChange = (contactId, value) => {
        setSplitValues(prev => ({
            ...prev,
            [contactId]: parseFloat(value) || 0
        }));
    };

    const getContactName = (contactId) => {
        const contact = contacts.find(c => (c.contactId || c.id || c.userId) === contactId);
        return contact ? `${contact.firstName || ''} ${contact.lastName || ''}`.trim() : 'Unknown';
    };

    const getSplitTypeName = (splitTypeId) => {
        const splitType = splitTypes.find(st => st.id === splitTypeId);
        return splitType?.name || '';
    };

    const renderSplitInputs = () => {
        const splitType = splitTypes.find(st => st.id === formik.values.splitTypeId);
        if (!splitType || selectedContacts.length === 0) return null;

        const splitTypeName = splitType.name?.toLowerCase();

        return (
            <Box sx={{ mt: 2 }}>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                    Split Details
                </Typography>
                {selectedContacts.map(contactId => (
                    <Box key={contactId} sx={{ mb: 2 }}>
                        <Grid container spacing={2} alignItems="center">
                            <Grid item xs={12} sm={4}>
                                <Typography variant="body2">
                                    {getContactName(contactId)}
                                </Typography>
                            </Grid>
                            <Grid item xs={12} sm={8}>
                                {splitTypeName === 'percentage' && (
                                    <TextField
                                        fullWidth
                                        size="small"
                                        type="number"
                                        label="Percentage"
                                        value={splitValues[contactId] || ''}
                                        onChange={(e) => handleSplitValueChange(contactId, e.target.value)}
                                        inputProps={{ min: 0, max: 100, step: 0.01 }}
                                        InputProps={{
                                            endAdornment: <Typography variant="body2">%</Typography>
                                        }}
                                    />
                                )}
                                {splitTypeName === 'exactamount' && (
                                    <TextField
                                        fullWidth
                                        size="small"
                                        type="number"
                                        label="Amount"
                                        value={splitValues[contactId] || ''}
                                        onChange={(e) => handleSplitValueChange(contactId, e.target.value)}
                                        inputProps={{ min: 0, step: 0.01 }}
                                    />
                                )}
                                {splitTypeName === 'shares' && (
                                    <TextField
                                        fullWidth
                                        size="small"
                                        type="number"
                                        label="Shares"
                                        value={splitValues[contactId] || ''}
                                        onChange={(e) => handleSplitValueChange(contactId, e.target.value)}
                                        inputProps={{ min: 0, step: 0.01 }}
                                    />
                                )}
                                {(splitTypeName === 'unequal' || splitTypeName === 'adjustment') && (
                                    <TextField
                                        fullWidth
                                        size="small"
                                        type="number"
                                        label="Amount"
                                        value={splitValues[contactId] || ''}
                                        onChange={(e) => handleSplitValueChange(contactId, e.target.value)}
                                        inputProps={{ min: 0, step: 0.01 }}
                                    />
                                )}
                            </Grid>
                        </Grid>
                    </Box>
                ))}
            </Box>
        );
    };

    return (
        <Box sx={{ p: { xs: 2, sm: 3 }, maxWidth: 800, mx: 'auto' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h5">Add Expense</Typography>
                <IconButton onClick={() => navigate(-1)}>
                    <CloseIcon />
                </IconButton>
            </Box>

            <form onSubmit={formik.handleSubmit}>
                <Card>
                    <CardContent>
                        <Grid container spacing={3}>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Description"
                                    name="description"
                                    value={formik.values.description}
                                    onChange={formik.handleChange}
                                    error={formik.touched.description && Boolean(formik.errors.description)}
                                    helperText={formik.touched.description && formik.errors.description}
                                    required
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <TextField
                                    fullWidth
                                    label="Amount"
                                    name="amount"
                                    type="number"
                                    value={formik.values.amount}
                                    onChange={formik.handleChange}
                                    error={formik.touched.amount && Boolean(formik.errors.amount)}
                                    helperText={formik.touched.amount && formik.errors.amount}
                                    inputProps={{ min: 0, step: 0.01 }}
                                    required
                                    InputProps={{
                                        startAdornment: <AttachMoneyIcon sx={{ mr: 1, color: 'text.secondary' }} />
                                    }}
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <DatePicker
                                    label="Date"
                                    value={formik.values.expenseDate}
                                    onChange={(value) => formik.setFieldValue('expenseDate', value)}
                                    slotProps={{
                                        textField: {
                                            fullWidth: true,
                                            error: formik.touched.expenseDate && Boolean(formik.errors.expenseDate),
                                            helperText: formik.touched.expenseDate && formik.errors.expenseDate
                                        }
                                    }}
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <FormControl fullWidth>
                                    <InputLabel>Group (Optional)</InputLabel>
                                    <Select
                                        name="groupId"
                                        value={formik.values.groupId || ''}
                                        onChange={formik.handleChange}
                                        label="Group (Optional)"
                                    >
                                        <MenuItem value="">None</MenuItem>
                                        {groups.map(group => (
                                            <MenuItem key={group.id} value={group.id}>
                                                {group.name}
                                            </MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <FormControl fullWidth required>
                                    <InputLabel>Split Type</InputLabel>
                                    <Select
                                        name="splitTypeId"
                                        value={formik.values.splitTypeId}
                                        onChange={formik.handleChange}
                                        label="Split Type"
                                        error={formik.touched.splitTypeId && Boolean(formik.errors.splitTypeId)}
                                    >
                                        {splitTypes.map(splitType => (
                                            <MenuItem key={splitType.id} value={splitType.id}>
                                                {splitType.name}
                                            </MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                            </Grid>

                            <Grid item xs={12}>
                                <Typography variant="subtitle2" sx={{ mb: 1 }}>
                                    Select People to Split With
                                </Typography>
                                <Paper variant="outlined" sx={{ p: 2, maxHeight: 200, overflow: 'auto' }}>
                                    {contacts.length === 0 ? (
                                        <Typography variant="body2" color="text.secondary">
                                            No contacts available
                                        </Typography>
                                    ) : (
                                        <List dense>
                                            {contacts.map(contact => {
                                                const contactId = contact.contactId || contact.id || contact.userId;
                                                const isSelected = selectedContacts.includes(contactId);
                                                return (
                                                    <ListItem
                                                        key={contactId}
                                                        button
                                                        onClick={() => handleContactToggle(contactId)}
                                                        selected={isSelected}
                                                    >
                                                        <ListItemAvatar>
                                                            <Avatar>
                                                                <PersonIcon />
                                                            </Avatar>
                                                        </ListItemAvatar>
                                                        <ListItemText
                                                            primary={`${contact.firstName || ''} ${contact.lastName || ''}`.trim()}
                                                            secondary={contact.email}
                                                        />
                                                    </ListItem>
                                                );
                                            })}
                                        </List>
                                    )}
                                </Paper>
                                {selectedContacts.length > 0 && (
                                    <Box sx={{ mt: 1, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                        {selectedContacts.map(contactId => (
                                            <Chip
                                                key={contactId}
                                                label={getContactName(contactId)}
                                                onDelete={() => handleContactToggle(contactId)}
                                                color="primary"
                                            />
                                        ))}
                                    </Box>
                                )}
                            </Grid>

                            {formik.values.splitTypeId && selectedContacts.length > 0 && renderSplitInputs()}

                            <Grid item xs={12}>
                                <Divider sx={{ my: 2 }} />
                                <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
                                    <Button
                                        variant="outlined"
                                        onClick={() => navigate(-1)}
                                        disabled={loading}
                                    >
                                        Cancel
                                    </Button>
                                    <Button
                                        type="submit"
                                        variant="contained"
                                        disabled={loading}
                                        startIcon={loading ? <CircularProgress size={20} /> : null}
                                    >
                                        {loading ? 'Adding...' : 'Add Expense'}
                                    </Button>
                                </Box>
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>
            </form>
        </Box>
    );
};

export default AddExpense;

