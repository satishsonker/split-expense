import React, { useState } from 'react';
import { Dialog, DialogContent, IconButton, CircularProgress, Box, Typography } from '@mui/material';
import { Close as CloseIcon, BrokenImage as BrokenImageIcon } from '@mui/icons-material';

const ImageViewDialog = ({ open, onClose, imagePath }) => {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(false);

    const handleImageLoad = () => {
        setLoading(false);
        setError(false);
    };

    const handleImageError = () => {
        setLoading(false);
        setError(true);
    };

    return (
        <Dialog 
            open={open} 
            onClose={onClose}
            maxWidth="md"
            fullWidth
        >
            <IconButton
                onClick={onClose}
                sx={{
                    position: 'absolute',
                    right: 8,
                    top: 8,
                    color: 'white',
                    bgcolor: 'rgba(0, 0, 0, 0.5)',
                    zIndex: 1,
                    '&:hover': {
                        bgcolor: 'rgba(0, 0, 0, 0.7)'
                    }
                }}
            >
                <CloseIcon />
            </IconButton>
            <DialogContent sx={{ p: 0, minHeight: 300, position: 'relative' }}>
                {loading && (
                    <Box sx={{ 
                        position: 'absolute',
                        top: '50%',
                        left: '50%',
                        transform: 'translate(-50%, -50%)'
                    }}>
                        <CircularProgress />
                    </Box>
                )}
                {error ? (
                    <Box sx={{ 
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                        height: '100%',
                        minHeight: 300
                    }}>
                        <BrokenImageIcon sx={{ fontSize: 48, color: 'text.secondary' }} />
                        <Typography color="text.secondary" sx={{ mt: 2 }}>
                            Failed to load image
                        </Typography>
                    </Box>
                ) : (
                    <img 
                        src={imagePath} 
                        alt="Group" 
                        style={{ 
                            width: '100%',
                            height: 'auto',
                            display: 'block'
                        }}
                        onLoad={handleImageLoad}
                        onError={handleImageError}
                    />
                )}
            </DialogContent>
        </Dialog>
    );
};

export default ImageViewDialog; 