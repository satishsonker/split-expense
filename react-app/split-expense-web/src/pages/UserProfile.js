import React, { useState, useEffect } from 'react';
import {
    Box,
    Card,
    CardContent,
    Grid,
    Typography,
    Avatar,
    IconButton,
    Button,
    TextField,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    useTheme,
    useMediaQuery
} from '@mui/material';
import {
    Edit as EditIcon,
    Delete as DeleteIcon,
    PhotoCamera as CameraIcon,
    Upload as UploadIcon
} from '@mui/icons-material';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { toast } from 'react-toastify';
import { getImageUrl } from '../utils/imageUtils';
import { useAuth } from '../context/AuthContext';
import DeleteConfirmationDialog from '../components/DeleteConfirmationDialog';
import ImageSourceDialog from '../components/ImageSourceDialog';

const validationSchema = Yup.object({
    firstName: Yup.string().required('First name is required'),
    lastName: Yup.string(),
    email: Yup.string().email('Invalid email address').required('Email is required'),
    phoneNumber: Yup.string().matches(/^\+?[1-9]\d{1,14}$/, 'Invalid phone number'),
    timezone: Yup.string().required('Timezone is required'),
    currency: Yup.string().required('Currency is required')
});

const timezones = [
    'UTC',
    'America/New_York',
    'Europe/London',
    'Asia/Tokyo',
    // Add more timezones as needed
];

const currencies = [
    'USD',
    'EUR',
    'GBP',
    'JPY',
    'INR',
    // Add more currencies as needed
];

