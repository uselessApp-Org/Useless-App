(() => {
    const themes = ["garden", "midnight", "bubblegum", "blueprint", "sunset", "terminal", "lavender", "coffee", "ocean", "paper"];
    const read = (key, fallback) => { try { return localStorage.getItem(key) || fallback; } catch { return fallback; } };
    const save = (key, value) => { try { localStorage.setItem(key, value); } catch { /* Preferences still work for this visit. */ } };
    let theme = read("useless.theme", "midnight");
    if (!themes.includes(theme)) theme = "midnight";
    const apply = () => document.documentElement.dataset.theme = theme;
    apply();
    window.uselessPreferences = {
        getTheme: () => theme,
        setTheme: value => { theme = themes.includes(value) ? value : "midnight"; apply(); save("useless.theme", theme); },
        getPins: () => { try { const pins = JSON.parse(read("useless.pins", "[]")); return Array.isArray(pins) ? pins.filter(x => typeof x === "string") : []; } catch { return []; } },
        setPins: pins => save("useless.pins", JSON.stringify(pins))
    };
    // Blazor enhanced navigation can replace document attributes.
    document.addEventListener("blazor:enhancedload", apply);
})();
