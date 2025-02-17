const env = {
    ENV: process.env.REACT_APP_ENV || 'development',
    BASE_URL_API:process.env.REACT_APP_API_BASE_URL || 'http://localhost:5008/',
    API_URL: process.env.REACT_APP_API_BASE_URL+process.env.REACT_APP_API_SUFFIX || 'http://localhost:5008/api/v1',
    DEBUG: process.env.REACT_APP_DEBUG === 'true',
    APP_NAME: process.env.REACT_APP_NAME || 'Split Expense',
    VERSION: process.env.REACT_APP_VERSION || '1.0.0',

    GA_ID: process.env.REACT_APP_GA_ID,

    isDevelopment() {
        return this.ENV === 'development';
    },

    isStaging() {
        return this.ENV === 'staging';
    },

    isProduction() {
        return this.ENV === 'production';
    },

    // Add environment-specific configurations
    apiConfig: {
        timeout: process.env.REACT_APP_API_TIMEOUT || 30000,
        retryAttempts: process.env.REACT_APP_API_RETRY_ATTEMPTS || 3,
    },

    // Feature flags
    features: {
        enableGoogleLogin: process.env.REACT_APP_ENABLE_GOOGLE_LOGIN === 'true',
        enableNotifications: process.env.REACT_APP_ENABLE_NOTIFICATIONS === 'true',
    }
};

export default env; 