/*
 * bootstrap-auto-dark-mode (modified)
 *
 * Source - https://github.com/shaack/bootstrap-auto-dark-mode
 * Author and copyright - Stefan Haack (https://shaack.com)
 * License - MIT
 */

const htmlElement = document.querySelector('html');

function updateTheme() {
    const theme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    document.querySelector("html").setAttribute('data-bs-theme', theme);
}

window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', updateTheme);
updateTheme();


/*
 * hide bootstrap navbar when scrolling (modified)
 *
 * Source - https://stackoverflow.com/a/45935816
 * Posted by Tomer Shay, modified by community
 * Retrieved 2026-01-16, License - CC BY-SA 3.0
 */

const nav = document.querySelector("nav");
const bannerHeight = nav.offsetHeight;
let lastScrollTop = 0;

window.addEventListener("scroll", function () {
    const currScrollTop = window.pageYOffset || document.documentElement.scrollTop;

    if (currScrollTop < bannerHeight)
        showNavbar();
    else if (currScrollTop > lastScrollTop)
        hideNavbar();
    else
        showNavbar();

    lastScrollTop = currScrollTop;
});

function showNavbar() { nav.style.transform = "translateY(0)"; }
function hideNavbar() { nav.style.transform = "translateY(-150%)"; }
