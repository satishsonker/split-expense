import { useState } from 'react';
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
    useTheme
} from '@mui/material';
import { Close as CloseIcon, Add as AddIcon } from '@mui/icons-material';
import { useFormik } from 'formik';
import * as Yup from 'yup';

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

const CreateGroupDialog = ({ open, onClose, onSubmit }) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
    const [newMemberEmail, setNewMemberEmail] = useState('');

    const formik = useFormik({
        initialValues: {
            name: '',
            members: []
        },
        validationSchema,
        onSubmit: (values) => {
            onSubmit(values);
            formik.resetForm();
        }
    });

    const handleAddMember = () => {
        if (newMemberEmail && !formik.values.members.find(m => m.email === newMemberEmail)) {
            formik.setFieldValue('members', [
                ...formik.values.members,
                { email: newMemberEmail, name: newMemberEmail.split('@')[0] }
            ]);
            setNewMemberEmail('');
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
        setNewMemberEmail('');
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
                    Create New Group
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
                    />

                    <Box sx={{ mt: 3 }}>
                        <Typography variant="subtitle1" gutterBottom>
                            Add Members (Optional)
                        </Typography>
                        
                        <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
                            <TextField
                                fullWidth
                                label="Member Email"
                                value={newMemberEmail}
                                onChange={(e) => setNewMemberEmail(e.target.value)}
                                onKeyPress={(e) => {
                                    if (e.key === 'Enter') {
                                        e.preventDefault();
                                        handleAddMember();
                                    }
                                }}
                            />
                            <IconButton 
                                onClick={handleAddMember}
                                color="primary"
                                sx={{ bgcolor: 'primary.light', '&:hover': { bgcolor: 'primary.main' } }}
                            >
                                <AddIcon />
                            </IconButton>
                        </Box>

                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                            {formik.values.members.map((member) => (
                                <Chip
                                    key={member.email}
                                    avatar={<Avatar>{member.name[0]}</Avatar>}
                                    label={member.email}
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
                        Create Group
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );
};

export default CreateGroupDialog; 