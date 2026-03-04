/* product-details.js — same as before, no changes needed */
(() => {
    'use strict';

    // ── Reveal animation ──────────────────────────────────────────
    const revealEl = document.querySelector('.pd--reveal');
    if (revealEl) {
        const io = new IntersectionObserver(([entry]) => {
            if (entry.isIntersecting) {
                revealEl.classList.add('pd--visible');
                io.disconnect();
            }
        }, { threshold: 0.06 });
        io.observe(revealEl);
    }

    // ── Lightbox ─────────────────────────────────────────────────
    const lightbox = document.getElementById('imageLightbox');
    const lightboxImg = document.getElementById('lightboxImage');
    const closeLightbox = document.getElementById('closeLightbox');
    const mainImage = document.getElementById('mainProductImage');
    const zoomBtn = document.getElementById('zoomBtn');
    const pdFigure = document.querySelector('.pd-figure');

    const openLightbox = () => {
        if (!mainImage || !lightbox) return;
        lightboxImg.src = mainImage.src;
        lightbox.classList.add('active');
        document.body.style.overflow = 'hidden';
    };
    const closeLightboxFn = () => {
        lightbox?.classList.remove('active');
        document.body.style.overflow = '';
    };

    zoomBtn?.addEventListener('click', openLightbox);
    pdFigure?.addEventListener('click', openLightbox);
    closeLightbox?.addEventListener('click', closeLightboxFn);
    lightbox?.addEventListener('click', e => { if (e.target === lightbox) closeLightboxFn(); });
    document.addEventListener('keydown', e => {
        if (e.key === 'Escape') closeLightboxFn();
    });

    // ── Print ─────────────────────────────────────────────────────
    document.getElementById('printBtn')?.addEventListener('click', () => window.print());

    // ── Share modal ───────────────────────────────────────────────
    const shareModalEl = document.getElementById('shareModal');
    const shareBtn = document.getElementById('shareBtn');

    if (shareBtn && shareModalEl && typeof bootstrap !== 'undefined') {
        const shareModal = new bootstrap.Modal(shareModalEl);
        shareBtn.addEventListener('click', () => shareModal.show());
    }

    // ── Copy link ─────────────────────────────────────────────────
    const copyBtn = document.getElementById('copyLinkBtn');
    const copyInput = document.getElementById('shareLink');

    copyBtn?.addEventListener('click', async () => {
        if (!copyInput) return;
        try {
            await navigator.clipboard.writeText(copyInput.value);
        } catch {
            copyInput.select();
            document.execCommand('copy');
        }
        const originalHTML = copyBtn.innerHTML;
        copyBtn.innerHTML = `
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none">
                <path d="M20 6L9 17l-5-5" stroke="currentColor" stroke-width="2.2"
                      stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            Copied!`;
        copyBtn.style.background = '#16a34a';
        setTimeout(() => {
            copyBtn.innerHTML = originalHTML;
            copyBtn.style.background = '';
        }, 2200);
    });
})();