(() => {
    const themes = ["garden", "midnight", "bubblegum", "blueprint", "sunset", "terminal", "lavender", "coffee", "ocean", "paper"];
    const read = (key, fallback) => { try { return localStorage.getItem(key) || fallback; } catch { return fallback; } };
    const save = (key, value) => { try { localStorage.setItem(key, value); } catch { /* Preferences still work for this visit. */ } };
    let theme = read("useless.theme", "midnight");
    if (!themes.includes(theme)) theme = "midnight";
    const validHex = value => /^#[0-9a-fA-F]{6}$/.test(value);
    let customAccent = read("useless.customAccent", "");
    const paintAccent = () => {
        const root = document.documentElement;
        if (!validHex(customAccent)) {
            ["--accent", "--accent-ink", "--green", "--button-text", "--emphasis"].forEach(key => root.style.removeProperty(key));
            return;
        }
        const hex = customAccent.slice(1);
        const r = parseInt(hex.slice(0,2),16), g = parseInt(hex.slice(2,4),16), b = parseInt(hex.slice(4,6),16);
        const srgb = x => { x/=255; return x <= .04045 ? x/12.92 : Math.pow((x+.055)/1.055,2.4); };
        const luminance = .2126*srgb(r)+.7152*srgb(g)+.0722*srgb(b);
        const ink = luminance > .179 ? "#111827" : "#ffffff";
        root.style.setProperty("--accent",customAccent);
        root.style.setProperty("--green",customAccent);
        root.style.setProperty("--emphasis",customAccent);
        root.style.setProperty("--accent-ink",ink);
        root.style.setProperty("--button-text",ink);
    };
    const apply = () => { document.documentElement.dataset.theme = theme; paintAccent(); };
    apply();
    window.uselessPreferences = {
        getTheme: () => theme,
        getAccent: () => validHex(customAccent) ? customAccent : "#b8d994",
        setAccent: value => { if (!validHex(value)) return; customAccent = value; save("useless.customAccent",value); paintAccent(); },
        resetAccent: () => { customAccent = ""; try { localStorage.removeItem("useless.customAccent"); } catch {} paintAccent(); },
        setTheme: value => { theme = themes.includes(value) ? value : "midnight"; apply(); save("useless.theme", theme); },
        getPins: () => { try { const pins = JSON.parse(read("useless.pins", "[]")); return Array.isArray(pins) ? pins.filter(x => typeof x === "string") : []; } catch { return []; } },
        setPins: pins => save("useless.pins", JSON.stringify(pins))
    };
    // Blazor enhanced navigation can replace document attributes.
    document.addEventListener("blazor:enhancedload", apply);
})();
