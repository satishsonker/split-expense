import { useState } from 'react';
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
    RadioGroup,
    Radio,
    FormControlLabel
} from '@mui/material';
import { Close as CloseIcon } from '@mui/icons-material';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { apiService } from '../utils/axios';
import { CONTACT_PATHS } from '../constants/apiPaths';
import { toast } from 'react-toastify';

const validationSchema = Yup.object().shape({
    searchType: Yup.string().required('Required'),
    searchValue: Yup.string().required('Required'),
    firstName: Yup.string().when('existingUser', {
        is: false,
        then: () => Yup.string().required('First name is required'),
        otherwise: () => Yup.string()
    }),
    lastName: Yup.string(),
    email: Yup.string().when('searchType', {
        is: 'email',
        then: () => Yup.string().email('Invalid email'),
        otherwise: () => Yup.string()
    }),
    phoneNumber: Yup.string().when('searchType', {
        is: 'phoneNumber',
        then: () => Yup.string().matches(/^[0-9]+$/, 'Must be only digits'),
        otherwise: () => Yup.string()
    }),
    existingUser: Yup.boolean(),
    userId: Yup.string()
});

const AddContactDialog = ({ open, onClose, onSubmit }) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
    const [searchResult, setSearchResult] = useState(null);
    const [searching, setSearching] = useState(false);

    const formik = useFormik({
        initialValues: {
            searchType: 'email',
            searchValue: '',
            existingUser: false,
            userId: '',
            firstName: '',
            lastName: '',
            email: '',
            phoneNumber: ''
        },
        validationSchema,
        onSubmit: async (values) => {
            await onSubmit(values);
            formik.resetForm();
            setSearchResult(null);
        }
    });

    const  handleSearch = async () => {
        if (!formik.values.searchValue) return;

        try {
            setSearching(true);
            const response = await apiService.get(CONTACT_PATHS.SEARCH_USER, {
                "searchTerm": formik.values.searchValue
            });
            
            if (response.data) {
                setSearchResult(response.data);
                formik.setValues({
                    ...formik.values,
                    existingUser: true,
                    userId: response.data.id,
                    firstName: response.data.firstName,
                    lastName: response.data.lastName,
                    email: response.data.email,
                    phoneNumber: response.data.phoneNumber
                });
            } else {
                setSearchResult(null);
                formik.setValues({
                    ...formik.values,
                    existingUser: false,
                    userId: '',
                    firstName: '',
                    lastName: '',
                    [formik.values.searchType]: formik.values.searchValue
                });
            }
        } catch (error) {
            console.error('Error searching user:', error);
            toast.error('Error searching for user');
            setSearchResult(null);
        } finally {
            setSearching(false);
        }
    };

    const handleClose = () => {
        formik.resetForm();
        setSearchResult(null);
        onClose();
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            fullScreen={fullScreen}
            maxWidth="sm"
            fullWidth
        >
            <DialogTitle>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Typography variant="h6">Add Contact</Typography>
                    <IconButton onClick={handleClose} size="small">
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>

            <form onSubmit={formik.handleSubmit}>
                <DialogContent>
                    <RadioGroup
                        row
                        name="searchType"
                        value={formik.values.searchType}
                        onChange={formik.handleChange}
                    >
                        <FormControlLabel value="email" control={<Radio />} label="Email" />
                        <FormControlLabel value="phoneNumber" control={<Radio />} label="Phone Number" />
                    </RadioGroup>

                    <Box sx={{ display: 'flex', gap: 1, mb: 3 }}>
                        <TextField
                            fullWidth
                            name="searchValue"
                            label={formik.values.searchType === 'email' ? 'Email' : 'Phone Number'}
                            value={formik.values.searchValue}
                            onChange={formik.handleChange}
                            error={formik.touched.searchValue && Boolean(formik.errors.searchValue)}
                            helperText={formik.touched.searchValue && formik.errors.searchValue}
                        />
                        <Button
                            variant="contained"
                            onClick={handleSearch}
                            disabled={!formik.values.searchValue || searching}
                        >
                            {searching ? 'Searching...' : 'Search'}
                        </Button>
                    </Box>

                    {!searchResult && !formik.values.existingUser && (
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
                            {formik.values.searchType === 'email' && (
                                <TextField
                                    fullWidth
                                    name="phoneNumber"
                                    label="Phone Number (Optional)"
                                    value={formik.values.phoneNumber}
                                    onChange={formik.handleChange}
                                    sx={{ mb: 2 }}
                                />
                            )}
                            {formik.values.searchType === 'phoneNumber' && (
                                <TextField
                                    fullWidth
                                    name="email"
                                    label="Email (Optional)"
                                    value={formik.values.email}
                                    onChange={formik.handleChange}
                                    error={formik.touched.email && Boolean(formik.errors.email)}
                                    helperText={formik.touched.email && formik.errors.email}
                                    sx={{ mb: 2 }}
                                />
                            )}
                        </Box>
                    )}

                    {searchResult && (
                        <Box sx={{ mt: 2, p: 2, bgcolor: 'background.default', borderRadius: 1 }}>
                            <Typography variant="subtitle1">Existing User Found:</Typography>
                            <Typography>
                                {`${searchResult.firstName} ${searchResult.lastName || ''}`}
                            </Typography>
                            <Typography color="textSecondary">
                                {searchResult.email}
                            </Typography>
                            {searchResult.phoneNumber && (
                                <Typography color="textSecondary">
                                    {searchResult.phoneNumber}
                                </Typography>
                            )}
                        </Box>
                    )}
                </DialogContent>

                <DialogActions sx={{ p: 2 }}>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button
                        type="submit"
                        variant="contained"
                        disabled={formik.isSubmitting || !formik.isValid}
                    >
                        Add Contact
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );
};

export default AddContactDialog; 