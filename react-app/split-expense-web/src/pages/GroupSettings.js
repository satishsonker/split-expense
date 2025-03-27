import React, { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    Card,
    CardContent,
    Button,
    TextField,
    Avatar,
    IconButton,
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    ListItemSecondary,
    Divider,
    useTheme,
    useMediaQuery
} from '@mui/material';
import {
    Edit as EditIcon,
    Delete as DeleteIcon,
    PhotoCamera as CameraIcon,
    PersonAdd as PersonAddIcon,
    ExitToApp as ExitIcon
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { GROUP_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';
import { getImageUrl } from '../utils/imageUtils';
import { toast } from 'react-toastify';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import ImageSourceDialog from '../components/ImageSourceDialog';
import DeleteConfirmationDialog from '../components/DeleteConfirmationDialog';
import { capitalizeText } from '../utils/stringUtils';
import AddGroupMemberDialog from '../components/AddGroupMemberDialog';

const validationSchema = Yup.object({
    name: Yup.string().required('Group name is required')
});

const GroupSettings = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const navigate = useNavigate();
    const { id } = useParams();
    const [group, setGroup] = useState(null);
    const [loading, setLoading] = useState(true);
    const [imageSourceDialogOpen, setImageSourceDialogOpen] = useState(false);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [leaveDialogOpen, setLeaveDialogOpen] = useState(false);
    const [addMemberDialogOpen, setAddMemberDialogOpen] = useState(false);

    const formik = useFormik({
        initialValues: {
            name: '',
            image: null
        },
        validationSchema,
        onSubmit: handleUpdateGroup
    });

    useEffect(() => {
        fetchGroupDetails();
    }, [id]);

    useEffect(() => {
        if (group) {
            formik.setValues({
                name: group.name || '',
                image: null
            });
        }
    }, [group]);

    const fetchGroupDetails = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(`${GROUP_PATHS.GET}${id}`);
            setGroup(response);
        } catch (error) {
            console.error('Error fetching group details:', error);
            toast.error('Failed to load group details');
        } finally {
            setLoading(false);
        }
    };

    async function handleUpdateGroup(values) {
        try {
            setLoading(true);
            const formData = new FormData();
            formData.append('name', capitalizeText(values.name));
            formData.append('id', id);
            
            if (values.image) {
                formData.append('image', values.image);
            }

            await apiService.put(GROUP_PATHS.UPDATE, formData);
            toast.success('Group updated successfully');
            navigate(`/groups/${id}`);
        } catch (error) {
            console.error('Error updating group:', error);
            toast.error('Failed to update group');
        } finally {
            setLoading(false);
        }
    }

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
            }
        };
        input.click();
    };

    const handleDeleteGroup = async () => {
        try {
            setLoading(true);
            await apiService.delete(`${GROUP_PATHS.DELETE}/${id}`);
            toast.success('Group deleted successfully');
            navigate('/groups');
        } catch (error) {
            console.error('Error deleting group:', error);
            toast.error('Failed to delete group');
        } finally {
            setLoading(false);
            setDeleteDialogOpen(false);
        }
    };

    const handleLeaveGroup = async () => {
        try {
            setLoading(true);
            await apiService.post(`${GROUP_PATHS.LEAVE}/${id}`);
            toast.success('Left group successfully');
            navigate('/groups');
        } catch (error) {
            console.error('Error leaving group:', error);
            toast.error('Failed to leave group');
        } finally {
            setLoading(false);
            setLeaveDialogOpen(false);
        }
    };

    const handleAddMemberDialogClose = (wasSuccessful) => {
        setAddMemberDialogOpen(false);
        if (wasSuccessful) {
            fetchGroupDetails(); // Refresh the group details
        }
    };

    return (
        <Box sx={{ p: { xs: 2, sm: 3 } }}>
            <Typography variant={isMobile ? 'h6' : 'h5'} gutterBottom>
                Group Settings
            </Typography>

            <form onSubmit={formik.handleSubmit}>
                <Card sx={{ mb: 3 }}>
                    <CardContent>
                        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2 }}>
                            <Box sx={{ position: 'relative' }}>
                                <Avatar
                                    src={group?.thumbImagePath ? getImageUrl(group.thumbImagePath) : undefined}
                                    sx={{ width: 100, height: 100 }}
                                />
                                <IconButton
                                    sx={{
                                        position: 'absolute',
                                        bottom: 0,
                                        right: 0,
                                        bgcolor: 'background.paper'
                                    }}
                                    onClick={() => setImageSourceDialogOpen(true)}
                                >
                                    <EditIcon />
                                </IconButton>
                            </Box>
                            <TextField
                                fullWidth
                                label="Group Name"
                                name="name"
                                value={formik.values.name}
                                onChange={formik.handleChange}
                                error={formik.touched.name && Boolean(formik.errors.name)}
                                helperText={formik.touched.name && formik.errors.name}
                            />
                        </Box>
                    </CardContent>
                </Card>

                {/* Members Section */}
                <Card sx={{ mb: 3 }}>
                    <CardContent>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                            <Typography variant="h6">Members</Typography>
                            <Button
                                startIcon={<PersonAddIcon />}
                                onClick={() => setAddMemberDialogOpen(true)}
                            >
                                Add Member
                            </Button>
                        </Box>
                        <List>
                            {group?.members?.map((member, index) => (
                                <React.Fragment key={member.id}>
                                    <ListItem
                                        secondaryAction={
                                            group.createdBy === member.addedUser.id ? (
                                                <Typography variant="caption" color="primary">
                                                    Admin
                                                </Typography>
                                            ) : (
                                                <IconButton
                                                    edge="end"
                                                    onClick={() => {/* Handle remove member */}}
                                                >
                                                    <DeleteIcon />
                                                </IconButton>
                                            )
                                        }
                                    >
                                        <ListItemAvatar>
                                            <Avatar
                                                src={member.addedUser?.profilePicture ? getImageUrl(member.addedUser.profilePicture) : undefined}
                                            >
                                                {member.addedUser?.firstName?.[0]}
                                            </Avatar>
                                        </ListItemAvatar>
                                        <ListItemText
                                            primary={`${member.addedUser?.firstName} ${member.addedUser?.lastName || ''}`}
                                            secondary={member.addedUser?.email}
                                        />
                                    </ListItem>
                                    {index < group.members.length - 1 && <Divider />}
                                </React.Fragment>
                            ))}
                        </List>
                    </CardContent>
                </Card>

                {/* Action Buttons */}
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                    <Button
                        type="submit"
                        variant="contained"
                        disabled={loading || !formik.isValid}
                        fullWidth
                    >
                        Save Changes
                    </Button>
                    <Button
                        variant="outlined"
                        color="error"
                        startIcon={<ExitIcon />}
                        onClick={() => setLeaveDialogOpen(true)}
                        fullWidth
                    >
                        Leave Group
                    </Button>
                    <Button
                        variant="contained"
                        color="error"
                        startIcon={<DeleteIcon />}
                        onClick={() => setDeleteDialogOpen(true)}
                        fullWidth
                    >
                        Delete Group
                    </Button>
                </Box>
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
                onConfirm={handleDeleteGroup}
                loading={loading}
                title="Delete Group"
                type="group"
                data={group}
                warningMessage="This will permanently delete the group and all its data. This action cannot be undone."
            />

            <DeleteConfirmationDialog
                open={leaveDialogOpen}
                onClose={() => setLeaveDialogOpen(false)}
                onConfirm={handleLeaveGroup}
                loading={loading}
                title="Leave Group"
                type="group"
                data={group}
                warningMessage="Are you sure you want to leave this group?"
            />

            <AddGroupMemberDialog
                open={addMemberDialogOpen}
                onClose={handleAddMemberDialogClose}
                groupId={id}
                existingMembers={group?.members || []}
            />
        </Box>
    );
};

export default GroupSettings; 