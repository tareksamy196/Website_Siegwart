// ═══════════════════════════════════════════════════════════════════════
// TEAM CAROUSEL — NO ANIMATIONS, INSTANT NAVIGATION
// ═══════════════════════════════════════════════════════════════════════

(function () {
    'use strict';

    const TeamCarousel = {
        carousels: [],
        isRTL: document.documentElement.dir === 'rtl',

        // Breakpoints and items per view
        breakpoints: {
            desktop: { min: 1200, itemsPerView: 4 },
            tablet: { min: 768, max: 1199, itemsPerView: 3 },
            mobile: { min: 0, max: 767, itemsPerView: 2 }
        },

        // ═══════════════════════════════════════════════════════════════
        // INITIALIZATION
        // ═══════════════════════════════════════════════════════════════
        init() {
            const containers = document.querySelectorAll('.team-carousel-container');

            if (!containers.length) return;

            containers.forEach(container => {
                this.setupCarousel(container);
            });

            // Handle window resize
            let resizeTimer;
            window.addEventListener('resize', () => {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(() => {
                    this.handleResize();
                }, 250);
            });

            // Keyboard navigation
            document.addEventListener('keydown', (e) => this.handleKeyboard(e));
        },

        setupCarousel(container) {
            const track = container.querySelector('.team-carousel-track');
            const cards = Array.from(track.querySelectorAll('.team-card'));
            const totalCards = parseInt(container.dataset.teamCount) || cards.length;
            const carouselId = track.id;

            const prevBtn = container.querySelector('.carousel-arrow-prev');
            const nextBtn = container.querySelector('.carousel-arrow-next');
            const dotsContainer = container.querySelector('.carousel-dots');

            const carousel = {
                container,
                track,
                cards,
                totalCards,
                carouselId,
                prevBtn,
                nextBtn,
                dotsContainer,
                currentIndex: 0,
                itemsPerView: this.getItemsPerView()
            };

            this.carousels.push(carousel);

            // Setup event listeners
            if (prevBtn) {
                prevBtn.addEventListener('click', () => this.navigate(carousel, 'prev'));
            }
            if (nextBtn) {
                nextBtn.addEventListener('click', () => this.navigate(carousel, 'next'));
            }

            // Touch support
            this.setupTouch(carousel);

            // Initial render
            this.updateCarousel(carousel);
        },

        // ═══════════════════════════════════════════════════════════════
        // RESPONSIVE LOGIC
        // ═══════════════════════════════════════════════════════════════
        getItemsPerView() {
            const width = window.innerWidth;

            if (width >= this.breakpoints.desktop.min) {
                return this.breakpoints.desktop.itemsPerView;
            } else if (width >= this.breakpoints.tablet.min) {
                return this.breakpoints.tablet.itemsPerView;
            } else {
                return this.breakpoints.mobile.itemsPerView;
            }
        },

        handleResize() {
            this.carousels.forEach(carousel => {
                const newItemsPerView = this.getItemsPerView();

                if (carousel.itemsPerView !== newItemsPerView) {
                    carousel.itemsPerView = newItemsPerView;
                    carousel.currentIndex = 0;
                    this.updateCarousel(carousel);
                }
            });
        },

        // ═══════════════════════════════════════════════════════════════
        // NAVIGATION
        // ═══════════════════════════════════════════════════════════════
        navigate(carousel, direction) {
            const maxIndex = Math.max(0, carousel.totalCards - carousel.itemsPerView);
            let newIndex = carousel.currentIndex;

            if (direction === 'prev') {
                newIndex = Math.max(0, carousel.currentIndex - 1);
            } else {
                newIndex = Math.min(maxIndex, carousel.currentIndex + 1);
            }

            if (newIndex !== carousel.currentIndex) {
                carousel.currentIndex = newIndex;
                this.updateCarousel(carousel);
            }
        },

        // ═══════════════════════════════════════════════════════════════
        // UPDATE CAROUSEL — INSTANT, NO ANIMATION
        // ═══════════════════════════════════════════════════════════════
        updateCarousel(carousel) {
            const needsCarousel = carousel.totalCards > carousel.itemsPerView;

            // Show/hide navigation
            this.updateNavigation(carousel, needsCarousel);

            // Calculate transform
            const cardWidth = 100 / carousel.itemsPerView;
            const offset = -carousel.currentIndex * cardWidth;

            // Apply transform instantly — no transition
            carousel.track.style.transition = 'none';
            carousel.track.style.transform = `translateX(${this.isRTL ? -offset : offset}%)`;

            // Update dots
            if (needsCarousel) {
                this.updateDots(carousel);
            }

            // Update button states
            this.updateButtonStates(carousel);
        },

        // ═══════════════════════════════════════════════════════════════
        // NAVIGATION VISIBILITY
        // ═══════════════════════════════════════════════════════════════
        updateNavigation(carousel, needsCarousel) {
            // Show/hide arrows
            if (carousel.prevBtn) {
                carousel.prevBtn.style.display = needsCarousel ? 'flex' : 'none';
            }
            if (carousel.nextBtn) {
                carousel.nextBtn.style.display = needsCarousel ? 'flex' : 'none';
            }

            // Show/hide dots
            if (carousel.dotsContainer) {
                carousel.dotsContainer.style.display = needsCarousel ? 'flex' : 'none';
            }

            // Update grid layout
            carousel.track.style.gridTemplateColumns = `repeat(${carousel.totalCards}, ${100 / carousel.itemsPerView}%)`;
        },

        updateButtonStates(carousel) {
            const maxIndex = Math.max(0, carousel.totalCards - carousel.itemsPerView);

            if (carousel.prevBtn) {
                carousel.prevBtn.disabled = carousel.currentIndex === 0;
                carousel.prevBtn.classList.toggle('disabled', carousel.currentIndex === 0);
            }

            if (carousel.nextBtn) {
                carousel.nextBtn.disabled = carousel.currentIndex >= maxIndex;
                carousel.nextBtn.classList.toggle('disabled', carousel.currentIndex >= maxIndex);
            }
        },

        // ═══════════════════════════════════════════════════════════════
        // DOTS INDICATOR
        // ═══════════════════════════════════════════════════════════════
        updateDots(carousel) {
            if (!carousel.dotsContainer) return;

            const maxIndex = Math.max(0, carousel.totalCards - carousel.itemsPerView);
            const totalDots = maxIndex + 1;

            // Create dots if needed
            if (carousel.dotsContainer.children.length !== totalDots) {
                carousel.dotsContainer.innerHTML = '';

                for (let i = 0; i <= maxIndex; i++) {
                    const dot = document.createElement('button');
                    dot.className = 'carousel-dot';
                    dot.setAttribute('aria-label', `Go to page ${i + 1}`);
                    dot.addEventListener('click', () => {
                        carousel.currentIndex = i;
                        this.updateCarousel(carousel);
                    });
                    carousel.dotsContainer.appendChild(dot);
                }
            }

            // Update active state
            const dots = carousel.dotsContainer.querySelectorAll('.carousel-dot');
            dots.forEach((dot, index) => {
                dot.classList.toggle('active', index === carousel.currentIndex);
            });
        },

        // ═══════════════════════════════════════════════════════════════
        // TOUCH/SWIPE SUPPORT
        // ═══════════════════════════════════════════════════════════════
        setupTouch(carousel) {
            let touchStartX = 0;
            let touchEndX = 0;

            carousel.track.addEventListener('touchstart', (e) => {
                touchStartX = e.changedTouches[0].screenX;
            }, { passive: true });

            carousel.track.addEventListener('touchend', (e) => {
                touchEndX = e.changedTouches[0].screenX;
                this.handleSwipe(carousel, touchStartX, touchEndX);
            }, { passive: true });
        },

        handleSwipe(carousel, startX, endX) {
            const diff = startX - endX;
            const threshold = 50;

            if (Math.abs(diff) < threshold) return;

            if (diff > 0) {
                this.navigate(carousel, this.isRTL ? 'prev' : 'next');
            } else {
                this.navigate(carousel, this.isRTL ? 'next' : 'prev');
            }
        },

        // ═══════════════════════════════════════════════════════════════
        // KEYBOARD NAVIGATION
        // ═══════════════════════════════════════════════════════════════
        handleKeyboard(e) {
            if (!['ArrowLeft', 'ArrowRight'].includes(e.key)) return;

            const container = e.target.closest('.team-carousel-container');
            if (!container) return;

            const carousel = this.carousels.find(c => c.container === container);
            if (!carousel) return;

            e.preventDefault();

            if (e.key === 'ArrowLeft') {
                this.navigate(carousel, this.isRTL ? 'next' : 'prev');
            } else if (e.key === 'ArrowRight') {
                this.navigate(carousel, this.isRTL ? 'prev' : 'next');
            }
        }
    };

    // Expose public API
    window.TeamCarousel = TeamCarousel;

})();