import React, { useState } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    Box,
    IconButton,
    Typography,
    useMediaQuery,
    useTheme,
    CircularProgress,
    List,
    ListItem,
    ListItemText,
    ListItemAvatar,
    Avatar,
    Divider,
    InputAdornment
} from '@mui/material';
import {
    Close as CloseIcon,
    Search as SearchIcon,
    PersonAdd as PersonAddIcon,
    ArrowBack as ArrowBackIcon
} from '@mui/icons-material';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { apiService } from '../utils/axios';
import { CONTACT_PATHS } from '../constants/apiPaths';
import { toast } from 'react-toastify';

const validationSchema = Yup.object().shape({
    searchTerm: Yup.string().required('Required'),
    isExistingUser: Yup.boolean(),
    userId: Yup.string(),
    firstName: Yup.string().when('isExistingUser', {
        is: false,
        then: () => Yup.string().required('First name is required')
    }),
    lastName: Yup.string(),
    email: Yup.string().email('Invalid email'),
    phone: Yup.string().matches(/^[0-9]*$/, 'Must be only digits')
}).test('emailOrPhone', 'Either email or phone is required', function (value) {
    if (value.isExistingUser) return true;
    return Boolean(value.email) || Boolean(value.phone);
});

const AddContactDialog = ({ open, onClose }) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
    const [searchResults, setSearchResults] = useState([]);
    const [loading, setLoading] = useState(false);
    const [showNewContactForm, setShowNewContactForm] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null);

    const formik = useFormik({
        initialValues: {
            searchTerm: '',
            isExistingUser: false,
            userId: '',
            firstName: '',
            lastName: '',
            email: '',
            phone: ''
        },
        validationSchema,
        onSubmit: async (values) => {
            try {
                setLoading(true);
                if (values.isExistingUser) {
                    await apiService.post(`${CONTACT_PATHS.ADD_IN_CONTACT_LIST}${values.userId}`);
                    toast.success('Contact added successfully');
                } else {
                    const createData = {
                        firstName: values.firstName,
                        lastName: values.lastName,
                        email: values.email,
                        phone: values.phone
                    };
                    await apiService.post(CONTACT_PATHS.CREATE, createData);
                    toast.success('Contact created and added successfully');
                }
                handleClose();
            } catch (error) {
                console.error('Error saving contact:', error);
                toast.error(error.response?.data?.message || 'Failed to save contact');
            } finally {
                setLoading(false);
            }
        }
    });

    const handleSearch = async () => {
        if (!formik.values.searchTerm) return;

        try {
            setLoading(true);
            const response = await apiService.get(CONTACT_PATHS.SEARCH_USER, { searchTerm: formik.values.searchTerm }
            );
            setSearchResults(response || []);
            setSelectedUser(null);
        } catch (error) {
            console.error('Error searching users:', error);
            toast.error('Error searching for users');
        } finally {
            setLoading(false);
        }
    };

    const handleUserSelect = (user) => {
        setSelectedUser(user);
        formik.setValues({
            ...formik.values,
            isExistingUser: true,
            userId: user.userId,
            firstName: user.firstName,
            lastName: user.lastName,
            email: user.email,
            phone: user.phone
        });
    };

    const handleShowNewContactForm = () => {
        setShowNewContactForm(true);
        setSearchResults([]);
        setSelectedUser(null);
        formik.setValues({
            ...formik.values,
            isExistingUser: false,
            userId: '',
            firstName: '',
            lastName: '',
            email: '',
            phone: ''
        });
    };

    const handleBack = () => {
        setShowNewContactForm(false);
        setSelectedUser(null);
        formik.resetForm();
    };

    const handleClose = () => {
        formik.resetForm();
        setSearchResults([]);
        setSelectedUser(null);
        setShowNewContactForm(false);
        onClose();
    };

    return (
        <Dialog
            fullScreen={fullScreen}
            open={open}
            onClose={handleClose}
            maxWidth="sm"
            fullWidth
        >
            <DialogTitle>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    {showNewContactForm ? (
                        <IconButton onClick={handleBack} edge="start" sx={{ mr: 1 }}>
                            <ArrowBackIcon />
                        </IconButton>
                    ) : null}
                    <Typography variant="h6">
                        {showNewContactForm ? 'Create New Contact' : 'Add Contact'}
                    </Typography>
                    <IconButton onClick={handleClose} size="small">
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>

            <form onSubmit={formik.handleSubmit}>
                <DialogContent>
                    {!showNewContactForm && (
                        <>
                            <TextField
                                fullWidth
                                name="searchTerm"
                                placeholder="Search by email or phone"
                                value={formik.values.searchTerm}
                                onChange={formik.handleChange}
                                error={formik.touched.searchTerm && Boolean(formik.errors.searchTerm)}
                                helperText={formik.touched.searchTerm && formik.errors.searchTerm}
                                InputProps={{
                                    endAdornment: (
                                        <InputAdornment position="end">
                                            <IconButton
                                                onClick={handleSearch}
                                                disabled={!formik.values.searchTerm || loading}
                                            >
                                                {loading ? (
                                                    <CircularProgress size={24} />
                                                ) : (
                                                    <SearchIcon />
                                                )}
                                            </IconButton>
                                        </InputAdornment>
                                    ),
                                }}
                                sx={{ mb: 2 }}
                            />

                            {searchResults.length > 0 && (
                                <List sx={{ width: '100%', bgcolor: 'background.paper' }}>
                                    {searchResults.map((user, index) => (
                                        <React.Fragment key={user.userId}>
                                            <ListItem
                                                button
                                                selected={selectedUser?.userId === user.userId}
                                                onClick={() => handleUserSelect(user)}
                                                sx={{
                                                    borderRadius: 1,
                                                    mb: 0.5,
                                                    '&.Mui-selected': {
                                                        bgcolor: 'primary.light',
                                                        '&:hover': {
                                                            bgcolor: 'primary.light',
                                                        },
                                                        '& .MuiListItemText-primary, & .MuiListItemText-secondary': {
                                                            color: 'white',
                                                        },
                                                    },
                                                    '&:hover': {
                                                        bgcolor: 'action.hover',
                                                    },
                                                }}
                                            >
                                                <ListItemAvatar>
                                                    <Avatar
                                                        sx={{
                                                            bgcolor: selectedUser?.userId === user.userId ? 
                                                                'primary.dark' : 'primary.main'
                                                        }}
                                                    >
                                                        {user.firstName[0]}
                                                    </Avatar>
                                                </ListItemAvatar>
                                                <ListItemText
                                                    primary={`${user.firstName} ${user.lastName || ''}`}
                                                    secondary={
                                                        <>
                                                            <Typography
                                                                component="span"
                                                                variant="body2"
                                                                sx={{
                                                                    display: 'block',
                                                                    color: selectedUser?.userId === user.userId ? 
                                                                        'white' : 'text.secondary'
                                                                }}
                                                            >
                                                                {user.email}
                                                            </Typography>
                                                            {user.phone && (
                                                                <Typography
                                                                    component="span"
                                                                    variant="body2"
                                                                    sx={{
                                                                        color: selectedUser?.userId === user.userId ? 
                                                                            'white' : 'text.secondary'
                                                                    }}
                                                                >
                                                                    {user.phone}
                                                                </Typography>
                                                            )}
                                                        </>
                                                    }
                                                />
                                            </ListItem>
                                            {index < searchResults.length - 1 && (
                                                <Divider variant="inset" component="li" />
                                            )}
                                        </React.Fragment>
                                    ))}
                                </List>
                            )}

                            {!loading && searchResults.length === 0 && formik.values.searchTerm && (
                                <Box sx={{ textAlign: 'center', py: 3 }}>
                                    <Typography color="textSecondary" gutterBottom>
                                        No users found
                                    </Typography>
                                    <Button
                                        variant="outlined"
                                        startIcon={<PersonAddIcon />}
                                        onClick={handleShowNewContactForm}
                                        sx={{ mt: 1 }}
                                    >
                                        Create New Contact
                                    </Button>
                                </Box>
                            )}
                        </>
                    )}

                    {showNewContactForm && (
                        <Box sx={{ mt: 2 }}>
                            <TextField
                                fullWidth
                                name="firstName"
                                label="First Name"
                                value={formik.values.firstName}
                                onChange={formik.handleChange}
                                error={formik.touched.firstName && Boolean(formik.errors.firstName)}
                                helperText={formik.touched.firstName && formik.errors.firstName}
                                sx={{ mb: 2 }}
                            />
                            <TextField
                                fullWidth
                                name="lastName"
                                label="Last Name (Optional)"
                                value={formik.values.lastName}
                                onChange={formik.handleChange}
                                sx={{ mb: 2 }}
                            />
                            <TextField
                                fullWidth
                                name="email"
                                label="Email"
                                value={formik.values.email}
                                onChange={formik.handleChange}
                                error={formik.touched.email && Boolean(formik.errors.email)}
                                helperText={formik.touched.email && formik.errors.email}
                                sx={{ mb: 2 }}
                            />
                            <TextField
                                fullWidth
                                name="phone"
                                label="Phone Number"
                                value={formik.values.phone}
                                onChange={formik.handleChange}
                                error={formik.touched.phone && Boolean(formik.errors.phone)}
                                helperText={formik.touched.phone && formik.errors.phone}
                            />
                        </Box>
                    )}
                </DialogContent>

                <DialogActions sx={{ p: 2 }}>
                    <Button onClick={handleClose}>Cancel</Button>
                    {(selectedUser || showNewContactForm) && (
                        <Button
                            type="submit"
                            variant="contained"
                            disabled={loading || !formik.isValid}
                            startIcon={loading && <CircularProgress size={20} />}
                        >
                            {loading ? 'Saving...' : selectedUser ? 'Add Contact' : 'Create Contact'}
                        </Button>
                    )}
                </DialogActions>
            </form>
        </Dialog>
    );
};

export default AddContactDialog; 