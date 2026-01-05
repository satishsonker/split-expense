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
    // Replace spaces back with + signs (URL encoding converts + to space)
    const token = searchParams.get('token')?.replace(/\s/g, '+');
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
                    NewPassword: values.password,
                    ConfirmPassword: values.confirmPassword
                }).then(res => {
                    if(res.data.success){
                    setSuccess(res.data.message || 'Password has been reset successfully');
                    setTimeout(() => navigate('/login'), 3000);
                    }else{
                    setError(res.data.message || 'An error occurred');
                    }
                });

            } catch (err) {
                setError(err.response?.data?.message || 'An error occurred');
            } finally {
                setIsLoading(false);
            }
        },
    });

    if (!token) {
        return (
            <Box className="auth-container" sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', p: { xs: 1, sm: 2 } }}>
                <Card className="auth-form" sx={{ width: { xs: '100%', sm: '90%', md: '420px' }, maxWidth: '100%' }}>
                    <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
                        <Alert severity="error">
                            Invalid reset token. Please request a new password reset.
                        </Alert>
                    </CardContent>
                </Card>
            </Box>
        );
    }

    return (
        <Box className="auth-container" sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', p: { xs: 1, sm: 2 } }}>
            <Card className="auth-form" sx={{ width: { xs: '100%', sm: '90%', md: '420px' }, maxWidth: '100%' }}>
                <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
                    <Box sx={{ textAlign: 'center', mb: 3 }}>
                        <Typography variant="h5" component="h1" gutterBottom sx={{ fontSize: { xs: '1.25rem', sm: '1.5rem' } }}>
                            Reset Password
                        </Typography>
                        <Typography variant="body2" color="textSecondary" sx={{ fontSize: { xs: '0.8rem', sm: '0.875rem' } }}>
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
                            inputProps={{ style: { fontSize: { xs: '14px', sm: '16px' } } }}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton
                                            onClick={() => setShowPassword(!showPassword)}
                                            edge="end"
                                            size="large"
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
                            inputProps={{ style: { fontSize: { xs: '14px', sm: '16px' } } }}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton
                                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                            edge="end"
                                            size="large"
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
                            sx={{ mt: 3, py: { xs: 1.2, sm: 1.5 } }}
                            disabled={isLoading}
                        >
                            {isLoading ? (
                                <CircularProgress size={24} color="inherit" />
                            ) : (
                                'Reset Password'
                            )}
                        </Button>

                        {error && (
                            <Alert severity="error" sx={{ mt: 2, fontSize: { xs: '0.8rem', sm: '0.875rem' } }}>
                                {error}
                            </Alert>
                        )}

                        {success && (
                            <Alert severity="success" sx={{ mt: 2, fontSize: { xs: '0.8rem', sm: '0.875rem' } }}>
                                {success}
                            </Alert>
                        )}
                    </form>

                    <Box sx={{ mt: 2, textAlign: 'center' }}>
                        <Button
                            fullWidth
                            variant="outlined"
                            size="large"
                            onClick={() => navigate('/login')}
                            sx={{ py: { xs: 1.2, sm: 1.5 } }}
                        >
                            Back to Login
                        </Button>
                    </Box>
                </CardContent>
            </Card>
        </Box>
    );
};

export default ResetPassword; 