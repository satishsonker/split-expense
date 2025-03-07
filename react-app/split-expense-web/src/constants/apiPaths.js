import env from '../config/env.config';

export const API_BASE_URL = env.API_URL;

export const GROUP_PATHS = {
    CREATE: `${API_BASE_URL}/group/create`,
    LIST: `${API_BASE_URL}/group/get/all`,
    GET_RECENTS: `${API_BASE_URL}/group/get/recent`,
    UPDATE: `${API_BASE_URL}/group/update`,
    DELETE: `${API_BASE_URL}/group/delete`,
    SEARCH: `${API_BASE_URL}/group/search`,
    MEMBERS: `${API_BASE_URL}/contacts/get/all?pageno=1&pageSize=500`
};

export const GROUP_TYPES_PATHS={
    CREATE: `${API_BASE_URL}/grouptype/create`,
    UPDATE: `${API_BASE_URL}/grouptype/update`,
    DELETE: `${API_BASE_URL}/grouptype/delete/{id}`,
    GET_BY_ID: `${API_BASE_URL}/grouptype/{id}`,
    LIST: `${API_BASE_URL}/grouptype/get/all`,
    SEARCH: `${API_BASE_URL}/grouptype/search`
}

export const AUTH_PATHS = {
    LOGIN: `${API_BASE_URL}/auth/login`,
    UPDATE_PROFILE: `${API_BASE_URL}/auth/update`,
    LOGOUT: `${API_BASE_URL}/auth/logout`,
    REGISTER: `${API_BASE_URL}/auth/register`,
    GOOGLE_LOGIN: `${API_BASE_URL}/auth/google`,
    FORGOT_PASSWORD: `${API_BASE_URL}/auth/forgot-password`,
    FORGOT_USERNAME: `${API_BASE_URL}/auth/forgot-username`,
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