import env from '../config/env.config';

export const API_BASE_URL = env.API_URL;

export const GROUP_PATHS = {
    CREATE: `${API_BASE_URL}/group/create`,
    LIST: `${API_BASE_URL}/group/get/all`,
    UPDATE: `${API_BASE_URL}/group/update`,
    DELETE: `${API_BASE_URL}/group/delete`,
    MEMBERS: `${API_BASE_URL}/contacts/get/all?pageno=1&pageSize=500`,
};

export const AUTH_PATHS = {
    LOGIN: `${API_BASE_URL}/auth/login`,
    REGISTER: `${API_BASE_URL}/auth/register`,
    GOOGLE_LOGIN: `${API_BASE_URL}/auth/google`,
    FORGOT_PASSWORD: `${API_BASE_URL}/auth/forgot-password`,
    RESET_PASSWORD: `${API_BASE_URL}/auth/reset-password`,
};

export const CONTACT_PATHS = {
    LIST: `${API_BASE_URL}/contacts/get/all`,
    SEARCH: `${API_BASE_URL}/contacts/search`,
    SEARCH_USER: `${API_BASE_URL}/contacts/search/user`,
    DELETE: `${API_BASE_URL}/contacts/delete`,
    ADD: `${API_BASE_URL}/contacts/add`,
    CREATE: `${API_BASE_URL}/contacts/create`,
    ADD_IN_CONTACT_LIST:`${API_BASE_URL}/contacts/add/in/contact/list/`,
}; 