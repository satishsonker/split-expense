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
    InputAdornment,
    IconButton,
    Divider,
    CircularProgress,
    Alert
} from '@mui/material';
import {
    Visibility,
    VisibilityOff,
    Google as GoogleIcon
} from '@mui/icons-material';
import { useAuth } from '../context/AuthContext';
import { AUTH_PATHS } from '../constants/apiPaths';
import { apiService } from '../utils/axios';

const validationSchema = Yup.object({
    email: Yup.string()
        .email('Invalid email address')
        .required('Email is required'),
    password: Yup.string()
        .min(6, 'Password must be at least 6 characters')
        .required('Password is required'),
});

const Login = () => {
    const navigate = useNavigate();
    const { login } = useAuth();
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const formik = useFormik({
        initialValues: {
            email: '',
            password: '',
        },
        validationSchema,
        onSubmit: async (values) => {
            setError('');
            setIsLoading(true);
            try {
                const response = await apiService.post(AUTH_PATHS.LOGIN, values);
                debugger;
                localStorage.setItem('user', JSON.stringify(response.user));
                localStorage.setItem('token', JSON.stringify(response));
                login(response.user);
                navigate('/');
            } catch (err) {
                setError(err.message || 'An error occurred during login');
            } finally {
                setIsLoading(false);
            }
        },
    });

    const handleGoogleLogin = async () => {
        // Implement Google login logic here
        window.location.href = AUTH_PATHS.GOOGLE_LOGIN;
    };

    return (
        <Box className="auth-container">
            <Card className="auth-form">
                <CardContent>
                    {/* Logo and App Name */}
                    <Box sx={{ textAlign: 'center', mb: 3 }}>
                        <img 
                            src="/logo.png" 
                            alt="Split Expense Logo" 
                            style={{ width: 80, height: 80, marginBottom: 16 }}
                        />
                        <Typography variant="h5" component="h1" gutterBottom>
                            Split Expense
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

                        <TextField
                            fullWidth
                            id="password"
                            name="password"
                            label="Password"
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

                        <Box sx={{ mt: 2, textAlign: 'right' }}>
                            <Link 
                                to="/forgot-password"
                                style={{ textDecoration: 'none', color: 'primary.main' }}
                            >
                                <Typography variant="body2" color="primary">
                                    Forgot Password?
                                </Typography>
                            </Link>
                        </Box>

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
                                'Login'
                            )}
                        </Button>

                        {error && (
                            <Alert severity="error" sx={{ mt: 2 }}>
                                {error}
                            </Alert>
                        )}

                        <Divider sx={{ my: 3 }}>OR</Divider>

                        <Button
                            fullWidth
                            variant="outlined"
                            size="large"
                            onClick={handleGoogleLogin}
                            startIcon={<GoogleIcon />}
                        >
                            Continue with Google
                        </Button>

                        <Box sx={{ mt: 3, textAlign: 'center' }}>
                            <Typography variant="body2">
                                Don't have an account?{' '}
                                <Link 
                                    to="/register"
                                    style={{ textDecoration: 'none', color: 'primary.main' }}
                                >
                                    Sign Up
                                </Link>
                            </Typography>
                        </Box>
                    </form>
                </CardContent>
            </Card>
        </Box>
    );
};

export default Login; 