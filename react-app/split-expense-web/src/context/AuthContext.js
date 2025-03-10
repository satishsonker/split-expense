import React, { createContext, useState, useContext, useEffect } from 'react';
import {safeJsonParse} from '../utils/commonUtils'

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(() => {
        const savedUser = localStorage.getItem('user');
        return safeJsonParse(savedUser);
    });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // Check localStorage for existing user session
        const storedUser = localStorage.getItem('user');
        if (storedUser) {
            var user=safeJsonParse(storedUser);
            setUser(user);
        }
        setLoading(false);
    }, []);

    const login = (userData) => {
        localStorage.setItem('user', JSON.stringify(userData));
        setUser(userData);
    };

    const logout = () => {
        localStorage.removeItem('user');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, logout, loading }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext); 