import React, { createContext, useContext, useState } from 'react';

const PerformanceContext = createContext(null);

export const PerformanceProvider = ({ children }) => {
    const [metrics, setMetrics] = useState({
        pageLoads: {},
        interactions: {},
        errors: []
    });

    const trackPageLoad = (path, loadTime) => {
        setMetrics(prev => ({
            ...prev,
            pageLoads: {
                ...prev.pageLoads,
                [path]: [...(prev.pageLoads[path] || []), loadTime]
            }
        }));
    };

    const trackInteraction = (name, duration) => {
        setMetrics(prev => ({
            ...prev,
            interactions: {
                ...prev.interactions,
                [name]: [...(prev.interactions[name] || []), duration]
            }
        }));
    };

    const trackError = (error) => {
        setMetrics(prev => ({
            ...prev,
            errors: [...prev.errors, { timestamp: Date.now(), error }]
        }));
    };

    return (
        <PerformanceContext.Provider value={{ metrics, trackPageLoad, trackInteraction, trackError }}>
            {children}
        </PerformanceContext.Provider>
    );
};

export const usePerformance = () => {
    const context = useContext(PerformanceContext);
    if (!context) {
        throw new Error('usePerformance must be used within a PerformanceProvider');
    }
    return context;
}; 