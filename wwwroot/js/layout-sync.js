document.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.getElementById('hs-sidebar-content-push-to-mini-sidebar');
    if (!sidebar) return;

    const shiftEls = document.querySelectorAll('[data-layout-shift]');

    function setShiftTransition(enabled) {
        shiftEls.forEach(el => {
            el.style.transition = enabled ? '' : 'margin-inline-start 0s linear';
            el.style.willChange = enabled ? '' : 'margin-inline-start';
        });
    }

    function applyShiftOnce() {
        const isLg = matchMedia('(min-width:1024px)').matches;
        const w = isLg ? sidebar.getBoundingClientRect().width : 0;
        shiftEls.forEach(el => { el.style.marginInlineStart = w + 'px'; });
    }

    let rafId = 0;
    function startSync() {
        cancelAnimationFrame(rafId);
        setShiftTransition(false);
        const step = () => { applyShiftOnce(); rafId = requestAnimationFrame(step); };
        step();
    }
    function stopSync() {
        cancelAnimationFrame(rafId);
        applyShiftOnce();
        setShiftTransition(true);
    }

    applyShiftOnce();
    new ResizeObserver(applyShiftOnce).observe(sidebar);
    new MutationObserver(applyShiftOnce).observe(sidebar, { attributes: true, attributeFilter: ['class','style'] });

    sidebar.addEventListener('transitionstart', (e) => {
        if (e.propertyName === 'width' || e.propertyName === 'transform') startSync();
    });
    sidebar.addEventListener('transitionend', (e) => {
        if (e.propertyName === 'width' || e.propertyName === 'transform') stopSync();
    });

    document.addEventListener('click', (e) => {
        if (e.target.closest('[data-hs-overlay-minifier], [data-hs-overlay]')) {
            setTimeout(() => { applyShiftOnce(); startSync(); }, 0);
            setTimeout(stopSync, 600);
        }
    });

    window.HSStaticMethods?.autoInit?.();
});