const UserProfile = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { user, logout } = useAuth();
    const [loading, setLoading] = useState(false);
    const [imageSourceDialogOpen, setImageSourceDialogOpen] = useState(false);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [imagePreview, setImagePreview] = useState(null);

    const formik = useFormik({
        initialValues: {
            firstName: '',
            lastName: '',
            email: '',
            phoneNumber: '',
            timezone: 'UTC',
            currency: 'USD',
            image: null
        },
        validationSchema,
        onSubmit: async (values) => {
            try {
                setLoading(true);
                const formData = new FormData();
                Object.keys(values).forEach(key => {
                    if (values[key] !== null && key !== 'image') {
                        formData.append(key, values[key]);
                    }
                });

                if (values.image) {
                    formData.append('image', values.image);
                }

                // Call your API to update profile
                // await apiService.put('/user/profile', formData);

                toast.success('Profile updated successfully');
            } catch (error) {
                console.error('Error updating profile:', error);
                toast.error('Failed to update profile');
            } finally {
                setLoading(false);
            }
        }
    });

    useEffect(() => {
        debugger;
        const userData = JSON.parse(localStorage.getItem('user'));
        if (userData) {
            formik.setValues({
                firstName: userData.firstName || '',
                lastName: userData.lastName || '',
                email: userData.email || '',
                phoneNumber: userData.phone || '',
                timezone: userData.timezone || 'UTC',
                currency: userData.currency || 'USD',
                image: null
            });
            debugger;
            if (userData.profilePicture || userData.thumbProfilePicture) {
                setImagePreview(getImageUrl(userData.thumbProfilePicture ?? userData.profilePicture));
            }
        }
    }, []);

    const handleImageSourceSelect = (source) => {
        setImageSourceDialogOpen(false);
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        if (source === 'camera') {
            input.capture = 'environment';
        }
        input.onchange = (e) => {
            const file = e.target.files[0];
            if (file) {
                formik.setFieldValue('image', file);
                const reader = new FileReader();
                reader.onloadend = () => {
                    setImagePreview(reader.result);
                };
                reader.readAsDataURL(file);
            }
        };
        input.click();
    };

    const handleDeleteAccount = async () => {
        try {
            setLoading(true);
            // Call your API to delete account
            // await apiService.delete('/user/account');

            toast.success('Account deleted successfully');
            logout();
        } catch (error) {
            console.error('Error deleting account:', error);
            toast.error('Failed to delete account');
        } finally {
            setLoading(false);
            setDeleteDialogOpen(false);
        }
    };

    return (
        <Box sx={{
            p: { xs: 2, sm: 3 },
            maxWidth: 'md',
            mx: 'auto'
        }}>
            <Typography
                variant={isMobile ? "h5" : "h4"}
                gutterBottom
                sx={{ mb: 3 }}
            >
                Profile Settings
            </Typography>

            <form onSubmit={formik.handleSubmit}>
                <Grid container spacing={3}>
                    {/* Profile Image */}
                    <Grid item xs={12} sx={{ display: 'flex', justifyContent: 'center' }}>
                        <Box sx={{ position: 'relative' }}>
                            <Avatar
                                src={imagePreview}
                                sx={{
                                    width: { xs: 120, sm: 150 },
                                    height: { xs: 120, sm: 150 },
                                    cursor: 'pointer',
                                    '&:hover': {
                                        opacity: 0.8
                                    }
                                }}
                                onClick={() => setImageSourceDialogOpen(true)}
                            >
                                {!imagePreview && user?.firstName?.[0]}
                            </Avatar>
                            <IconButton
                                sx={{
                                    position: 'absolute',
                                    bottom: 0,
                                    right: 0,
                                    bgcolor: 'background.paper',
                                    boxShadow: 1,
                                    '&:hover': {
                                        bgcolor: 'background.paper'
                                    }
                                }}
                                onClick={() => setImageSourceDialogOpen(true)}
                            >
                                <EditIcon />
                            </IconButton>
                        </Box>
                    </Grid>

                    {/* User Info Card */}
                    <Grid item xs={12}>
                        <Card elevation={isMobile ? 0 : 1}>
                            <CardContent>
                                <Grid container spacing={2}>
                                    <Grid item xs={12} sm={6}>
                                        <TextField
                                            fullWidth
                                            label="First Name"
                                            name="firstName"
                                            value={formik.values.firstName}
                                            onChange={formik.handleChange}
                                            error={formik.touched.firstName && Boolean(formik.errors.firstName)}
                                            helperText={formik.touched.firstName && formik.errors.firstName}
                                        />
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <TextField
                                            fullWidth
                                            label="Last Name"
                                            name="lastName"
                                            value={formik.values.lastName}
                                            onChange={formik.handleChange}
                                            error={formik.touched.lastName && Boolean(formik.errors.lastName)}
                                            helperText={formik.touched.lastName && formik.errors.lastName}
                                        />
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <TextField
                                            fullWidth
                                            label="Email"
                                            name="email"
                                            type="email"
                                            value={formik.values.email}
                                            onChange={formik.handleChange}
                                            error={formik.touched.email && Boolean(formik.errors.email)}
                                            helperText={formik.touched.email && formik.errors.email}
                                        />
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <TextField
                                            fullWidth
                                            label="Phone Number"
                                            name="phoneNumber"
                                            value={formik.values.phoneNumber}
                                            onChange={formik.handleChange}
                                            error={formik.touched.phoneNumber && Boolean(formik.errors.phoneNumber)}
                                            helperText={formik.touched.phoneNumber && formik.errors.phoneNumber}
                                        />
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <FormControl fullWidth>
                                            <InputLabel>Timezone</InputLabel>
                                            <Select
                                                name="timezone"
                                                value={formik.values.timezone}
                                                onChange={formik.handleChange}
                                                label="Timezone"
                                            >
                                                {timezones.map(tz => (
                                                    <MenuItem key={tz} value={tz}>{tz}</MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <FormControl fullWidth>
                                            <InputLabel>Currency</InputLabel>
                                            <Select
                                                name="currency"
                                                value={formik.values.currency}
                                                onChange={formik.handleChange}
                                                label="Currency"
                                            >
                                                {currencies.map(currency => (
                                                    <MenuItem key={currency} value={currency}>{currency}</MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>
                                    </Grid>
                                </Grid>
                            </CardContent>
                        </Card>
                    </Grid>

                    {/* Action Buttons */}
                    <Grid item xs={12} sx={{
                        display: 'flex',
                        flexDirection: { xs: 'column', sm: 'row' },
                        justifyContent: 'space-between',
                        gap: 2
                    }}>
                        <Button
                            fullWidth={isMobile}
                            variant="contained"
                            color="error"
                            startIcon={<DeleteIcon />}
                            onClick={() => setDeleteDialogOpen(true)}
                        >
                            Delete Account
                        </Button>
                        <Button
                            fullWidth={isMobile}
                            type="submit"
                            variant="contained"
                            disabled={loading || !formik.isValid}
                        >
                            {loading ? 'Saving...' : 'Save Changes'}
                        </Button>
                    </Grid>
                </Grid>
            </form>

            {/* Dialogs */}
            <ImageSourceDialog
                open={imageSourceDialogOpen}
                onClose={() => setImageSourceDialogOpen(false)}
                onSelect={handleImageSourceSelect}
            />

            <DeleteConfirmationDialog
                open={deleteDialogOpen}
                onClose={() => setDeleteDialogOpen(false)}
                onConfirm={handleDeleteAccount}
                loading={loading}
                title="Delete Account"
                type="account"
                data={user}
                warningMessage="This will permanently delete your account and all associated data. This action cannot be undone."
            />
        </Box>
    );
};

export default UserProfile; 