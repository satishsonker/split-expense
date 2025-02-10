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
import 'react-toastify/dist/ReactToastify.css';
import './App.css';
import Groups from './pages/Groups';

const AppContent = () => {
  const { trackError } = usePerformance();

  return (
    <ErrorBoundary onError={trackError}>
      <AuthProvider>
        <Router>
          <ToastContainer position="top-right" autoClose={3000} />
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/reset-password" element={<ResetPassword />} />
            <Route path="/" element={<PrivateRoute><Layout /></PrivateRoute>}>
              <Route index element={<Dashboard />} />
              <Route path="groups" element={<PrivateRoute><Layout /></PrivateRoute>}>
                <Route index element={<Groups />} />
              </Route>
              {/* Add more protected routes here */}
            </Route>
          </Routes>
        </Router>
      </AuthProvider>
    </ErrorBoundary>
  );
};

function App() {
  return (
    <PerformanceProvider>
      <AppContent />
    </PerformanceProvider>
  );
}

export default App;
