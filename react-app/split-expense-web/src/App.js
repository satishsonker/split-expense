import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { AuthProvider } from './context/AuthContext';
import { PerformanceProvider, usePerformance } from './context/PerformanceContext';
import ErrorBoundary from './components/ErrorBoundary';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import ForgotPassword from './pages/ForgotPassword';
import ResetPassword from './pages/ResetPassword';
import Layout from './components/Layout';
import PrivateRoute from './components/PrivateRoute';
import Groups from './pages/Groups';
import Contacts from './pages/Contacts';
import 'react-toastify/dist/ReactToastify.css';
import './App.css';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';

const AppContent = () => {
  const { trackError } = usePerformance();

  return (
    <ErrorBoundary onError={trackError}>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/" element={<PrivateRoute><Layout /></PrivateRoute>}>
            <Route index element={<Dashboard />} />
            <Route path="groups" element={<Groups />} />
            <Route path="contacts" element={<Contacts />} />
            {/* Add more protected routes here */}
          </Route>
        </Routes>
      </AuthProvider>
    </ErrorBoundary>
  );
};

function App() {
  return (
    <PerformanceProvider>
      <ErrorBoundary>
        <LocalizationProvider dateAdapter={AdapterDayjs}>
          <Router>
            <AuthProvider>
              <ToastContainer />
              <AppContent />
            </AuthProvider>
          </Router>
        </LocalizationProvider>
      </ErrorBoundary>
    </PerformanceProvider>
  );
}

export default App;
