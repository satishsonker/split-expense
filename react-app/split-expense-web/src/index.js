import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './utils/reportWebVitals';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

// Function to handle web vitals metrics
const reportWebVitalsMetrics = (metric) => {
  // You can send metrics to your analytics service here
  console.log(metric);

  // Example of sending to Google Analytics
  if (window.gtag) {
    window.gtag('event', metric.name, {
      event_category: 'Web Vitals',
      event_label: metric.id,
      value: Math.round(metric.name === 'CLS' ? metric.value * 1000 : metric.value),
      non_interaction: true,
    });
  }

  // Example of sending to custom backend
  // fetch('/api/metrics', {
  //   method: 'POST',
  //   headers: { 'Content-Type': 'application/json' },
  //   body: JSON.stringify(metric)
  // });
};

// Report web vitals
reportWebVitals(reportWebVitalsMetrics);
