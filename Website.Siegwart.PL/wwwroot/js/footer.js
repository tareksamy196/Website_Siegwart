// footer.js - lazy load iframe + init AOS if available
(function () {
    'use strict';

    // Load AOS if elements using data-aos exist and AOS not loaded
    function initAOSIfNeeded() {
        if (!document.querySelector('[data-aos]')) return Promise.resolve();
        if (window.AOS) {
            window.AOS.init({ duration: 800, easing: 'ease-in-out', once: true, offset: 100 });
            return Promise.resolve();
        }
        // Dynamically load CSS + script
        return new Promise((resolve) => {
            if (!document.querySelector('link[href*="aos@2.3.1"]')) {
                var l = document.createElement('link');
                l.rel = 'stylesheet';
                l.href = 'https://unpkg.com/aos@2.3.1/dist/aos.css';
                document.head.appendChild(l);
            }
            if (!document.querySelector('script[src*="aos@2.3.1"]')) {
                var s = document.createElement('script');
                s.src = 'https://unpkg.com/aos@2.3.1/dist/aos.js';
                s.onload = function () {
                    if (window.AOS && typeof window.AOS.init === 'function') {
                        window.AOS.init({ duration: 800, easing: 'ease-in-out', once: true, offset: 100 });
                    }
                    resolve();
                };
                document.body.appendChild(s);
            } else {
                // script exists but not initialized
                if (window.AOS && typeof window.AOS.init === 'function') {
                    window.AOS.init({ duration: 800, easing: 'ease-in-out', once: true, offset: 100 });
                }
                resolve();
            }
        });
    }

    function lazyLoadIframes() {
        const frames = Array.from(document.querySelectorAll('.footer-map iframe[data-src]'));
        if (!frames.length) return;
        if ('IntersectionObserver' in window) {
            const io = new IntersectionObserver((entries, obs) => {
                entries.forEach(en => {
                    if (!en.isIntersecting) return;
                    const f = en.target;
                    f.src = f.dataset.src;
                    f.removeAttribute('data-src');
                    obs.unobserve(f);
                });
            }, { rootMargin: '300px' });
            frames.forEach(f => io.observe(f));
        } else {
            frames.forEach(f => {
                f.src = f.dataset.src;
                f.removeAttribute('data-src');
            });
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        initAOSIfNeeded();
        // Support both data-src and data-src attr used above
        const f = document.querySelector('.footer-map iframe[data-src]');
        if (f) lazyLoadIframes();
        else {
            // If iframe has src already, do nothing
        }
    });

})();