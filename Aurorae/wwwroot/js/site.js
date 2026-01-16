/*
 * bootstrap-auto-dark-mode (modified)
 *
 * Author and copyright: Stefan Haack (https://shaack.com)
 * Repository: https://github.com/shaack/bootstrap-auto-dark-mode)
 * License: MIT, see file 'LICENSE'
 */

{
    const htmlElement = document.querySelector('html');
    if (htmlElement.getAttribute('data-bs-theme') == 'auto') {
        function updateTheme() {
            document.querySelector("html").setAttribute('data-bs-theme',
                window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
        }

        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', updateTheme);
        updateTheme();
    }
}


/*
 * hide bootstrap navbar when scrolling (modified)
 *
 * Source - https://stackoverflow.com/a/45935816
 * Posted by Tomer Shay, modified by community. See post 'Timeline' for change history
 * Retrieved 2026-01-16, License - CC BY-SA 3.0
 */

document.addEventListener("DOMContentLoaded", function () {
    const nav = document.querySelector("nav");

    const bannerHeight = nav.offsetHeight;
    let lastScrollTop = 0;

    window.addEventListener("scroll", function () {
        const currScrollTop = window.pageYOffset || document.documentElement.scrollTop;

        if (currScrollTop < bannerHeight) {
            nav.style.transform = "translateY(0)";
        } else if (currScrollTop > lastScrollTop) {
            nav.style.transform = "translateY(-150%)";
        } else {
            nav.style.transform = "translateY(0)";
        }

        lastScrollTop = currScrollTop;
    });
});
