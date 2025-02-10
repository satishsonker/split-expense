import env from '../config/env.config';

export const API_BASE_URL = env.API_URL;

export const GROUP_PATHS = {
    CREATE: `${API_BASE_URL}/group/create`,
    LIST: `${API_BASE_URL}/group/get/all`,
    MEMBERS: `${API_BASE_URL}/users/contacts`,
};

export const AUTH_PATHS = {
    LOGIN: `${API_BASE_URL}/auth/login`,
    REGISTER: `${API_BASE_URL}/auth/register`,
    GOOGLE_LOGIN: `${API_BASE_URL}/auth/google`,
    FORGOT_PASSWORD: `${API_BASE_URL}/auth/forgot-password`,
    RESET_PASSWORD: `${API_BASE_URL}/auth/reset-password`,
}; 