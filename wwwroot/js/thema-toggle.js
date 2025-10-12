document.addEventListener('DOMContentLoaded', () => {
    const html = document.documentElement;
    const btn = document.getElementById('theme-toggle');
    const iconSun  = document.getElementById('icon-sun');  // sol
    const iconMoon = document.getElementById('icon-moon'); // lua

    function apply(dark) {
        html.classList.toggle('dark', dark);
        localStorage.setItem('theme', dark ? 'dark' : 'light');

        if (iconSun && iconMoon) {
            iconSun.style.display  = dark ? 'inline' : 'none';
            iconMoon.style.display = dark ? 'none'   : 'inline';
        }

        if (btn) btn.setAttribute('aria-pressed', String(dark));
    }

    const saved = localStorage.getItem('theme');
    if (saved === 'dark' || saved === 'light') {
        apply(saved === 'dark');
    } else {
        apply(window.matchMedia('(prefers-color-scheme: dark)').matches);
    }

    // Toggle ao clicar
    btn?.addEventListener('click', () => {
        apply(!html.classList.contains('dark'));
    });

    // Se quiser reagir à mudança do SO *apenas quando não houver escolha do usuário*:
    const mql = window.matchMedia('(prefers-color-scheme: dark)');
    const onOsChange = e => {
        if (!localStorage.getItem('theme')) apply(e.matches);
    };
    mql.addEventListener?.('change', onOsChange);
});
