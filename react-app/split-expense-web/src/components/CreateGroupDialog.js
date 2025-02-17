import React, { useState, useEffect, useRef } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    Box,
    Chip,
    Avatar,
    IconButton,
    Typography,
    useMediaQuery,
    useTheme,
    Autocomplete,
    InputAdornment,
    Switch,
    FormControlLabel,
    Grid,
    Paper,
    CircularProgress
} from '@mui/material';
import {
    Close as CloseIcon,
    Add as AddIcon,
    Upload as UploadIcon,
    Delete as DeleteIcon,
    PhotoCamera as CameraIcon,
    Image as GalleryIcon
} from '@mui/icons-material';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { getGroupIcon } from '../utils/groupIcons';
import { GROUP_PATHS, CONTACT_PATHS, GROUP_TYPES_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { toast } from 'react-toastify';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import dayjs from 'dayjs';

const validationSchema = Yup.object({
    name: Yup.string()
        .required('Group name is required')
        .max(100, 'Group name must be at most 100 characters'),
    members: Yup.array(),
    groupTypeId: Yup.number().nullable(),
    groupDetail: Yup.object({
        enableGroupDate: Yup.boolean(),
        enableSettleUpReminders: Yup.boolean(),
        enableBalanceAlert: Yup.boolean(),
        maxGroupBudget: Yup.number().nullable(),
        startDate: Yup.date().nullable(),
        endDate: Yup.date().nullable().when('startDate', (startDate, schema) => {
            return startDate && startDate[0]!=null ? schema.min(startDate, 'End date must be after start date') : schema;
        })
    })
});

const ImageSourceDialog = ({ open, onClose, onSelect }) => {
    return (
        <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
            <DialogTitle>
                <Typography variant="h6">Select Image Source</Typography>
            </DialogTitle>
            <DialogContent dividers>
                <Grid container spacing={2}>
                    <Grid item xs={6}>
                        <Button
                            fullWidth
                            variant="outlined"
                            onClick={() => onSelect('camera')}
                            sx={{ 
                                p: 3, 
                                display: 'flex', 
                                flexDirection: 'column',
                                gap: 1
                            }}
                        >
                            <CameraIcon sx={{ fontSize: 40 }} />
                            <Typography>Camera</Typography>
                        </Button>
                    </Grid>
                    <Grid item xs={6}>
                        <Button
                            fullWidth
                            variant="outlined"
                            onClick={() => onSelect('gallery')}
                            sx={{ 
                                p: 3, 
                                display: 'flex', 
                                flexDirection: 'column',
                                gap: 1
                            }}
                        >
                            <GalleryIcon sx={{ fontSize: 40 }} />
                            <Typography>Gallery</Typography>
                        </Button>
                    </Grid>
                </Grid>
            </DialogContent>
        </Dialog>
    );
};

const CreateGroupDialog = ({ open, onClose, onSubmit }) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
    const [groupTypes, setGroupTypes] = useState([]);
    const [selectedGroupType, setSelectedGroupType] = useState(null);
    const [imagePreview, setImagePreview] = useState(null);
    const [loading, setLoading] = useState(false);
    const [imageSourceDialogOpen, setImageSourceDialogOpen] = useState(false);
    const [contacts, setContacts] = useState([]);
    const [loadingContacts, setLoadingContacts] = useState(false);
    const cameraInputRef = useRef(null);
    const galleryInputRef = useRef(null);

    useEffect(() => {
        const fetchContacts = async () => {
            if (open) {
                try {
                    setLoadingContacts(true);
                    const response = await apiService.get(CONTACT_PATHS.LIST);
                    setContacts(response.data || []);
                } catch (error) {
                    console.error('Error fetching contacts:', error);
                    toast.error('Failed to load contacts');
                } finally {
                    setLoadingContacts(false);
                }
            }
        };

        const fetchGroupTypes = async () => {
            if (open) {
                try {
                    const response = await apiService.get(GROUP_TYPES_PATHS.LIST);
                    setGroupTypes(response.data || []);
                } catch (error) {
                    console.error('Error fetching group types:', error);
                    toast.error('Failed to fetch group types');
                }
            }
        };

        if (open) {
            fetchContacts();
            fetchGroupTypes();
        }
    }, [open]);

    const handleImageSourceSelect = (source) => {
        setImageSourceDialogOpen(false);
        if (source === 'camera') {
            cameraInputRef.current?.click();
        } else {
            galleryInputRef.current?.click();
        }
    };

    const handleImageCapture = (event) => {
        const file = event.target.files?.[0];
        if (file) {
            formik.setFieldValue('image', file);
            const reader = new FileReader();
            reader.onloadend = () => {
                setImagePreview(reader.result);
            };
            reader.readAsDataURL(file);
        }
        event.target.value = '';
    };

    const handleRemoveImage = () => {
        setImagePreview(null);
        formik.setFieldValue('image', null);
    };

    const handleGroupTypeSelect = (type) => {
        setSelectedGroupType(type);
        formik.setFieldValue('groupTypeId', type.id);
    };

    const formik = useFormik({
        initialValues: {
            name: '',
            members: [],
            groupTypeId: null,
            image: null,
            icon: '',
            groupDetail: {
                enableGroupDate: false,
                enableSettleUpReminders: false,
                enableBalanceAlert: false,
                maxGroupBudget: null,
                startDate: null,
                endDate: null
            }
        },
        validationSchema,
        validateOnMount: false,
        validateOnChange: true,
        onSubmit: async (values) => {
            try {
                setLoading(true);
                await onSubmit(values);
                handleClose();
            } catch (error) {
                console.error('Error creating group:', error);
                toast.error('Failed to create group');
            } finally {
                setLoading(false);
            }
        }
    });

    const handleClose = () => {
        formik.resetForm();
        setImagePreview(null);
        setSelectedGroupType(null);
        onClose();
    };

    const isFormValid = () => {
        return formik.values.name.trim() !== '' && !formik.errors.name;
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            maxWidth="sm"
            fullWidth
            fullScreen={fullScreen}
        >
            <DialogTitle>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Typography variant="h6">Create New Group</Typography>
                    <IconButton onClick={handleClose} size="small">
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>

            <form onSubmit={formik.handleSubmit}>
                <DialogContent dividers>
                    <Grid container spacing={3}>
                        <Grid item xs={12}>
                            <Box sx={{ display: 'flex', gap: 2 }}>
                                {/* Image Upload Box */}
                                <Box
                                    sx={{
                                        width: 100,
                                        height: 100,
                                        flexShrink: 0,
                                        border: '1px dashed',
                                        borderColor: 'primary.main',
                                        borderRadius: 1,
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'center',
                                        position: 'relative',
                                        overflow: 'hidden',
                                        cursor: 'pointer'
                                    }}
                                    onClick={() => setImageSourceDialogOpen(true)}
                                >
                                    {imagePreview ? (
                                        <>
                                            <img
                                                src={imagePreview}
                                                alt="Group"
                                                style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                                            />
                                            <IconButton
                                                size="small"
                                                sx={{
                                                    position: 'absolute',
                                                    top: 4,
                                                    right: 4,
                                                    bgcolor: 'background.paper'
                                                }}
                                                onClick={(e) => {
                                                    e.stopPropagation();
                                                    handleRemoveImage();
                                                }}
                                            >
                                                <DeleteIcon fontSize="small" />
                                            </IconButton>
                                        </>
                                    ) : (
                                        <UploadIcon />
                                    )}
                                </Box>

                                {/* Hidden file inputs */}
                                <input
                                    ref={cameraInputRef}
                                    type="file"
                                    accept="image/*"
                                    capture="environment"
                                    hidden
                                    onChange={handleImageCapture}
                                />
                                <input
                                    ref={galleryInputRef}
                                    type="file"
                                    accept="image/*"
                                    hidden
                                    onChange={handleImageCapture}
                                />

                                {/* Group Name TextField */}
                                <TextField
                                    fullWidth
                                    name="name"
                                    label="Group Name"
                                    value={formik.values.name}
                                    onChange={formik.handleChange}
                                    error={formik.touched.name && Boolean(formik.errors.name)}
                                    helperText={formik.touched.name && formik.errors.name}
                                />
                            </Box>
                        </Grid>

                        {/* Members Selection */}
                        <Grid item xs={12}>
                            <Autocomplete
                                multiple
                                options={contacts}
                                loading={loadingContacts}
                                getOptionLabel={(option) => `${option.contactUser.firstName} ${option.contactUser.lastName || ''}`}
                                value={formik.values.members}
                                onChange={(_, newValue) => {
                                    formik.setFieldValue('members', newValue);
                                }}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Select Members (Optional)"
                                        placeholder="Search members"
                                        error={formik.touched.members && Boolean(formik.errors.members)}
                                        helperText={formik.touched.members && formik.errors.members}
                                    />
                                )}
                                renderTags={(value, getTagProps) =>
                                    value.map((option, index) => (
                                        <Chip
                                            key={option.id}
                                            label={`${option.contactUser.firstName} ${option.contactUser.lastName || ''}`}
                                            {...getTagProps({ index })}
                                            avatar={
                                                <Avatar>
                                                    {option.contactUser.firstName[0]}
                                                </Avatar>
                                            }
                                        />
                                    ))
                                }
                                renderOption={(props, option) => (
                                    <li {...props}>
                                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                            <Avatar sx={{ width: 32, height: 32 }}>
                                                {option.contactUser.firstName[0]}
                                            </Avatar>
                                            <Box>
                                                <Typography>
                                                    {`${option.contactUser.firstName} ${option.contactUser.lastName || ''}`}
                                                </Typography>
                                                <Typography variant="body2" color="textSecondary">
                                                    {option.contactUser.email}
                                                </Typography>
                                            </Box>
                                        </Box>
                                    </li>
                                )}
                            />
                        </Grid>

                        {/* Group Types */}
                        <Grid item xs={12}>
                            <Typography variant="subtitle2" gutterBottom>
                                Group Type (Optional)
                            </Typography>
                            <Box 
                                sx={{ 
                                    display: 'flex', 
                                    gap: 1, 
                                    overflowX: 'auto', 
                                    pb: 1,
                                    '&::-webkit-scrollbar': {
                                        height: 6
                                    },
                                    '&::-webkit-scrollbar-thumb': {
                                        backgroundColor: 'rgba(0,0,0,.2)',
                                        borderRadius: 3
                                    }
                                }}
                            >
                                {groupTypes.map((type) => {
                                    const GroupIcon = getGroupIcon(type.name);
                                    return (
                                        <Paper
                                            key={type.id}
                                            elevation={selectedGroupType?.id === type.id ? 3 : 1}
                                            sx={{
                                                p: 1,
                                                cursor: 'pointer',
                                                minWidth: 80,
                                                textAlign: 'center',
                                                bgcolor: selectedGroupType?.id === type.id ? 'primary.light' : 'background.paper',
                                                color: selectedGroupType?.id === type.id ? 'primary.contrastText' : 'text.primary',
                                                '&:hover': {
                                                    bgcolor: 'action.hover'
                                                }
                                            }}
                                            onClick={() => handleGroupTypeSelect(type)}
                                        >
                                            <GroupIcon sx={{ fontSize: 24, mb: 0.5 }} />
                                            <Typography variant="caption" display="block">
                                                {type.name}
                                            </Typography>
                                        </Paper>
                                    );
                                })}
                            </Box>
                            {selectedGroupType?.description && (
                                <Typography 
                                    variant="body2" 
                                    color="textSecondary" 
                                    sx={{ mt: 1, fontStyle: 'italic' }}
                                >
                                    {selectedGroupType.description}
                                </Typography>
                            )}
                        </Grid>

                        {/* Conditional Fields Based on Group Type */}
                        {selectedGroupType && (
                            <Grid item xs={12}>
                                <Paper variant="outlined" sx={{ p: 2 }}>
                                    {selectedGroupType.name === 'Trip' && (
                                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                                            <FormControlLabel
                                                control={
                                                    <Switch
                                                        checked={formik.values.groupDetail.enableGroupDate}
                                                        onChange={(e) => formik.setFieldValue('groupDetail.enableGroupDate', e.target.checked)}
                                                    />
                                                }
                                                label="Enable Group Date"
                                            />
                                            <DatePicker
                                                label="Start Date"
                                                value={formik.values.groupDetail.startDate}
                                                onChange={(date) => formik.setFieldValue('groupDetail.startDate', date)}
                                                slotProps={{ 
                                                    textField: { 
                                                        fullWidth: true,
                                                        error: formik.touched.groupDetail?.startDate && Boolean(formik.errors.groupDetail?.startDate),
                                                        helperText: formik.touched.groupDetail?.startDate && formik.errors.groupDetail?.startDate
                                                    } 
                                                }}
                                            />
                                            <DatePicker
                                                label="End Date"
                                                value={formik.values.groupDetail.endDate}
                                                onChange={(date) => formik.setFieldValue('groupDetail.endDate', date)}
                                                slotProps={{ 
                                                    textField: { 
                                                        fullWidth: true,
                                                        error: formik.touched.groupDetail?.endDate && Boolean(formik.errors.groupDetail?.endDate),
                                                        helperText: formik.touched.groupDetail?.endDate && formik.errors.groupDetail?.endDate
                                                    } 
                                                }}
                                                minDate={formik.values.groupDetail.startDate}
                                            />
                                        </Box>
                                    )}

                                    {selectedGroupType.name === 'Home' && (
                                        <FormControlLabel
                                            control={
                                                <Switch
                                                    checked={formik.values.groupDetail.enableSettleUpReminders}
                                                    onChange={(e) => formik.setFieldValue('groupDetail.enableSettleUpReminders', e.target.checked)}
                                                />
                                            }
                                            label="Enable Settle Up Reminders"
                                        />
                                    )}

                                    {selectedGroupType.name === 'Couple' && (
                                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                                            <FormControlLabel
                                                control={
                                                    <Switch
                                                        checked={formik.values.groupDetail.enableBalanceAlert}
                                                        onChange={(e) => formik.setFieldValue('groupDetail.enableBalanceAlert', e.target.checked)}
                                                    />
                                                }
                                                label="Enable Balance Alert"
                                            />
                                            <TextField
                                                fullWidth
                                                label="Max Group Budget"
                                                type="number"
                                                value={formik.values.groupDetail.maxGroupBudget || ''}
                                                onChange={(e) => formik.setFieldValue('groupDetail.maxGroupBudget', parseFloat(e.target.value))}
                                                InputProps={{
                                                    startAdornment: <InputAdornment position="start">$</InputAdornment>
                                                }}
                                            />
                                        </Box>
                                    )}
                                </Paper>
                            </Grid>
                        )}
                    </Grid>
                </DialogContent>

                <DialogActions sx={{ p: 2 }}>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button 
                        type="submit" 
                        variant="contained"
                        disabled={loading || !isFormValid()}
                        startIcon={loading && <CircularProgress size={20} />}
                    >
                        {loading ? 'Creating...' : 'Create Group'}
                    </Button>
                </DialogActions>
            </form>

            {/* Image Source Selection Dialog */}
            <ImageSourceDialog
                open={imageSourceDialogOpen}
                onClose={() => setImageSourceDialogOpen(false)}
                onSelect={handleImageSourceSelect}
            />
        </Dialog>
    );
};

export default CreateGroupDialog; 