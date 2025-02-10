const reportWebVitals = async (onPerfEntry) => {
  if (onPerfEntry && onPerfEntry instanceof Function) {
    try {
      const webVitals = await import('web-vitals');
      const { getCLS, getFID, getFCP, getLCP, getTTFB } = webVitals;

      getCLS(onPerfEntry); // Cumulative Layout Shift
      getFID(onPerfEntry); // First Input Delay
      getFCP(onPerfEntry); // First Contentful Paint
      getLCP(onPerfEntry); // Largest Contentful Paint
      getTTFB(onPerfEntry); // Time to First Byte
    } catch (error) {
      console.error('Error loading web-vitals:', error);
    }
  }
};

export default reportWebVitals; 