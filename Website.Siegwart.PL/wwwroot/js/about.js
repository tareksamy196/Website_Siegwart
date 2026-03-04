/**
 * ═══════════════════════════════════════════════════════════════════
 * ABOUT PAGE — PROFESSIONAL JAVASCRIPT
 * Egyptian Railway & Concrete Manufacturing Company
 * ═══════════════════════════════════════════════════════════════════
 */

(function () {
    'use strict';

    // ═══════════════════════════════════════════════════════════════
    // CONSTANTS
    // ═══════════════════════════════════════════════════════════════
    const COUNTER_DURATION = 1200;
    const COUNTER_THRESHOLD = 0.5;
    const AOS_CONFIG = {
        duration: 600,
        easing: 'ease-in-out',
        once: true,
        offset: 100
    };

    // ═══════════════════════════════════════════════════════════════
    // INITIALIZATION
    // ═══════════════════════════════════════════════════════════════
    document.addEventListener('DOMContentLoaded', function () {
        initAOS();
        initCounters();
        initLightbox();
    });

    // ═══════════════════════════════════════════════════════════════
    // AOS ANIMATION
    // ═══════════════════════════════════════════════════════════════
    function initAOS() {
        if (typeof AOS !== 'undefined') {
            AOS.init(AOS_CONFIG);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // STAT COUNTERS
    // ═══════════════════════════════════════════════════════════════
    function initCounters() {
        const counters = document.querySelectorAll('.about-stat-number');

        counters.forEach(function (counter) {
            let hasAnimated = false;

            const observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting && !hasAnimated) {
                        hasAnimated = true;
                        animateCounter(counter);
                        observer.unobserve(counter);
                    }
                });
            }, { threshold: COUNTER_THRESHOLD });

            observer.observe(counter);
        });
    }

    function animateCounter(element) {
        const targetText = element.dataset.count;
        const targetNumber = parseInt(targetText.replace(/\D/g, ''), 10);
        const suffix = targetText.replace(/\d/g, '');

        if (isNaN(targetNumber) || targetNumber === 0) {
            element.textContent = targetText;
            return;
        }

        let currentNumber = 0;
        const increment = Math.ceil(targetNumber / (COUNTER_DURATION / 16));

        function updateCounter() {
            currentNumber += increment;

            if (currentNumber >= targetNumber) {
                element.textContent = targetText;
                return;
            }

            element.textContent = Math.floor(currentNumber) + suffix;
            requestAnimationFrame(updateCounter);
        }

        requestAnimationFrame(updateCounter);
    }

    // ═══════════════════════════════════════════════════════════════
    // CERTIFICATE LIGHTBOX
    // ═══════════════════════════════════════════════════════════════
    function initLightbox() {
        const lightbox = document.getElementById('aboutLightbox');
        const lightboxImage = document.getElementById('aboutLightboxImage');
        const closeButton = document.querySelector('.about-lightbox-close');
        const certCards = document.querySelectorAll('.about-cert-card');

        if (!lightbox || !lightboxImage || !closeButton) {
            return;
        }

        // Open lightbox when clicking on certificate cards
        certCards.forEach(function (card) {
            const img = card.querySelector('.about-cert-image img');
            if (!img) return;

            card.addEventListener('click', function () {
                lightboxImage.src = img.src;
                lightboxImage.alt = img.alt;
                openLightbox();
            });

            // Keyboard support
            card.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    lightboxImage.src = img.src;
                    lightboxImage.alt = img.alt;
                    openLightbox();
                }
            });
        });

        // Close button
        closeButton.addEventListener('click', closeLightbox);

        // Click outside to close
        lightbox.addEventListener('click', function (e) {
            if (e.target === lightbox) {
                closeLightbox();
            }
        });

        // Escape key to close
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && lightbox.classList.contains('active')) {
                closeLightbox();
            }
        });

        function openLightbox() {
            lightbox.classList.add('active');
            document.body.style.overflow = 'hidden';
            closeButton.focus();
        }

        function closeLightbox() {
            lightbox.classList.remove('active');
            document.body.style.overflow = '';
        }
    }

})();