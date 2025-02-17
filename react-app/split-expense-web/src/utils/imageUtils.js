import env from '../config/env.config';

export const getImageUrl = (path) => {
    let baseUrl = env.BASE_URL_API;
    if (!path || path === undefined || baseUrl === undefined || baseUrl === null) return null;
    // Remove leading slash if it exists in both API_URL and path

    baseUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    const imagePath = path.startsWith('/') ? path : `/${path}`;
    var fullImagePath = `${baseUrl}${imagePath}`;
    return fullImagePath;
}; 