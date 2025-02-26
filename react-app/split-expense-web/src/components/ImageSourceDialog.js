import React from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    Grid,
    Button,
    Typography,
    useTheme,
    useMediaQuery
} from '@mui/material';
import {
    PhotoCamera as CameraIcon,
    Image as GalleryIcon
} from '@mui/icons-material';

const ImageSourceDialog = ({ open, onClose, onSelect }) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));

    return (
        <Dialog 
            open={open} 
            onClose={onClose}
            fullScreen={fullScreen}
            maxWidth="xs" 
            fullWidth
        >
            <DialogTitle>
                <Typography variant="h6">Select Image Source</Typography>
            </DialogTitle>
            <DialogContent>
                <Grid container spacing={2} sx={{ mt: 1 }}>
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

export default ImageSourceDialog; 