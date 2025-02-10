import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
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
    CircularProgress,
    InputAdornment,
    IconButton
} from '@mui/material';
import {
    Visibility,
    VisibilityOff
} from '@mui/icons-material';
import { AUTH_PATHS } from '../constants/apiPaths';
import axios from 'axios';

const validationSchema = Yup.object({
    password: Yup.string()
        .min(6, 'Password must be at least 6 characters')
        .required('Password is required')
        .matches(
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/,
            'Password must contain at least one uppercase letter, one lowercase letter, and one number'
        ),
    confirmPassword: Yup.string()
        .oneOf([Yup.ref('password'), null], 'Passwords must match')
        .required('Confirm password is required'),
});

const ResetPassword = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const token = searchParams.get('token');
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const formik = useFormik({
        initialValues: {
            password: '',
            confirmPassword: '',
        },
        validationSchema,
        onSubmit: async (values) => {
            if (!token) {
                setError('Invalid reset token');
                return;
            }

            setError('');
            setSuccess('');
            setIsLoading(true);

            try {
                await axios.post(AUTH_PATHS.RESET_PASSWORD, {
                    token,
                    password: values.password,
                });
                setSuccess('Password has been successfully reset');
                setTimeout(() => navigate('/login'), 3000);
            } catch (err) {
                setError(err.response?.data?.message || 'An error occurred');
            } finally {
                setIsLoading(false);
            }
        },
    });

    if (!token) {
        return (
            <Box className="auth-container">
                <Card className="auth-form">
                    <CardContent>
                        <Alert severity="error">
                            Invalid reset token. Please request a new password reset.
                        </Alert>
                    </CardContent>
                </Card>
            </Box>
        );
    }

    return (
        <Box className="auth-container">
            <Card className="auth-form">
                <CardContent>
                    <Box sx={{ textAlign: 'center', mb: 3 }}>
                        <Typography variant="h5" component="h1" gutterBottom>
                            Reset Password
                        </Typography>
                        <Typography variant="body2" color="textSecondary">
                            Enter your new password below
                        </Typography>
                    </Box>

                    <form onSubmit={formik.handleSubmit}>
                        <TextField
                            fullWidth
                            id="password"
                            name="password"
                            label="New Password"
                            type={showPassword ? 'text' : 'password'}
                            variant="outlined"
                            margin="normal"
                            value={formik.values.password}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            error={formik.touched.password && Boolean(formik.errors.password)}
                            helperText={formik.touched.password && formik.errors.password}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton
                                            onClick={() => setShowPassword(!showPassword)}
                                            edge="end"
                                        >
                                            {showPassword ? <VisibilityOff /> : <Visibility />}
                                        </IconButton>
                                    </InputAdornment>
                                ),
                            }}
                        />

                        <TextField
                            fullWidth
                            id="confirmPassword"
                            name="confirmPassword"
                            label="Confirm Password"
                            type={showConfirmPassword ? 'text' : 'password'}
                            variant="outlined"
                            margin="normal"
                            value={formik.values.confirmPassword}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
                            helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton
                                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                            edge="end"
                                        >
                                            {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                                        </IconButton>
                                    </InputAdornment>
                                ),
                            }}
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
                                'Reset Password'
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
                    </form>
                </CardContent>
            </Card>
        </Box>
    );
};

export default ResetPassword; 