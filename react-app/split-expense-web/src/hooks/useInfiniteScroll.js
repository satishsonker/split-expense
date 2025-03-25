import { useRef, useEffect } from 'react';

const useInfiniteScroll = (callback) => {
    const observer = useRef();

    const infiniteScrollRef = useRef(null);

    useEffect(() => {
        const currentElement = infiniteScrollRef.current;
        
        const options = {
            root: null,
            rootMargin: '20px',
            threshold: 1.0
        };

        observer.current = new IntersectionObserver((entries) => {
            const first = entries[0];
            if (first.isIntersecting) {
                callback();
            }
        }, options);

        if (currentElement) {
            observer.current.observe(currentElement);
        }

        return () => {
            if (currentElement) {
                observer.current.unobserve(currentElement);
            }
        };
    }, [callback]);

    return [infiniteScrollRef];
};

export default useInfiniteScroll; 