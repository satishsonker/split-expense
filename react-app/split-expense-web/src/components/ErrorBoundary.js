import React from 'react';
import {
    Box,
    Typography,
    Button,
    Paper,
    Container
} from '@mui/material';
import {
    Error as ErrorIcon,
    NetworkCheck as NetworkIcon,
    Storage as StorageIcon,
    Code as CodeIcon,
    BugReport as BugIcon,
    Refresh as RefreshIcon
} from '@mui/icons-material';

class ErrorBoundary extends React.Component {
    constructor(props) {
        super(props);
        this.state = { 
            hasError: false,
            error: null,
            errorInfo: null
        };
    }

    static getDerivedStateFromError(error) {
        return { hasError: true };
    }

    componentDidCatch(error, errorInfo) {
        this.setState({
            error: error,
            errorInfo: errorInfo
        });

        // Track error if tracking function is provided
        if (this.props.onError) {
            this.props.onError({
                message: error.message,
                stack: error.stack,
                componentStack: errorInfo.componentStack
            });
        }

        // Log error to console in development
        if (process.env.NODE_ENV === 'development') {
            console.error('Error caught by ErrorBoundary:', error, errorInfo);
        }
    }

    getErrorType() {
        const { error } = this.state;
        if (!error) return 'UNKNOWN';

        if (error.name === 'NetworkError' || error.message.includes('network'))
            return 'NETWORK';
        if (error.name === 'SyntaxError' || error.name === 'TypeError')
            return 'CODE';
        if (error.message.includes('localStorage') || error.message.includes('storage'))
            return 'STORAGE';
        if (error.name === 'ApiError' || error.message.includes('API'))
            return 'API';
        
        return 'UNKNOWN';
    }

    getErrorContent() {
        const errorType = this.getErrorType();
        const contents = {
            NETWORK: {
                icon: <NetworkIcon sx={{ fontSize: 60, color: 'error.main' }} />,
                title: 'Connection Error',
                message: 'We\'re having trouble connecting to our servers. Please check your internet connection and try again.',
            },
            CODE: {
                icon: <CodeIcon sx={{ fontSize: 60, color: 'error.main' }} />,
                title: 'Technical Error',
                message: 'Something went wrong in our application. Our team has been notified and is working on it.',
            },
            STORAGE: {
                icon: <StorageIcon sx={{ fontSize: 60, color: 'error.main' }} />,
                title: 'Storage Error',
                message: 'There was a problem accessing your local storage. Please make sure cookies are enabled.',
            },
            API: {
                icon: <BugIcon sx={{ fontSize: 60, color: 'error.main' }} />,
                title: 'Service Error',
                message: 'We\'re having trouble with our service. Please try again in a few minutes.',
            },
            UNKNOWN: {
                icon: <ErrorIcon sx={{ fontSize: 60, color: 'error.main' }} />,
                title: 'Unexpected Error',
                message: 'An unexpected error occurred. Please try refreshing the page.',
            },
        };

        return contents[errorType];
    }

    render() {
        if (this.state.hasError) {
            const errorContent = this.getErrorContent();
            
            return (
                <Container maxWidth="sm">
                    <Box
                        sx={{
                            minHeight: '100vh',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            p: 3
                        }}
                    >
                        <Paper
                            elevation={3}
                            sx={{
                                p: 4,
                                textAlign: 'center',
                                borderRadius: 2
                            }}
                        >
                            {errorContent.icon}
                            
                            <Typography variant="h5" component="h1" sx={{ mt: 2, mb: 1 }}>
                                {errorContent.title}
                            </Typography>
                            
                            <Typography color="text.secondary" sx={{ mb: 3 }}>
                                {errorContent.message}
                            </Typography>

                            <Button
                                variant="contained"
                                startIcon={<RefreshIcon />}
                                onClick={() => window.location.reload()}
                            >
                                Refresh Page
                            </Button>

                            {process.env.NODE_ENV === 'development' && (
                                <Box sx={{ mt: 4, textAlign: 'left' }}>
                                    <Typography variant="subtitle2" color="error" sx={{ mb: 1 }}>
                                        Error Details (Development Only):
                                    </Typography>
                                    <pre style={{ 
                                        overflow: 'auto',
                                        padding: '8px',
                                        background: '#f5f5f5',
                                        borderRadius: '4px',
                                        fontSize: '12px'
                                    }}>
                                        {this.state.error && this.state.error.toString()}
                                        {this.state.errorInfo && this.state.errorInfo.componentStack}
                                    </pre>
                                </Box>
                            )}
                        </Paper>
                    </Box>
                </Container>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary; 