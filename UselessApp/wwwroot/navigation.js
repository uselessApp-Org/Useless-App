// Mobile shell controls use native checkboxes so they work before Blazor connects.
(() => {
    if (window.uselessNavigationReady) return;
    window.uselessNavigationReady = true;
    const ids = ["menu-toggle", "appearance-toggle", "account-toggle"];
    const close = except => ids.forEach(id => {
        const input = document.getElementById(id);
        if (input && id !== except) input.checked = false;
    });
    document.addEventListener("change", event => {
        if (ids.includes(event.target.id) && event.target.checked) close(event.target.id);
    });
    document.addEventListener("click", event => {
        if (event.target.closest(".nav-scrollable a, .login-container a, a.brand")) close();
        else if (!event.target.closest(".sidebar, .top-row")) close();
    });
    document.addEventListener("keydown", event => {
        if (event.key !== "Escape") return;
        const openId = ids.find(id => document.getElementById(id)?.checked);
        close();
        if (openId) document.getElementById(openId)?.focus();
    });
    if (window.Blazor?.addEventListener) window.Blazor.addEventListener("enhancedload", () => close());
})();
