document.addEventListener('DOMContentLoaded', () => {
    const html = document.documentElement;
    const btn  = document.getElementById('theme-toggle');

    function apply(dark) {
        html.classList.toggle('dark', dark);
        localStorage.setItem('theme', dark ? 'dark' : 'light');
        html.style.colorScheme = dark ? 'dark' : 'light';
        if (btn) btn.setAttribute('aria-pressed', String(dark));
    }

    const saved = localStorage.getItem('theme');
    const isDark = saved ? (saved === 'dark') : html.classList.contains('dark');
    if (btn) btn.setAttribute('aria-pressed', String(isDark));

    btn?.addEventListener('click', () => apply(!html.classList.contains('dark')));

    const mql = window.matchMedia('(prefers-color-scheme: dark)');
    mql.addEventListener?.('change', e => {
        if (!localStorage.getItem('theme')) apply(e.matches);
    });
});
