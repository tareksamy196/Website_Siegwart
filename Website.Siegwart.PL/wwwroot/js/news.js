// news.js — Share Modal + Copy Link
(function () {
    'use strict';

    function init() {

        var shareBtn = document.getElementById('shareBtn');
        var shareModalEl = document.getElementById('shareModal');
        var copyBtn = document.getElementById('copyLinkBtn');
        var copyInput = document.getElementById('shareLinkInput');

        // Fill URL input
        if (copyInput) {
            copyInput.value = window.location.href;
        }

        // Open modal
        if (shareBtn && shareModalEl) {
            shareBtn.addEventListener('click', function () {
                if (typeof bootstrap !== 'undefined') {
                    bootstrap.Modal.getOrCreateInstance(shareModalEl).show();
                }
            });
        }

        // Copy link
        if (copyBtn && copyInput) {
            copyBtn.addEventListener('click', function () {
                navigator.clipboard.writeText(copyInput.value).then(function () {
                    var original = copyBtn.innerHTML;
                    copyBtn.innerHTML = '<i class="bi bi-check-lg"></i> ' +
                        (document.documentElement.lang === 'ar' ? 'تم النسخ' : 'Copied!');
                    copyBtn.style.background = '#16a34a';
                    setTimeout(function () {
                        copyBtn.innerHTML = original;
                        copyBtn.style.background = '';
                    }, 2200);
                }).catch(function () {
                    copyInput.select();
                    document.execCommand('copy');
                });
            });
        }

        // Social share links
        var currentUrl = encodeURIComponent(window.location.href);
        var titleEl = document.querySelector('.article-title');
        var articleTitle = encodeURIComponent(titleEl ? titleEl.textContent.trim() : document.title);

        document.querySelectorAll('[data-share]').forEach(function (el) {
            el.addEventListener('click', function (e) {
                e.preventDefault();
                var map = {
                    facebook: 'https://www.facebook.com/sharer/sharer.php?u=' + currentUrl,
                    twitter: 'https://twitter.com/intent/tweet?url=' + currentUrl + '&text=' + articleTitle,
                    linkedin: 'https://www.linkedin.com/sharing/share-offsite/?url=' + currentUrl,
                    whatsapp: 'https://wa.me/?text=' + articleTitle + '%20' + currentUrl
                };
                var url = map[this.dataset.share];
                if (url) window.open(url, '_blank', 'width=640,height=460,noopener,noreferrer');
            });
        });

    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();