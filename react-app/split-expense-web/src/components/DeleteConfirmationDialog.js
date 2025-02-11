import React from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    Typography,
    Avatar,
    useTheme,
    useMediaQuery,
    AvatarGroup,
    Chip
} from '@mui/material';
import {
    Warning as WarningIcon,
    Group as GroupIcon,
    Person as PersonIcon
} from '@mui/icons-material';

const DeleteConfirmationDialog = ({
    open,
    onClose,
    onConfirm,
    loading,
    title,
    type = 'contact', // 'contact' or 'group'
    data,
    warningMessage
}) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));

    const renderContent = () => {
        if (type === 'contact') {
            return (
                <>
                    <Avatar
                        sx={{
                            width: 64,
                            height: 64,
                            margin: '0 auto',
                            mb: 2,
                            bgcolor: theme.palette.primary.main
                        }}
                    >
                        <PersonIcon sx={{ fontSize: 32 }} />
                    </Avatar>
                    <Box sx={{ mt: 2, p: 2, bgcolor: 'background.default', borderRadius: 1 }}>
                        <Typography variant="subtitle1">
                            {`${data?.contactUser?.firstName} ${data?.contactUser?.lastName || ''}`}
                        </Typography>
                        <Typography color="textSecondary" variant="body2">
                            {data?.contactUser?.email}
                        </Typography>
                        {data?.contactUser?.phoneNumber && (
                            <Typography color="textSecondary" variant="body2">
                                {data.contactUser.phoneNumber}
                            </Typography>
                        )}
                    </Box>
                </>
            );
        }

        if (type === 'group') {
            return (
                <>
                    <Avatar
                        sx={{
                            width: 64,
                            height: 64,
                            margin: '0 auto',
                            mb: 2,
                            bgcolor: theme.palette.primary.main
                        }}
                    >
                        <GroupIcon sx={{ fontSize: 32 }} />
                    </Avatar>
                    <Box sx={{ mt: 2, p: 2, bgcolor: 'background.default', borderRadius: 1 }}>
                        <Typography variant="subtitle1" gutterBottom>
                            {data?.name}
                        </Typography>
                        {data?.description && (
                            <Typography color="textSecondary" variant="body2" gutterBottom>
                                {data.description}
                            </Typography>
                        )}
                        {data?.members && data.members.length > 0 && (
                            <Box sx={{ mt: 2 }}>
                                <Typography variant="body2" color="textSecondary" gutterBottom>
                                    Members ({data.members.length}):
                                </Typography>
                                <AvatarGroup
                                    max={5}
                                    sx={{
                                        justifyContent: 'center',
                                        '& .MuiAvatar-root': {
                                            width: 32,
                                            height: 32,
                                            fontSize: '0.875rem'
                                        }
                                    }}
                                >
                                    {data.members.map((member) => (
                                        <Avatar
                                            key={member.id}
                                            alt={member.addedUser?.firstName}
                                            src={member.addedUser?.profilePicture}
                                        >
                                            {member.addedUser?.firstName?.[0]}
                                        </Avatar>
                                    ))}
                                </AvatarGroup>
                            </Box>
                        )}
                        {data?.expenses?.length > 0 && (
                            <Box sx={{ mt: 2 }}>
                                <Chip
                                    label={`${data.expenses.length} Active Expenses`}
                                    color="warning"
                                    size="small"
                                />
                            </Box>
                        )}
                    </Box>
                </>
            );
        }
    };

    return (
        <Dialog
            open={open}
            onClose={onClose}
            maxWidth="xs"
            fullWidth
            fullScreen={fullScreen}
        >
            <DialogTitle>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <WarningIcon color="warning" />
                    <Typography variant="h6">{title || `Delete ${type}`}</Typography>
                </Box>
            </DialogTitle>
            <DialogContent>
                <Box sx={{ textAlign: 'center', py: 2 }}>
                    <Typography variant="body1" gutterBottom>
                        Are you sure you want to delete this {type}?
                    </Typography>
                    {renderContent()}
                    <Typography color="error" variant="body2" sx={{ mt: 2 }}>
                        {warningMessage || 'This action cannot be undone.'}
                    </Typography>
                </Box>
            </DialogContent>
            <DialogActions sx={{ p: 2, pt: 0 }}>
                <Button
                    onClick={onClose}
                    disabled={loading}
                >
                    Cancel
                </Button>
                <Button
                    onClick={onConfirm}
                    variant="contained"
                    color="error"
                    disabled={loading}
                    sx={{ minWidth: 100 }}
                >
                    {loading ? 'Deleting...' : 'Delete'}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default DeleteConfirmationDialog; 