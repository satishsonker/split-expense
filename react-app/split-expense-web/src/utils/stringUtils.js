/**
 * Capitalizes the first letter of each word in a string
 * @param {string} text - The text to capitalize
 * @param {boolean} [forceLowerRest=true] - Whether to force the rest of each word to lowercase
 * @returns {string} The capitalized text
 */
export const capitalizeText = (text, forceLowerRest = true) => {
    if (!text) return '';
    
    return text.split(' ')
        .map(word => {
            if (!word) return '';
            const firstChar = word.charAt(0).toUpperCase();
            const restOfWord = forceLowerRest ? word.slice(1).toLowerCase() : word.slice(1);
            return firstChar + restOfWord;
        })
        .join(' ');
};

/**
 * Capitalizes only the first letter of a string
 * @param {string} text - The text to capitalize
 * @returns {string} The capitalized text
 */
export const capitalizeFirstLetter = (text) => {
    if (!text) return '';
    return text.charAt(0).toUpperCase() + text.slice(1);
}; 