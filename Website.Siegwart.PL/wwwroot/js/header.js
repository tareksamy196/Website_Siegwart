// ================================================================================
// HEADER JAVASCRIPT - MODULAR & CLEAN
// 
// Table of Contents:
// 1. Scroll Behavior
// 2. Desktop Dropdowns
// 3. Mobile Menu
// 4. Contact Panel
// 5. Keyboard Navigation
// 6. Utilities
// 7. Initialization
// ================================================================================

(function () {
    'use strict';

    // ============================================================================
    // 1. SCROLL BEHAVIOR
    // ============================================================================
    const ScrollBehavior = {
        header: null,
        ticking: false,

        init() {
            this.header = document.getElementById('mainHeader');
            if (!this.header) return;

            window.addEventListener('scroll', () => this.onScroll());
        },

        onScroll() {
            if (!this.ticking) {
                requestAnimationFrame(() => this.applyScrollClass());
                this.ticking = true;
            }
        },

        applyScrollClass() {
            if (!this.header) return;
            this.header.classList.toggle('scrolled', window.scrollY > 20);
            this.ticking = false;
        }
    };

    // ============================================================================
    // 2. DESKTOP DROPDOWNS
    // ============================================================================
    const DesktopDropdowns = {
        dropdowns: [],

        init() {
            this.dropdowns = Array.from(document.querySelectorAll('[data-dd]'));
            this.dropdowns.forEach(dropdown => this.setupDropdown(dropdown));
            this.setupGlobalClickHandler();
        },

        setupDropdown(dropdown) {
            const button = dropdown.querySelector('.sig-dd-toggle');
            if (!button) return;

            button.addEventListener('click', (e) => this.handleClick(e, dropdown));
            dropdown.addEventListener('mouseenter', () => this.handleMouseEnter(dropdown));
            dropdown.addEventListener('mouseleave', () => this.handleMouseLeave(dropdown));
            dropdown.addEventListener('focusin', () => this.handleFocusIn(dropdown));
            dropdown.addEventListener('focusout', (e) => this.handleFocusOut(dropdown, e));
        },

        handleClick(e, dropdown) {
            e.preventDefault();
            const isOpen = dropdown.classList.contains('open');
            this.closeAll(dropdown);
            this.setOpen(dropdown, !isOpen);
        },

        handleMouseEnter(dropdown) {
            if (this.isDesktop()) {
                this.closeAll(dropdown);
                this.setOpen(dropdown, true);
            }
        },

        handleMouseLeave(dropdown) {
            if (this.isDesktop()) {
                this.setOpen(dropdown, false);
            }
        },

        handleFocusIn(dropdown) {
            if (this.isDesktop()) {
                this.closeAll(dropdown);
                this.setOpen(dropdown, true);
            }
        },

        handleFocusOut(dropdown, e) {
            if (!dropdown.contains(e.relatedTarget)) {
                this.setOpen(dropdown, false);
            }
        },

        setOpen(dropdown, open) {
            dropdown.classList.toggle('open', open);
            const button = dropdown.querySelector('.sig-dd-toggle');
            if (button) {
                button.setAttribute('aria-expanded', String(open));
            }
        },

        closeAll(except = null) {
            this.dropdowns.forEach(dropdown => {
                if (except && dropdown === except) return;
                this.setOpen(dropdown, false);
            });
        },

        setupGlobalClickHandler() {
            document.addEventListener('click', (e) => {
                const isInsideDropdown = this.dropdowns.some(d => d.contains(e.target));
                if (!isInsideDropdown) {
                    this.closeAll();
                }
            });
        },

        isDesktop() {
            return window.matchMedia('(min-width: 1100px)').matches;
        }
    };

    // ============================================================================
    // 3. MOBILE MENU
    // ============================================================================
    const MobileMenu = {
        toggle: null,
        menu: null,
        close: null,
        overlay: null,
        accordions: [],

        init() {
            this.toggle = document.getElementById('mobileToggle');
            this.menu = document.getElementById('mobileMenu');
            this.close = document.getElementById('mobileClose');
            this.overlay = document.getElementById('mobileOverlay');

            if (!this.menu || !this.overlay || !this.toggle) return;

            this.toggle.addEventListener('click', () => this.open());
            this.close?.addEventListener('click', () => this.close());
            this.overlay.addEventListener('click', () => this.closeMenu());

            this.initAccordions();
        },

        initAccordions() {
            this.accordions = Array.from(document.querySelectorAll('[data-mdd]'));
            this.accordions.forEach(button => {
                button.addEventListener('click', () => this.toggleAccordion(button));
            });
        },

        toggleAccordion(button) {
            const panel = button.nextElementSibling;
            const isOpen = panel?.classList.contains('open');

            // Close all accordions
            this.accordions.forEach(btn => {
                const p = btn.nextElementSibling;
                p?.classList.remove('open');
                btn.setAttribute('aria-expanded', 'false');
            });

            // Toggle clicked accordion
            panel?.classList.toggle('open', !isOpen);
            button.setAttribute('aria-expanded', String(!isOpen));
        },

        open() {
            this.setOpen(true);
        },

        closeMenu() {
            this.setOpen(false);
        },

        setOpen(open) {
            if (!this.menu || !this.overlay || !this.toggle) return;

            this.menu.classList.toggle('active', open);
            this.overlay.classList.toggle('active', open);
            this.overlay.setAttribute('aria-hidden', String(!open));
            this.menu.setAttribute('aria-hidden', String(!open));
            this.toggle.setAttribute('aria-expanded', String(open));

            document.body.style.overflow = open ? 'hidden' : '';

            if (open) {
                DesktopDropdowns.closeAll();
            }
        }
    };

    // ============================================================================
    // 4. CONTACT PANEL
    // ============================================================================
    const ContactPanel = {
        panel: null,
        overlay: null,
        openBtn: null,
        openMobileBtn: null,
        closeBtn: null,
        lastFocused: null,

        init() {
            this.panel = document.getElementById('contactPanel');
            this.overlay = document.getElementById('panelOverlay');
            this.openBtn = document.getElementById('openSidePanel');
            this.openMobileBtn = document.getElementById('openSidePanelMobile');
            this.closeBtn = document.getElementById('closePanel');

            if (!this.panel || !this.overlay) return;

            this.openBtn?.addEventListener('click', () => this.open());
            this.openMobileBtn?.addEventListener('click', () => this.open());
            this.closeBtn?.addEventListener('click', () => this.close());
            this.overlay.addEventListener('click', () => this.close());
        },

        open() {
            this.setOpen(true);
        },

        close() {
            this.setOpen(false);
        },

        setOpen(open) {
            if (!this.panel || !this.overlay) return;

            this.panel.classList.toggle('active', open);
            this.overlay.classList.toggle('active', open);
            this.overlay.setAttribute('aria-hidden', String(!open));
            this.panel.setAttribute('aria-hidden', String(!open));
            this.openBtn?.setAttribute('aria-expanded', String(open));

            document.body.style.overflow = open ? 'hidden' : '';

            if (open) {
                MobileMenu.closeMenu();
                this.lastFocused = document.activeElement;
                this.panel.focus();
            } else if (this.lastFocused && typeof this.lastFocused.focus === 'function') {
                this.lastFocused.focus();
            }
        }
    };

    // ============================================================================
    // 5. KEYBOARD NAVIGATION
    // ============================================================================
    const KeyboardNav = {
        init() {
            document.addEventListener('keydown', (e) => this.handleKeydown(e));
        },

        handleKeydown(e) {
            if (e.key === 'Escape') {
                DesktopDropdowns.closeAll();
                MobileMenu.closeMenu();
                ContactPanel.close();
            }
        }
    };

    // ============================================================================
    // 6. UTILITIES
    // ============================================================================
    const Utils = {
        // Reserved for future utility functions
        log(message) {
            if (console && console.log) {
                console.log(`[Header] ${message}`);
            }
        }
    };

    // ============================================================================
    // 7. INITIALIZATION
    // ============================================================================
    function init() {
        Utils.log('Header initialized');

        ScrollBehavior.init();
        DesktopDropdowns.init();
        MobileMenu.init();
        ContactPanel.init();
        KeyboardNav.init();
    }

    // Start when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Expose public API
    window.HeaderApp = {
        openMobileMenu: () => MobileMenu.open(),
        closeMobileMenu: () => MobileMenu.closeMenu(),
        openContactPanel: () => ContactPanel.open(),
        closeContactPanel: () => ContactPanel.close()
    };

})();