import axios from 'axios';
import env from '../config/env.config';
import { toast } from 'react-toastify';
import { safeJsonParse } from '../utils/commonUtils'
import { AUTH_PATHS } from '../constants/apiPaths';

class ApiService {
    constructor() {
        this.client = axios.create({
            baseURL: env.API_URL,
            timeout: 30000,
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Access-Control-Allow-Origin': "*",
            }
        });

        // Add request interceptor
        this.client.interceptors.request.use(
            this.handleRequest,
            this.handleRequestError
        );

        // Add response interceptor
        this.client.interceptors.response.use(
            this.handleResponse,
            this.handleResponseError
        );
    }

    // Request interceptor
    handleRequest = (config) => {
        // Don't set Content-Type for FormData, let browser set it with boundary
        if (config.data instanceof FormData) {
            delete config.headers['Content-Type'];
        }

        let excludedApiPathForAccessToken = [AUTH_PATHS.LOGIN]
        if (!excludedApiPathForAccessToken.includes(config.url)) {
            const user = JSON.parse(localStorage.getItem('user'));
            const tokenData = JSON.parse(localStorage.getItem('token') ?? "{}");
            if (user?.userId) {
                config.headers['userId'] = user.userId;
            }

            if (tokenData?.token) {
                config.headers['Authorization'] = `Bearer ${tokenData.token}`;
            }
        }
        if (env.isDevelopment()) {
            console.log('API Request:', {
                url: config.url,
                method: config.method,
                data: config.data,
                headers: config.headers
            });
        }

        return config;
    };

    handleRequestError = (error) => {
        console.error('Request Error:', error);
        return Promise.reject(error);
    };

    // Response interceptor
    handleResponse = (response) => {
        if (env.isDevelopment()) {
            console.log('API Response:', {
                url: response.config.url,
                status: response.status,
                data: response.data
            });
        }
        return response.data;
    };

    handleResponseError = (error) => {
        if (env.isDevelopment()) {
            console.error('Response Error:', {
                url: error.config?.url,
                status: error.response?.status,
                message: error.message,
                response: error.response?.data
            });
        }

        if (error.response?.status === 401) {
            localStorage.removeItem('user');
            window.location.href = '/login';
            return Promise.reject(new Error('Unauthorized access'));
        }

        const errorMessage = error.response?.data?.message || 'Something went wrong';
        toast.error(errorMessage);
        return Promise.reject(error);
    };

    // Wrapper methods
    async get(url, params = {}, config = {}) {
        try {
            return await this.client.get(url, { ...config, params });
        } catch (error) {
            throw error;
        }
    }

    async post(url, data = {}, config = {}) {
        try {
            return await this.client.post(url, data, config);
        } catch (error) {
            throw error;
        }
    }

    async put(url, data = {}, config = {}) {
        try {
            return await this.client.put(url, data, config);
        } catch (error) {
            throw error;
        }
    }

    async delete(url, config = {}) {
        try {
            return await this.client.delete(url, config);
        } catch (error) {
            throw error;
        }
    }

    async patch(url, data = {}, config = {}) {
        try {
            return await this.client.patch(url, data, config);
        } catch (error) {
            throw error;
        }
    }
}

export const apiService = new ApiService(); 