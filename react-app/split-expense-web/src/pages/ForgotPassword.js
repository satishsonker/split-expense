import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import {
    Box,
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    Alert,
    CircularProgress
} from '@mui/material';
import { AUTH_PATHS } from '../constants/apiPaths';
import axios from 'axios';

const validationSchema = Yup.object({
    email: Yup.string()
        .email('Invalid email address')
        .required('Email is required'),
});

const ForgotPassword = () => {
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const formik = useFormik({
        initialValues: {
            email: '',
        },
        validationSchema,
        onSubmit: async (values) => {
            setError('');
            setSuccess('');
            setIsLoading(true);
            try {
                await axios.post(AUTH_PATHS.FORGOT_PASSWORD, values);
                setSuccess('Password reset instructions have been sent to your email');
                setTimeout(() => navigate('/login'), 3000);
            } catch (err) {
                setError(err.response?.data?.message || 'An error occurred');
            } finally {
                setIsLoading(false);
            }
        },
    });

    return (
        <Box className="auth-container">
            <Card className="auth-form">
                <CardContent>
                    <Box sx={{ textAlign: 'center', mb: 3 }}>
                        <Typography variant="h5" component="h1" gutterBottom>
                            Forgot Password
                        </Typography>
                        <Typography variant="body2" color="textSecondary">
                            Enter your email address and we'll send you instructions to reset your password.
                        </Typography>
                    </Box>

                    <form onSubmit={formik.handleSubmit}>
                        <TextField
                            fullWidth
                            id="email"
                            name="email"
                            label="Email"
                            variant="outlined"
                            margin="normal"
                            value={formik.values.email}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            error={formik.touched.email && Boolean(formik.errors.email)}
                            helperText={formik.touched.email && formik.errors.email}
                        />

                        <Button
                            fullWidth
                            type="submit"
                            variant="contained"
                            size="large"
                            sx={{ mt: 3 }}
                            disabled={isLoading}
                        >
                            {isLoading ? (
                                <CircularProgress size={24} color="inherit" />
                            ) : (
                                'Send Reset Instructions'
                            )}
                        </Button>

                        {error && (
                            <Alert severity="error" sx={{ mt: 2 }}>
                                {error}
                            </Alert>
                        )}

                        {success && (
                            <Alert severity="success" sx={{ mt: 2 }}>
                                {success}
                            </Alert>
                        )}

                        <Box sx={{ mt: 3, textAlign: 'center' }}>
                            <Link 
                                to="/login"
                                style={{ textDecoration: 'none', color: 'primary.main' }}
                            >
                                <Typography variant="body2" color="primary">
                                    Back to Login
                                </Typography>
                            </Link>
                        </Box>
                    </form>
                </CardContent>
            </Card>
        </Box>
    );
};

export default ForgotPassword; 