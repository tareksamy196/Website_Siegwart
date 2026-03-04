/**
 * ==========================================================================
 * CONTACT PAGE - FORM VALIDATION & SUBMISSION
 * Professional contact form handling with validation
 * ==========================================================================
 */

(function () {
    'use strict';

    // ==========================================================================
    // CONFIGURATION
    // ==========================================================================
    const CONFIG = {
        selectors: {
            form: '#contactForm',
            submitBtn: '#contactSubmit',
            btnText: '.contact__button-text',
            btnLoading: '.contact__button-loading',
            inputs: 'input, textarea',
            requiredFields: '[required]',
            alerts: '.contact__alert'
        },
        classes: {
            invalid: 'is-invalid',
            valid: 'is-valid'
        },
        timing: {
            alertAutoHide: 5000
        },
        regex: {
            email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        }
    };

    // ==========================================================================
    // DOM READY
    // ==========================================================================
    document.addEventListener('DOMContentLoaded', initializeContactPage);

    // ==========================================================================
    // INITIALIZATION
    // ==========================================================================
    function initializeContactPage() {
        initializeAOS();
        initializeForm();
        initializeAlerts();
    }

    /**
     * Initialize AOS (Animate On Scroll)
     */
    function initializeAOS() {
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 800,
                easing: 'ease-in-out',
                once: true,
                offset: 100
            });
        }
    }

    /**
     * Initialize Contact Form
     */
    function initializeForm() {
        const form = document.querySelector(CONFIG.selectors.form);
        const submitBtn = document.querySelector(CONFIG.selectors.submitBtn);

        if (!form || !submitBtn) return;

        // Form submit handler
        form.addEventListener('submit', handleFormSubmit);

        // Input validation handlers
        const inputs = form.querySelectorAll(CONFIG.selectors.inputs);
        inputs.forEach(input => {
            input.addEventListener('blur', () => validateField(input));
            input.addEventListener('input', () => {
                if (input.classList.contains(CONFIG.classes.invalid)) {
                    validateField(input);
                }
            });
        });
    }

    /**
     * Initialize Alert Auto-Hide
     */
    function initializeAlerts() {
        const alerts = document.querySelectorAll(CONFIG.selectors.alerts);
        alerts.forEach(alert => {
            setTimeout(() => {
                if (typeof bootstrap !== 'undefined') {
                    const bsAlert = new bootstrap.Alert(alert);
                    bsAlert.close();
                }
            }, CONFIG.timing.alertAutoHide);
        });
    }

    // ==========================================================================
    // FORM HANDLING
    // ==========================================================================

    /**
     * Handle Form Submission
     * @param {Event} e - Submit event
     */
    function handleFormSubmit(e) {
        e.preventDefault();

        if (validateForm()) {
            submitForm();
        }
    }

    /**
     * Validate Entire Form
     * @returns {boolean} - Form validity
     */
    function validateForm() {
        const form = document.querySelector(CONFIG.selectors.form);
        const requiredFields = form.querySelectorAll(CONFIG.selectors.requiredFields);
        let isValid = true;

        requiredFields.forEach(field => {
            if (!validateField(field)) {
                isValid = false;
            }
        });

        // Focus first invalid field
        if (!isValid) {
            const firstInvalid = form.querySelector(`.${CONFIG.classes.invalid}`);
            if (firstInvalid) {
                firstInvalid.focus();
            }
        }

        return isValid;
    }

    /**
     * Validate Individual Field
     * @param {HTMLElement} field - Input element
     * @returns {boolean} - Field validity
     */
    function validateField(field) {
        const value = field.value.trim();
        let isValid = true;

        // Required validation
        if (field.hasAttribute('required')) {
            if (!value) {
                isValid = false;
            }
        }

        // Email validation
        if (field.type === 'email' && value) {
            if (!CONFIG.regex.email.test(value)) {
                isValid = false;
            }
        }

        // Update UI
        updateFieldUI(field, isValid);

        return isValid;
    }

    /**
     * Update Field UI State
     * @param {HTMLElement} field - Input element
     * @param {boolean} isValid - Validation result
     */
    function updateFieldUI(field, isValid) {
        if (isValid) {
            field.classList.remove(CONFIG.classes.invalid);
            field.classList.add(CONFIG.classes.valid);
        } else {
            field.classList.remove(CONFIG.classes.valid);
            field.classList.add(CONFIG.classes.invalid);
        }
    }

    /**
     * Submit Form
     */
    function submitForm() {
        const form = document.querySelector(CONFIG.selectors.form);
        const submitBtn = document.querySelector(CONFIG.selectors.submitBtn);
        const btnText = submitBtn.querySelector(CONFIG.selectors.btnText);
        const btnLoading = submitBtn.querySelector(CONFIG.selectors.btnLoading);

        // Update button state
        submitBtn.disabled = true;
        btnText.style.display = 'none';
        btnLoading.style.display = 'flex';

        // Submit form
        form.submit();
    }

})();