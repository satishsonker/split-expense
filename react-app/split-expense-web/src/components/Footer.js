import { Box, Container, Typography, Link, useTheme, useMediaQuery } from '@mui/material';

const Footer = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    return (
        <Box
            component="footer"
            sx={{
                py: 3,
                px: 2,
                mt: 'auto',
                backgroundColor: (theme) =>
                    theme.palette.mode === 'light'
                        ? theme.palette.grey[200]
                        : theme.palette.grey[800],
            }}
        >
            <Container maxWidth="lg">
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: isMobile ? 'column' : 'row',
                        justifyContent: 'space-between',
                        alignItems: isMobile ? 'center' : 'flex-start',
                        textAlign: isMobile ? 'center' : 'left',
                    }}
                >
                    <Box sx={{ mb: isMobile ? 2 : 0 }}>
                        <Typography variant="h6" color="text.primary" gutterBottom>
                            Split Expense
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            The easiest way to split expenses with friends and family
                        </Typography>
                    </Box>

                    <Box
                        sx={{
                            display: 'flex',
                            flexDirection: isMobile ? 'column' : 'row',
                            gap: 4,
                        }}
                    >
                        <Box>
                            <Typography variant="subtitle1" color="text.primary" gutterBottom>
                                Quick Links
                            </Typography>
                            <Link href="/about" color="inherit" display="block">About</Link>
                            <Link href="/contact" color="inherit" display="block">Contact</Link>
                            <Link href="/privacy" color="inherit" display="block">Privacy Policy</Link>
                        </Box>

                        <Box>
                            <Typography variant="subtitle1" color="text.primary" gutterBottom>
                                Help
                            </Typography>
                            <Link href="/faq" color="inherit" display="block">FAQ</Link>
                            <Link href="/support" color="inherit" display="block">Support</Link>
                            <Link href="/terms" color="inherit" display="block">Terms of Service</Link>
                        </Box>
                    </Box>
                </Box>

                <Box sx={{ mt: 3, textAlign: 'center' }}>
                    <Typography variant="body2" color="text.secondary">
                        © {new Date().getFullYear()} Split Expense. All rights reserved.
                    </Typography>
                </Box>
            </Container>
        </Box>
    );
};

export default Footer; 