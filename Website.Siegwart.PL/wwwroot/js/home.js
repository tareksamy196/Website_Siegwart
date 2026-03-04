// ================================================================================
// HOME PAGE JAVASCRIPT - CLEAN & MODULAR
// 
// Table of Contents:
// 1. Stats Counter Animation
// 2. News Sharing
// 3. Contact Panel
// 4. Smooth Scroll
// 5. Lazy Load Images
// 6. Keyboard Support
// 7. Utilities
// 8. Initialization
// ================================================================================

(function () {
    'use strict';

    // ============================================================================
    // 1. STATS COUNTER ANIMATION
    // ============================================================================
    const StatsCounter = {
        init() {
            const statsSection = document.querySelector('.sig-stats-grid');
            if (!statsSection) return;

            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting && !entry.target.dataset.counted) {
                        entry.target.dataset.counted = 'true';
                        this.animateAll(entry.target);
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.5 });

            observer.observe(statsSection);
        },

        animateAll(container) {
            const counters = container.querySelectorAll('.sig-stat-num');
            counters.forEach(counter => {
                const target = parseInt(counter.dataset.count || '0', 10);
                this.animateSingle(counter, target);
            });
        },

        animateSingle(element, target) {
            const duration = 2000;
            const start = performance.now();

            const update = (currentTime) => {
                const elapsed = currentTime - start;
                const progress = Math.min(elapsed / duration, 1);
                const eased = 1 - Math.pow(1 - progress, 3); // Ease out cubic
                const current = Math.floor(eased * target);

                element.textContent = current.toLocaleString();

                if (progress < 1) {
                    requestAnimationFrame(update);
                } else {
                    element.textContent = target.toLocaleString();
                }
            };

            requestAnimationFrame(update);
        }
    };

    // ============================================================================
    // 2. NEWS SHARING
    // ============================================================================
    const NewsShare = {
        init() {
            const shareButtons = document.querySelectorAll('.js-share-news');
            shareButtons.forEach(button => {
                button.addEventListener('click', this.handleShare.bind(this));
            });
        },

        handleShare(e) {
            e.preventDefault();

            const newsId = e.currentTarget.dataset.newsId;
            const newsTitle = e.currentTarget.dataset.newsTitle;
            const newsUrl = `${window.location.href.split('#')[0]}#news-${newsId}`;

            if (navigator.share) {
                navigator.share({
                    title: 'Siegwart News',
                    text: newsTitle,
                    url: newsUrl
                }).catch(err => console.log('Error sharing:', err));
            } else {
                Utils.copyToClipboard(newsUrl);
                Utils.showNotification('Link copied to clipboard!');
            }
        }
    };

    // ============================================================================
    // 3. CONTACT PANEL
    // ============================================================================
    const ContactPanel = {
        init() {
            const triggers = document.querySelectorAll('.js-open-contact');
            triggers.forEach(trigger => {
                trigger.addEventListener('click', this.open.bind(this));
            });
        },

        open(e) {
            e.preventDefault();

            const panel = document.getElementById('contactPanel');
            if (!panel) {
                console.warn('Contact panel not found');
                return;
            }

            panel.classList.add('is-open');
            document.body.classList.add('panel-open');

            // Focus first input
            const firstInput = panel.querySelector('input, textarea');
            if (firstInput) {
                setTimeout(() => firstInput.focus(), 300);
            }
        },

        close() {
            const panel = document.getElementById('contactPanel');
            if (panel) {
                panel.classList.remove('is-open');
                document.body.classList.remove('panel-open');
            }
        }
    };

    // ============================================================================
    // 4. SMOOTH SCROLL
    // ============================================================================
    const SmoothScroll = {
        init() {
            this.initScrollHint();
            this.initAnchorLinks();
        },

        initScrollHint() {
            const scrollButton = document.querySelector('.js-scroll-hint');
            if (scrollButton) {
                scrollButton.addEventListener('click', () => {
                    const firstSection = document.querySelector('.sig-section');
                    if (firstSection) {
                        firstSection.scrollIntoView({ behavior: 'smooth' });
                    }
                });
            }
        },

        initAnchorLinks() {
            document.querySelectorAll('a[href^="#"]').forEach(anchor => {
                anchor.addEventListener('click', function (e) {
                    const href = this.getAttribute('href');
                    if (href !== '#' && href.length > 1) {
                        e.preventDefault();
                        const target = document.querySelector(href);
                        if (target) {
                            target.scrollIntoView({ behavior: 'smooth' });
                        }
                    }
                });
            });
        }
    };

    // ============================================================================
    // 5. LAZY LOAD IMAGES
    // ============================================================================
    const LazyLoad = {
        init() {
            if (!('IntersectionObserver' in window)) return;

            const imageObserver = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        if (img.dataset.src) {
                            img.src = img.dataset.src;
                            img.removeAttribute('data-src');
                        }
                        imageObserver.unobserve(img);
                    }
                });
            });

            document.querySelectorAll('img[loading="lazy"]').forEach(img => {
                imageObserver.observe(img);
            });
        }
    };

    // ============================================================================
    // 6. KEYBOARD SUPPORT
    // ============================================================================
    const KeyboardSupport = {
        init() {
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    ContactPanel.close();
                }
            });
        }
    };

    // ============================================================================
    // 7. UTILITIES
    // ============================================================================
    const Utils = {
        copyToClipboard(text) {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(text);
            } else {
                // Fallback for older browsers
                const textarea = document.createElement('textarea');
                textarea.value = text;
                textarea.style.position = 'fixed';
                textarea.style.opacity = '0';
                document.body.appendChild(textarea);
                textarea.select();
                document.execCommand('copy');
                document.body.removeChild(textarea);
            }
        },

        showNotification(message, duration = 3000) {
            const notification = document.createElement('div');
            notification.className = 'home-notification';
            notification.textContent = message;
            notification.setAttribute('role', 'alert');
            notification.setAttribute('aria-live', 'polite');

            Object.assign(notification.style, {
                position: 'fixed',
                bottom: '24px',
                right: '24px',
                padding: '16px 24px',
                background: 'var(--blue, #0096ff)',
                color: 'white',
                borderRadius: '8px',
                boxShadow: '0 4px 16px rgba(0, 0, 0, 0.15)',
                fontWeight: '600',
                zIndex: '9999',
                opacity: '0',
                transform: 'translateX(400px)',
                transition: 'opacity 0.3s ease, transform 0.3s ease'
            });

            document.body.appendChild(notification);

            // Trigger animation
            requestAnimationFrame(() => {
                notification.style.opacity = '1';
                notification.style.transform = 'translateX(0)';
            });

            // Remove after duration
            setTimeout(() => {
                notification.style.opacity = '0';
                notification.style.transform = 'translateX(400px)';

                setTimeout(() => {
                    if (notification.parentNode) {
                        document.body.removeChild(notification);
                    }
                }, 300);
            }, duration);
        }
    };

    // ============================================================================
    // 8. INITIALIZATION
    // ============================================================================
    function init() {
        console.log('Home page initialized');

        StatsCounter.init();
        NewsShare.init();
        ContactPanel.init();
        SmoothScroll.init();
        LazyLoad.init();
        KeyboardSupport.init();
    }

    // Start when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Expose public API
    window.HomeApp = {
        openContact: ContactPanel.open.bind(ContactPanel),
        closeContact: ContactPanel.close.bind(ContactPanel),
        showNotification: Utils.showNotification
    };

})();