import React, { useState, useEffect } from 'react';
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
    InputAdornment
} from '@mui/material';
import { Close as CloseIcon, Add as AddIcon } from '@mui/icons-material';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { getGroupIcon } from '../utils/groupIcons';
import { GROUP_PATHS } from '../constants/apiPaths';
import { toast } from 'react-toastify';
import { apiService } from '../utils/axios';

const validationSchema = Yup.object({
    name: Yup.string()
        .required('Group name is required')
        .max(100, 'Group name must be at most 100 characters'),
    members: Yup.array()
        .of(
            Yup.object({
                email: Yup.string()
                    .email('Invalid email address')
                    .required('Email is required'),
                name: Yup.string()
            })
        )
});

const CreateGroupDialog = ({ open, onClose, onSubmit, group }) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
    const [selectedUser, setSelectedUser] = useState(null);
    const [users, setUsers] = useState([]);
    const [groupIcon, setGroupIcon] = useState(null);
    const isEditing = Boolean(group);

    // Fetch users/contacts from API
    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await apiService.get(GROUP_PATHS.MEMBERS);
                debugger;
                var modifiedResponse=response.data?.map((ele,index)=>{
                    return ele?.contactUser;
                });
                setUsers(modifiedResponse || []);
            } catch (error) {
                console.error('Error fetching contacts:', error);
            }
        };

        if (open) {
            fetchUsers();
        }
    }, [open]);

    const formik = useFormik({
        initialValues: {
            name: group?.name || '',
            members: group?.members?.map(m => ({
                id: m.addedUser.id,
                email: m.addedUser.email,
                name: `${m.addedUser.firstName} ${m.addedUser.lastName}`
            })) || []
        },
        validationSchema,
        enableReinitialize: true,
        onSubmit: async (values) => {
            if (isEditing) {
                await apiService.put(`${GROUP_PATHS.UPDATE}/${group.id}`, {
                    name: values.name,
                    members: values.members.map(m => m.id)
                });
            } else {
                await apiService.post(GROUP_PATHS.CREATE, {
                    name: values.name,
                    members: values.members.map(m => m.id)
                });
            }
            onSubmit(values);
            formik.resetForm();
        }
    });

    // Update group icon when name changes
    useEffect(() => {
        const IconComponent = getGroupIcon(formik.values.name);
        setGroupIcon(IconComponent);
    }, [formik.values.name]);

    const handleAddMember = () => {
        if (selectedUser && !formik.values.members.find(m => m.email === selectedUser.email)) {
            formik.setFieldValue('members', [
                ...formik.values.members,
                { id: selectedUser.userId, email: selectedUser.email, name: selectedUser.firstName }
            ]);
            setSelectedUser(null);
        }
    };

    const handleRemoveMember = (email) => {
        formik.setFieldValue(
            'members',
            formik.values.members.filter(m => m.email !== email)
        );
    };

    const handleClose = () => {
        formik.resetForm();
        setSelectedUser(null);
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
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        {groupIcon && (
                            <Avatar sx={{ bgcolor: 'primary.main' }}>
                                {React.createElement(groupIcon)}
                            </Avatar>
                        )}
                        <Typography variant="h6">
                            {isEditing ? 'Edit Group' : 'Create New Group'}
                        </Typography>
                    </Box>
                    <IconButton onClick={handleClose} size="small">
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>

            <form onSubmit={formik.handleSubmit}>
                <DialogContent>
                    <TextField
                        fullWidth
                        id="name"
                        name="name"
                        label="Group Name"
                        value={formik.values.name}
                        onChange={formik.handleChange}
                        error={formik.touched.name && Boolean(formik.errors.name)}
                        helperText={formik.touched.name && formik.errors.name}
                        margin="normal"
                        InputProps={{
                            endAdornment: groupIcon && (
                                <InputAdornment position="end">
                                    {React.createElement(groupIcon)}
                                </InputAdornment>
                            )
                        }}
                    />

                    <Box sx={{ mt: 3 }}>
                        <Typography variant="subtitle1" gutterBottom>
                            Add Members (Optional)
                        </Typography>
                        
                        <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
                            <Autocomplete
                                fullWidth
                                value={selectedUser}
                                onChange={(event, newValue) => {
                                    setSelectedUser(newValue);
                                }}
                                options={users.filter(user => 
                                    !formik.values.members.find(m => m.email === user.email)
                                )}
                                getOptionLabel={(option) => `${option.firstName} (${option.email})`}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="Select Member"
                                    />
                                )}
                            />
                            <Button
                                variant="contained"
                                onClick={handleAddMember}
                                disabled={!selectedUser}
                                sx={{ 
                                    minWidth: '120px',
                                    height: '56px', // Match Autocomplete height
                                    whiteSpace: 'nowrap'
                                }}
                            >
                                <AddIcon sx={{ mr: 1 }} />
                                Add
                            </Button>
                        </Box>

                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                            {formik.values.members.map((member) => (
                                <Chip
                                    key={member.email}
                                    avatar={<Avatar>{member.name[0]}</Avatar>}
                                    label={`${member.name} (${member.email})`}
                                    onDelete={() => handleRemoveMember(member.email)}
                                />
                            ))}
                        </Box>
                    </Box>
                </DialogContent>

                <DialogActions sx={{ p: 2 }}>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button 
                        type="submit" 
                        variant="contained"
                        disabled={formik.isSubmitting || !formik.values.name}
                    >
                        {isEditing ? 'Update Group' : 'Create Group'}
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );
};

export default CreateGroupDialog; 