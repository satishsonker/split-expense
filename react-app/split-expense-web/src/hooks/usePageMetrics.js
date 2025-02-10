import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';

const usePageMetrics = () => {
    const location = useLocation();

    useEffect(() => {
        // Track page view
        if (window.gtag) {
            window.gtag('config', process.env.REACT_APP_GA_MEASUREMENT_ID, {
                page_path: location.pathname + location.search
            });
        }

        // Track page load performance
        if (window.performance && window.performance.getEntriesByType) {
            const navigationEntries = window.performance.getEntriesByType('navigation');
            if (navigationEntries.length > 0) {
                const timing = navigationEntries[0];
                
                const metrics = {
                    pageLoadTime: timing.loadEventEnd - timing.startTime,
                    dnsLookupTime: timing.domainLookupEnd - timing.domainLookupStart,
                    serverResponseTime: timing.responseEnd - timing.requestStart,
                    domContentLoadTime: timing.domContentLoadedEventEnd - timing.startTime,
                    path: location.pathname,
                };

                // Log or send metrics to your analytics service
                console.log('Page Performance Metrics:', metrics);
            }
        }

        // Clear performance entries for next page
        if (window.performance && window.performance.clearResourceTimings) {
            window.performance.clearResourceTimings();
        }
    }, [location]);
};

export default usePageMetrics; 