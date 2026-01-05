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
        .required('Split type is required')
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
            const parsedGroupId = parseInt(groupId);
            formik.setFieldValue('groupId', parsedGroupId);
            fetchGroupMembers(parsedGroupId);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
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
                // Extract member data properly - members have addedUser property
                const memberList = response.members.map(m => ({
                    id: m.addedUser?.userId || m.userId,
                    firstName: m.addedUser?.firstName || m.firstName || '',
                    lastName: m.addedUser?.lastName || m.lastName || '',
                    email: m.addedUser?.email || m.email || '',
                    isGroupMember: true
                }));
                setSelectedContacts(memberList);
                // Initialize split values for group members
                const initialSplitValues = {};
                memberList.forEach(member => {
                    initialSplitValues[member.id] = 0;
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
                selectedContacts.forEach(contact => {
                    const contactId = contact?.id || contact?.userId || contact;
                    shares.push({
                        userId: typeof contactId === 'object' ? (contactId?.id || contactId?.userId) : contactId,
                        splitTypeId: splitTypeId,
                        amountOwed: equalAmount,
                        exactAmount: equalAmount
                    });
                });
                break;
            case 'percentage':
                selectedContacts.forEach(contact => {
                    const contactId = contact?.id || contact?.userId || contact;
                    const userId = typeof contactId === 'object' ? (contactId?.id || contactId?.userId) : contactId;
                    const percentage = splitValues[userId] || 0;
                    shares.push({
                        userId: userId,
                        splitTypeId: splitTypeId,
                        percentage: percentage,
                        amountOwed: (amount * percentage) / 100
                    });
                });
                break;
            case 'exactamount':
                selectedContacts.forEach(contact => {
                    const contactId = contact?.id || contact?.userId || contact;
                    const userId = typeof contactId === 'object' ? (contactId?.id || contactId?.userId) : contactId;
                    const exactAmount = splitValues[userId] || 0;
                    shares.push({
                        userId: userId,
                        splitTypeId: splitTypeId,
                        exactAmount: exactAmount,
                        amountOwed: exactAmount
                    });
                });
                break;
            case 'shares':
                const totalShares = Object.values(splitValues).reduce((sum, val) => sum + (val || 0), 0);
                selectedContacts.forEach(contact => {
                    const contactId = contact?.id || contact?.userId || contact;
                    const userId = typeof contactId === 'object' ? (contactId?.id || contactId?.userId) : contactId;
                    const sharesValue = splitValues[userId] || 0;
                    shares.push({
                        userId: userId,
                        splitTypeId: splitTypeId,
                        shares: sharesValue,
                        amountOwed: totalShares > 0 ? (amount * sharesValue) / totalShares : 0
                    });
                });
                break;
            default:
                // Unequal or custom
                selectedContacts.forEach(contact => {
                    const contactId = contact?.id || contact?.userId || contact;
                    const userId = typeof contactId === 'object' ? (contactId?.id || contactId?.userId) : contactId;
                    const exactAmount = splitValues[userId] || 0;
                    shares.push({
                        userId: userId,
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
                // Validate that at least one contact is selected
                if (!selectedContacts || selectedContacts.length === 0) {
                    toast.error('Please select at least one person to split with');
                    return;
                }

                setLoading(true);
                
                const shares = calculateShares(
                    values.splitTypeId,
                    parseFloat(values.amount),
                    selectedContacts
                );

                // Validate that shares were calculated
                if (!shares || shares.length === 0) {
                    toast.error('Failed to calculate expense shares. Please check your split type and selected contacts.');
                    setLoading(false);
                    return;
                }

                const expenseData = {
                    description: values.description,
                    amount: parseFloat(values.amount),
                    expenseDate: values.expenseDate.toISOString(),
                    paidByUserId: values.paidByUserId,
                    groupId: values.groupId || null,
                    splitTypeId: values.splitTypeId,
                    expenseShares: shares.map(share => ({
                        userId: share.userId,
                        splitTypeId: share.splitTypeId,
                        percentage: share.percentage || null,
                        shares: share.shares || null,
                        exactAmount: share.exactAmount || null,
                        adjustedAmount: share.adjustedAmount || null,
                        amountOwed: share.amountOwed || 0
                    }))
                };

                console.log('Submitting expense data:', JSON.stringify(expenseData, null, 2));
                const response = await apiService.post(EXPENSE_PATHS.CREATE, expenseData);
                console.log('Expense created successfully:', response);
                toast.success('Expense added successfully');
                
                if (groupId) {
                    navigate(`/groups/${groupId}`);
                } else {
                    navigate('/groups');
                }
            } catch (error) {
                console.error('Error creating expense:', error);
                const errorMessage = error.response?.data?.message || error.message || 'Failed to create expense';
                toast.error(errorMessage);
            } finally {
                setLoading(false);
            }
        }
    });

    const handleContactToggle = (contactId) => {
        const contact = contacts.find(c => (c.contactId || c.id || c.userId) === contactId);
        
        setSelectedContacts(prev => {
            const existingIndex = prev.findIndex(c => {
                const cId = c?.id || c?.userId || c;
                return cId === contactId;
            });
            
            if (existingIndex >= 0) {
                const newList = prev.filter((c, index) => {
                    const cId = c?.id || c?.userId || c;
                    return cId !== contactId;
                });
                const newSplitValues = { ...splitValues };
                delete newSplitValues[contactId];
                setSplitValues(newSplitValues);
                return newList;
            } else {
                // Only add if contact exists and is not already in the list
                if (contact && !prev.find(c => {
                    const cId = c?.id || c?.userId || c;
                    return cId === contactId;
                })) {
                    return [...prev, contact];
                }
                return prev;
            }
        });
    };

    const handleGroupChange = (e) => {
        const groupId = e.target.value;
        formik.setFieldValue('groupId', groupId);
        
        if (groupId) {
            // Fetch and set group members only
            fetchGroupMembers(groupId);
        } else {
            // Reset to all contacts
            setSelectedContacts([]);
            setSplitValues({});
        }
    };

    const handleSplitValueChange = (contactId, value) => {
        setSplitValues(prev => ({
            ...prev,
            [contactId]: parseFloat(value) || 0
        }));
    };

    const getContactName = (contactId) => {
        // Check in selectedContacts first (for group members)
        const selectedContact = Array.isArray(selectedContacts) && selectedContacts.find(c => (c.id || c.userId || c) === contactId);
        if (selectedContact && typeof selectedContact === 'object') {
            return `${selectedContact.firstName || ''} ${selectedContact.lastName || ''}`.trim();
        }
        
        // Check in contacts list
        const contact = contacts.find(c => (c.contactId || c.id || c.userId) === contactId);
        return contact ? `${contact.firstName || ''} ${contact.lastName || ''}`.trim() : 'Unknown';
    };

    const getDisplayContacts = () => {
        // If group is selected, show selected contacts (group members)
        if (formik.values.groupId && Array.isArray(selectedContacts) && selectedContacts.length > 0) {
            return selectedContacts;
        }
        // Otherwise show all contacts
        return contacts;
    };

    const getSplitTypeName = (splitTypeId) => {
        const splitType = splitTypes.find(st => st.id === splitTypeId);
        return splitType?.name || '';
    };

    const renderSplitInputs = () => {
        const splitType = splitTypes.find(st => st.id === formik.values.splitTypeId);
        if (!splitType || selectedContacts.length === 0) return null;

        const splitTypeName = splitType.name?.toLowerCase();
        const isGroup = formik.values.groupId;

        return (
            <Grid item xs={12}>
                <Typography variant="subtitle2" sx={{ mb: { xs: 1, sm: 1.5 }, fontWeight: 600 }}>
                    Split Details ({splitType.name})
                </Typography>
                <Paper variant="outlined" sx={{ p: { xs: 1, sm: 2 }, borderRadius: 1 }}>
                    <Grid container spacing={{ xs: 1, sm: 2 }}>
                        {selectedContacts.map((contact, index) => {
                            const contactId = contact?.id || contact?.userId || contact;
                            const userId = typeof contactId === 'object' ? (contactId?.id || contactId?.userId) : contactId;
                            const uniqueKey = `split-input-${userId || index}`;
                            const contactName = typeof contact === 'object' 
                                ? `${contact.firstName || ''} ${contact.lastName || ''}`.trim()
                                : getContactName(contactId);
                            
                            let label = '';
                            let helper = '';
                            let inputValue = splitValues[userId] || '';
                            let step = 0.01;

                            if (splitTypeName === 'percentage') {
                                label = 'Percentage (%)';
                                helper = 'Total should equal 100%';
                                step = 0.01;
                            } else if (splitTypeName === 'exactamount') {
                                label = 'Amount';
                                helper = 'Exact amount for this person';
                                step = 0.01;
                            } else if (splitTypeName === 'shares') {
                                label = 'Shares';
                                helper = 'Number of shares';
                                step = 0.01;
                            } else if (splitTypeName === 'equal') {
                                // Equal split - no input needed
                                return (
                                    <Grid item xs={12} key={uniqueKey}>
                                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', py: 1, px: 1, backgroundColor: 'action.hover', borderRadius: 1 }}>
                                            <Typography variant="body2" sx={{ fontSize: { xs: '0.85rem', sm: '0.95rem' } }}>
                                                {contactName}
                                            </Typography>
                                            <Typography variant="caption" sx={{ fontSize: { xs: '0.75rem', sm: '0.85rem' } }}>
                                                {(formik.values.amount / (selectedContacts.length + 1)).toFixed(2)}
                                            </Typography>
                                        </Box>
                                    </Grid>
                                );
                            } else {
                                label = 'Amount';
                                helper = 'Custom amount';
                                step = 0.01;
                            }

                            return (
                                <Grid item xs={12} sm={6} md={4} key={uniqueKey}>
                                    <Box sx={{ backgroundColor: 'background.paper', p: { xs: 1, sm: 1.5 }, borderRadius: 1, border: '1px solid', borderColor: 'divider' }}>
                                        <Typography variant="caption" sx={{ display: 'block', mb: 1, fontSize: { xs: '0.7rem', sm: '0.8rem' } }}>
                                            {contactName}
                                        </Typography>
                                        <TextField
                                            fullWidth
                                            size="small"
                                            type="number"
                                            label={label}
                                            value={inputValue}
                                            onChange={(e) => handleSplitValueChange(userId, e.target.value)}
                                            inputProps={{ min: 0, step: step }}
                                            sx={{
                                                '& .MuiOutlinedInput-input': { fontSize: { xs: '0.85rem', sm: '1rem' } },
                                                '& .MuiInputBase-sizeSmall': { p: { xs: '0.5rem', sm: '0.75rem' } }
                                            }}
                                        />
                                        <Typography variant="caption" sx={{ display: 'block', mt: 0.5, fontSize: { xs: '0.7rem', sm: '0.75rem' }, color: 'text.secondary' }}>
                                            {helper}
                                        </Typography>
                                    </Box>
                                </Grid>
                            );
                        })}
                    </Grid>
                </Paper>
            </Grid>
        );
    };

    return (
        <Box sx={{ p: { xs: 1.5, sm: 2, md: 3 }, maxWidth: { xs: '100%', md: 800 }, mx: 'auto' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: { xs: 2, sm: 3 }, gap: 1 }}>
                <Typography variant="h5" sx={{ fontSize: { xs: '1.25rem', sm: '1.5rem' } }}>
                    {formik.values.groupId ? 'Add Group Expense' : 'Add Personal Expense'}
                </Typography>
                <IconButton onClick={() => navigate(-1)} size="large">
                    <CloseIcon />
                </IconButton>
            </Box>

            <form onSubmit={formik.handleSubmit}>
                <Card>
                    <CardContent sx={{ p: { xs: 1.5, sm: 2, md: 3 } }}>
                        <Grid container spacing={{ xs: 1.5, sm: 2, md: 3 }}>
                            {/* Expense Type and Group Selection */}
                            <Grid item xs={12} sm={6}>
                                <FormControl fullWidth>
                                    <InputLabel>Expense Type</InputLabel>
                                    <Select
                                        value={formik.values.groupId ? 'group' : 'personal'}
                                        onChange={(e) => {
                                            if (e.target.value === 'group') {
                                                formik.setFieldValue('groupId', '');
                                            } else {
                                                formik.setFieldValue('groupId', null);
                                                setSelectedContacts([]);
                                                setSplitValues({});
                                            }
                                        }}
                                        label="Expense Type"
                                    >
                                        <MenuItem value="personal">Personal</MenuItem>
                                        <MenuItem value="group">Group</MenuItem>
                                    </Select>
                                </FormControl>
                            </Grid>

                            {formik.values.groupId !== null && (
                                <Grid item xs={12} sm={6}>
                                    <FormControl fullWidth>
                                        <InputLabel>Group</InputLabel>
                                        <Select
                                            name="groupId"
                                            value={formik.values.groupId || ''}
                                            onChange={handleGroupChange}
                                            label="Group"
                                        >
                                            <MenuItem value="">Select a group...</MenuItem>
                                            {groups.map(group => (
                                                <MenuItem key={group.id} value={group.id}>
                                                    {group.name}
                                                </MenuItem>
                                            ))}
                                        </Select>
                                    </FormControl>
                                </Grid>
                            )}

                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    label="Description"
                                    name="description"
                                    placeholder="e.g., Coffee, Dinner, Movie tickets"
                                    value={formik.values.description}
                                    onChange={formik.handleChange}
                                    onBlur={formik.handleBlur}
                                    error={formik.touched.description && Boolean(formik.errors.description)}
                                    helperText={formik.touched.description && formik.errors.description}
                                    required
                                    multiline
                                    rows={2}
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
                                    onBlur={formik.handleBlur}
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
                                <FormControl fullWidth required>
                                    <InputLabel>Paid By</InputLabel>
                                    <Select
                                        name="paidByUserId"
                                        value={formik.values.paidByUserId}
                                        onChange={formik.handleChange}
                                        label="Paid By"
                                        error={formik.touched.paidByUserId && Boolean(formik.errors.paidByUserId)}
                                    >
                                        <MenuItem value={user?.userId || user?.id}>{user?.name || 'You'}</MenuItem>
                                        {formik.values.groupId && Array.isArray(selectedContacts) && selectedContacts
                                            .filter(member => {
                                                const memberId = member?.id || member?.userId;
                                                return memberId && memberId !== (user?.userId || user?.id);
                                            })
                                            .map(member => {
                                                const memberId = member?.id || member?.userId;
                                                return (
                                                    <MenuItem key={`paid-by-${memberId}`} value={memberId}>
                                                        {`${member.firstName || ''} ${member.lastName || ''}`.trim()}
                                                    </MenuItem>
                                                );
                                            })}
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

                            {/* Members Selection - Only shown for group expenses */}
                            {formik.values.groupId && (
                                <Grid item xs={12}>
                                    <Typography variant="subtitle2" sx={{ mb: 1.5, fontWeight: 600 }}>
                                        Group Members ({selectedContacts.length})
                                    </Typography>
                                    <Paper variant="outlined" sx={{ p: { xs: 1, sm: 2 }, maxHeight: { xs: 250, sm: 300 }, overflow: 'auto', borderRadius: 1 }}>
                                        {selectedContacts.length === 0 ? (
                                            <Typography variant="body2" color="text.secondary" sx={{ py: 2, textAlign: 'center' }}>
                                                No members in this group
                                            </Typography>
                                        ) : (
                                            <List dense sx={{ p: 0 }}>
                                                {selectedContacts.map((member, index) => {
                                                    const memberId = member?.id || member?.userId;
                                                    const uniqueKey = `group-member-${memberId || index}`;
                                                    const memberName = `${member.firstName || ''} ${member.lastName || ''}`.trim();
                                                    return (
                                                        <Box key={uniqueKey}>
                                                            <ListItem
                                                                sx={{
                                                                    py: { xs: 1, sm: 1.5 },
                                                                    px: { xs: 0.5, sm: 1 },
                                                                    backgroundColor: 'action.hover',
                                                                    borderRadius: 1,
                                                                    mb: 0.5
                                                                }}
                                                            >
                                                                <ListItemAvatar>
                                                                    <Avatar sx={{ width: { xs: 32, sm: 40 }, height: { xs: 32, sm: 40 }, fontSize: { xs: '0.75rem', sm: '1rem' } }}>
                                                                        {memberName.charAt(0).toUpperCase()}
                                                                    </Avatar>
                                                                </ListItemAvatar>
                                                                <ListItemText
                                                                    primary={memberName}
                                                                    secondary={member.email}
                                                                    primaryTypographyProps={{ sx: { fontSize: { xs: '0.9rem', sm: '1rem' } } }}
                                                                    secondaryTypographyProps={{ sx: { fontSize: { xs: '0.75rem', sm: '0.875rem' } } }}
                                                                />
                                                            </ListItem>
                                                            {index < selectedContacts.length - 1 && <Divider sx={{ my: 0.5 }} />}
                                                        </Box>
                                                    );
                                                })}
                                            </List>
                                        )}
                                    </Paper>
                                </Grid>
                            )}

                            {/* Personal Expense - Show all contacts */}
                            {!formik.values.groupId && (
                                <Grid item xs={12}>
                                    <Typography variant="subtitle2" sx={{ mb: 1.5, fontWeight: 600 }}>
                                        Select People to Split With
                                    </Typography>
                                    <Paper variant="outlined" sx={{ p: { xs: 1, sm: 2 }, maxHeight: { xs: 250, sm: 300 }, overflow: 'auto', borderRadius: 1 }}>
                                        {contacts.length === 0 ? (
                                            <Typography variant="body2" color="text.secondary" sx={{ py: 2, textAlign: 'center' }}>
                                                No contacts available
                                            </Typography>
                                        ) : (
                                            <List dense sx={{ p: 0 }}>
                                                {contacts.map((contact, index) => {
                                                    const contactId = contact.contactId || contact.id || contact.userId;
                                                    const uniqueKey = `contact-${contactId || index}`;
                                                    const isSelected = selectedContacts.some(c => {
                                                        const cId = c?.id || c?.userId || c;
                                                        return cId === contactId;
                                                    });
                                                    const contactName = `${contact.firstName || ''} ${contact.lastName || ''}`.trim();
                                                    return (
                                                        <Box key={uniqueKey}>
                                                            <ListItem
                                                                button
                                                                onClick={() => handleContactToggle(contactId)}
                                                                selected={isSelected}
                                                                sx={{
                                                                    py: { xs: 1, sm: 1.5 },
                                                                    px: { xs: 0.5, sm: 1 },
                                                                    borderRadius: 1,
                                                                    mb: 0.5,
                                                                    backgroundColor: isSelected ? 'action.selected' : 'transparent',
                                                                    '&:hover': { backgroundColor: 'action.hover' }
                                                                }}
                                                            >
                                                                <ListItemAvatar>
                                                                    <Avatar sx={{ width: { xs: 32, sm: 40 }, height: { xs: 32, sm: 40 }, fontSize: { xs: '0.75rem', sm: '1rem' } }}>
                                                                        {contactName.charAt(0).toUpperCase()}
                                                                    </Avatar>
                                                                </ListItemAvatar>
                                                                <ListItemText
                                                                    primary={contactName}
                                                                    secondary={contact.email}
                                                                    primaryTypographyProps={{ sx: { fontSize: { xs: '0.9rem', sm: '1rem' } } }}
                                                                    secondaryTypographyProps={{ sx: { fontSize: { xs: '0.75rem', sm: '0.875rem' } } }}
                                                                />
                                                            </ListItem>
                                                            {index < contacts.length - 1 && <Divider sx={{ my: 0.5 }} />}
                                                        </Box>
                                                    );
                                                })}
                                            </List>
                                        )}
                                    </Paper>
                                    {selectedContacts.length > 0 && (
                                        <Box sx={{ mt: 1.5, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                            {selectedContacts.map((contact, index) => {
                                                const memberId = contact?.id || contact?.userId || contact;
                                                const uniqueKey = `selected-chip-${typeof memberId === 'object' ? (memberId?.id || memberId?.userId || index) : (memberId || index)}`;
                                                return (
                                                    <Chip
                                                        key={uniqueKey}
                                                        label={getContactName(memberId)}
                                                        onDelete={() => handleContactToggle(memberId)}
                                                        color="primary"
                                                        size="small"
                                                        sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}
                                                    />
                                                );
                                            })}
                                        </Box>
                                    )}
                                </Grid>
                            )}

                            {formik.values.splitTypeId && selectedContacts.length > 0 && renderSplitInputs()}

                            <Grid item xs={12}>
                                <Divider sx={{ my: { xs: 1, sm: 2 } }} />
                                <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 1.5, flexWrap: 'wrap' }}>
                                    <Button
                                        variant="outlined"
                                        onClick={() => navigate(-1)}
                                        disabled={loading}
                                        sx={{ fontSize: { xs: '0.8rem', sm: '1rem' }, py: { xs: 0.75, sm: 1 } }}
                                    >
                                        Cancel
                                    </Button>
                                    <Button
                                        type="submit"
                                        variant="contained"
                                        disabled={loading || !formik.isValid}
                                        onClick={() => {
                                            console.log('Submit button clicked');
                                            console.log('Form values:', formik.values);
                                            console.log('Form errors:', formik.errors);
                                            console.log('Form touched:', formik.touched);
                                            console.log('Form isValid:', formik.isValid);
                                            console.log('Selected contacts:', selectedContacts);
                                        }}
                                        startIcon={loading ? <CircularProgress size={20} /> : null}
                                        sx={{ fontSize: { xs: '0.8rem', sm: '1rem' }, py: { xs: 0.75, sm: 1 }, minWidth: { xs: 120, sm: 150 } }}
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

