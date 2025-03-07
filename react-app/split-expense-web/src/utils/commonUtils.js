/**
 * Capitalizes only the first letter of a string
 * @param {string} str - The string to parse as a JSON
 * @returns {object} The JSON object
 */
export const safeJsonParse=(str)=> {
    try {
        return JSON.parse(str);
    } catch (error) {
        console.error("Invalid JSON:", error.message);
        return null; // or return a default object if needed
    }
}