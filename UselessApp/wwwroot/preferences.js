(() => {
    const themes = ["seasonal-auto","lava","cosmic","disco","aurora","aquarium","candy","midnight","garden","bubblegum","blueprint","sunset","terminal","lavender","coffee","ocean","paper","slate","sand","cherry","spring","summer","autumn","winter","halloween","festive","valentine","harvest","newyear","rainy"];
    const read = (key, fallback) => { try { return localStorage.getItem(key) || fallback; } catch { return fallback; } };
    const save = (key, value) => { try { localStorage.setItem(key, value); } catch { /* Preferences still work for this visit. */ } };
    const systemTheme = window.matchMedia?.("(prefers-color-scheme: dark)");
    let savedTheme = read("useless.theme", "");
    let followsSystem = !themes.includes(savedTheme);
    const preferredTheme = () => systemTheme?.matches ? "midnight" : "garden";
    let theme = followsSystem ? preferredTheme() : savedTheme;
    let customColor = read("useless.customAccent", "");
    let customEnabled = read("useless.customAccentEnabled", "false") === "true";
    const validColor = value => /^#[0-9a-fA-F]{6}$/.test(value);
    const luminance = hex => {
        const rgb = [1,3,5].map(i => parseInt(hex.slice(i,i+2),16)/255).map(v => v <= .04045 ? v/12.92 : ((v+.055)/1.055)**2.4);
        return .2126*rgb[0]+.7152*rgb[1]+.0722*rgb[2];
    };
    const readableInk = hex => {
        const l=luminance(hex);
        return (1.05/(l+.05)) >= ((l+.05)/.05) ? '#ffffff' : '#000000';
    };
    const seasonalTheme = (date = new Date()) => {
        const month = date.getMonth() + 1, day = date.getDate();
        if (month === 10 && day >= 20) return "halloween";
        if (month === 12) return "festive";
        if (month === 1 && day <= 7) return "newyear";
        if (month === 11) return "harvest";
        if (month === 2 && day <= 14) return "valentine";
        if (month >= 3 && month <= 5) return "spring";
        if (month >= 6 && month <= 8) return "summer";
        if (month >= 9 && month <= 11) return "autumn";
        return "winter";
    };
    const apply = () => {
        document.documentElement.dataset.theme = theme === "seasonal-auto" ? seasonalTheme() : theme;
        document.querySelectorAll("#theme-choice").forEach(select => { select.value = theme; });
        document.querySelectorAll("[data-theme-option]").forEach(button => {
            const selected = button.dataset.themeOption === theme;
            button.setAttribute("aria-pressed", String(selected));
            if (selected) document.querySelectorAll("[data-theme-label]").forEach(label => { label.textContent = button.children[1].textContent; });
        });
        document.querySelectorAll("[data-custom-color-enabled]").forEach(button => {
            const swatchColor = validColor(customColor) ? customColor : "#35a884";
            button.style.backgroundColor = swatchColor;
            const symbolInk = readableInk(swatchColor);
            button.style.color = symbolInk;
            button.style.setProperty("--symbol-outline", symbolInk === "#ffffff" ? "#000000" : "#ffffff");
            button.setAttribute("aria-label", (customEnabled ? "Disable" : "Enable") + " custom color " + swatchColor);
            button.title = (customEnabled ? "Disable" : "Enable") + " custom color";
            button.setAttribute("aria-pressed", String(customEnabled));

        });
        if (customEnabled && validColor(customColor)) {
            document.documentElement.style.setProperty('--green',customColor);
            document.documentElement.style.setProperty('--accent',customColor);
            document.documentElement.style.setProperty('--button-text',readableInk(customColor));
            document.documentElement.style.setProperty('--accent-ink',readableInk(customColor));
        } else {
            for(const name of ['--green','--accent','--button-text','--accent-ink']) document.documentElement.style.removeProperty(name);
        }
    };
    const savedColors = () => { try {const colors=JSON.parse(read('useless.savedColors','[]'));return Array.isArray(colors)?colors.filter(validColor).slice(0,20):[];}catch{return [];} };
    apply();
    window.uselessPreferences = {
        getTheme: () => theme,
        getSeasonalTheme: seasonalTheme,
        isCustomAccentEnabled: () => customEnabled,
        setCustomAccentEnabled: enabled => {
            if (enabled && !validColor(customColor)) {
                customColor = window.uselessPreferences.getCustomAccent();
                save("useless.customAccent", customColor);
            }
            customEnabled = Boolean(enabled);
            save("useless.customAccentEnabled", String(customEnabled));
            apply();
        },
        getCustomAccent: () => { const color=validColor(customColor) ? customColor : getComputedStyle(document.documentElement).getPropertyValue("--green").trim(); return /^#[0-9a-f]{3}$/i.test(color) ? "#"+color.slice(1).split("").map(c=>c+c).join("") : validColor(color) ? color : "#35a884"; },
        setCustomAccent: value => { if (validColor(value)) {customColor=value;save("useless.customAccent",value);}else{customColor="";customEnabled=false;save("useless.customAccentEnabled","false");try{localStorage.removeItem("useless.customAccent");}catch{}}apply(); },
        setTheme: value => { followsSystem = false; theme = themes.includes(value) ? value : preferredTheme(); apply(); save("useless.theme", theme); },
        getSavedColors: savedColors,
        saveColor: color => { if (!validColor(color)) return savedColors(); const list=[color.toLowerCase(),...savedColors().filter(c => c.toLowerCase() !== color.toLowerCase())].slice(0,20);save('useless.savedColors',JSON.stringify(list));return list; },
        deleteColor: color => {const list=savedColors().filter(c => c.toLowerCase() !== color.toLowerCase());save('useless.savedColors',JSON.stringify(list));return list;},
        pickScreenColor: async () => { if (!window.EyeDropper) return null; try {const result=await new window.EyeDropper().open();return result.sRGBHex;}catch{return null;} },
        getPins: () => { try { const pins = JSON.parse(read("useless.pins", "[]")); return Array.isArray(pins) ? pins.filter(x => typeof x === "string") : []; } catch { return []; } },
        setPins: pins => save("useless.pins", JSON.stringify(pins))
    };
    systemTheme?.addEventListener("change", () => {
        if (followsSystem) { theme = preferredTheme(); apply(); }
    });
    // Update Auto season across date boundaries without changing a saved choice.
    setInterval(() => { if (theme === "seasonal-auto") apply(); }, 60000);
    document.addEventListener("visibilitychange", () => { if (!document.hidden && theme === "seasonal-auto") apply(); });
    const rgbToHex = (r,g,b) => '#'+[r,g,b].map(n=>Math.max(0,Math.min(255,n)).toString(16).padStart(2,'0')).join('');
    const hexToHsl = hex => {
        const [r,g,b]=[1,3,5].map(i=>parseInt(hex.slice(i,i+2),16)/255);
        const max=Math.max(r,g,b),min=Math.min(r,g,b),d=max-min;let h=0,s=0,l=(max+min)/2;
        if(d){s=d/(1-Math.abs(2*l-1));switch(max){case r:h=((g-b)/d)%6;break;case g:h=(b-r)/d+2;break;default:h=(r-g)/d+4;}h*=60;if(h<0)h+=360;}
        return [Math.round(h),Math.round(s*100),Math.round(l*100)];
    };
    const hslToHex = (h,s,l) => {
        s/=100;l/=100;const a=s*Math.min(l,1-l);
        const f=n=>{const k=(n+h/30)%12;return Math.round((l-a*Math.max(-1,Math.min(k-3,9-k,1)))*255);};
        return rgbToHex(f(0),f(8),f(4));
    };
    const initializedToggles = new WeakSet();
    const initializeColorEditors = () => document.querySelectorAll('[data-color-editor]').forEach(root => {
        const $ = s => root.querySelector(s);
        const panel=$('#accent-panel'),toggle=$('[data-color-toggle]'),hex=$('[data-hex]'),dot=$('[data-accent-dot]'),preview=$('[data-color-preview]');
        const hue=$('[data-hue]'),sat=$('[data-saturation]'),light=$('[data-lightness]'),status=$('[data-color-status]');
        const swatches=$('[data-saved-colors]');
        const required=[panel,toggle,hex,dot,preview,hue,sat,light,status,swatches,
            $('[data-color-wheel]'),$('[data-wheel-marker]'),$('[data-color-close]'),
            $('[data-save-color]'),$('[data-reset-color]'),$('#theme-choice'),
            $('[data-screen-pick]'),$('[data-image-upload]'),$('[data-image-area]'),
            $('[data-image-canvas]'),$('[data-close-image]')];
        if(required.some(element=>!element) || initializedToggles.has(toggle)) return;
        initializedToggles.add(toggle);
        const notify=text=>status.textContent=text;
        const updateUI=(color,sourceHsl)=>{
            hex.value=color;preview.style.backgroundColor=color;
            const hsl=sourceHsl || hexToHsl(color);[hue.value,sat.value,light.value]=hsl;
            const [h,s,l]=hsl;
            hue.style.background=`linear-gradient(to right,${[0,60,120,180,240,300,360].map(v=>`hsl(${v} ${s}% ${l}%)`).join(',')})`;
            sat.style.background=`linear-gradient(to right,hsl(${h} 0% ${l}%),hsl(${h} 100% ${l}%))`;
            light.style.background=`linear-gradient(to right,#000,hsl(${h} ${s}% 50%),#fff)`;
            const wheel=$('[data-color-wheel]'),marker=$('[data-wheel-marker]'),angle=h*Math.PI/180;
            marker.style.left=`${50+Math.cos(angle)*s/2}%`;marker.style.top=`${50+Math.sin(angle)*s/2}%`;
            wheel.setAttribute('aria-valuenow',h);wheel.setAttribute('aria-valuetext',`Hue ${h}, saturation ${s}%, lightness ${l}%`);
        };
        const choose=(color,sourceHsl)=>{
            if(!validColor(color)) {notify('Enter a six-digit hex color, like #35a884.');return;}
            window.uselessPreferences.setCustomAccent(color);updateUI(color,sourceHsl);notify(customEnabled ? 'Color applied and saved on this device.' : 'Color saved. Click the color square to enable it.');
        };
        const renderSaved=()=>{
            swatches.replaceChildren();
            savedColors().forEach(color=>{
                const item=document.createElement('span');item.className='accent-saved';
                const use=document.createElement('button');use.type='button';use.className='accent-swatch';use.style.backgroundColor=color;use.title='Apply '+color;use.setAttribute('aria-label','Apply '+color);use.addEventListener('click',()=>choose(color));
                const del=document.createElement('button');del.type='button';del.className='accent-remove';del.textContent='×';del.setAttribute('aria-label','Remove '+color);del.addEventListener('click',()=>{window.uselessPreferences.deleteColor(color);renderSaved();});
                item.append(use,del);swatches.append(item);
            });
        };
        const open=value=>{panel.hidden=!value;toggle.setAttribute('aria-expanded',String(value));if(value){updateUI(window.uselessPreferences.getCustomAccent());renderSaved();}};
        toggle.addEventListener('click',()=>open(panel.hidden));
        $('[data-color-close]').addEventListener('click',()=>open(false));
        [hue,sat,light].forEach(slider=>slider.addEventListener('input',()=>{const hsl=[Number(hue.value),Number(sat.value),Number(light.value)];choose(hslToHex(...hsl),hsl);}));
        const wheel=$('[data-color-wheel]');
        const pickWheel=event=>{
            const rect=wheel.getBoundingClientRect(),dx=(event.clientX-rect.left)/rect.width-.5,dy=(event.clientY-rect.top)/rect.height-.5;
            choose(hslToHex((Math.atan2(dy,dx)*180/Math.PI+360)%360,Math.min(100,Math.hypot(dx,dy)*200),50));
        };
        wheel.addEventListener('pointerdown',event=>{wheel.setPointerCapture(event.pointerId);pickWheel(event);});
        wheel.addEventListener('pointermove',event=>{if(wheel.hasPointerCapture(event.pointerId))pickWheel(event);});
        wheel.addEventListener('keydown',event=>{
            if(!['ArrowLeft','ArrowRight','ArrowUp','ArrowDown'].includes(event.key))return;event.preventDefault();
            choose(hslToHex((Number(hue.value)+(event.key==='ArrowRight'?3:event.key==='ArrowLeft'?-3:0)+360)%360,Math.max(0,Math.min(100,Number(sat.value)+(event.key==='ArrowUp'?2:event.key==='ArrowDown'?-2:0))),50));
        });
        hex.addEventListener('change',()=>choose(hex.value.trim()));
        $('[data-save-color]').addEventListener('click',()=>{window.uselessPreferences.saveColor(hex.value);renderSaved();notify('Saved to favorites on this device.');});
        $('[data-reset-color]').addEventListener('click',()=>{window.uselessPreferences.setCustomAccent('');updateUI(window.uselessPreferences.getCustomAccent());notify('Theme default restored.');});
        const themeChoice=$('#theme-choice');themeChoice.value=window.uselessPreferences.getTheme();
        themeChoice.addEventListener('change',()=>{window.uselessPreferences.setTheme(themeChoice.value);updateUI(window.uselessPreferences.getCustomAccent());});
        const screen=$('[data-screen-pick]');
        {
            screen.hidden=!(window.isSecureContext && typeof window.EyeDropper === 'function');
            screen.addEventListener('click',async()=>{
                if(!window.isSecureContext || typeof window.EyeDropper !== 'function') { notify('Screen sampling is not supported by this browser. Choose a photo below and tap a pixel instead.'); return; }
                try {const result=await new window.EyeDropper().open(); if(result?.sRGBHex) choose(result.sRGBHex);}
                catch {notify('Screen sampling was canceled or blocked. Choose a photo to sample a color.');}
            });
        }
        // Mobile-friendly image sampling: image bytes stay in browser memory; no file upload occurs.
        const upload=$('[data-image-upload]'),area=$('[data-image-area]'),canvas=$('[data-image-canvas]'),context=canvas.getContext('2d',{willReadFrequently:true});
        upload.addEventListener('change',()=>{
            const file=upload.files?.[0];if(!file||!file.type.startsWith('image/')) return;
            const image=new Image(),url=URL.createObjectURL(file);
            image.onload=()=>{
                const scale=Math.min(1,720/image.naturalWidth,480/image.naturalHeight);
                canvas.width=Math.max(1,Math.round(image.naturalWidth*scale));canvas.height=Math.max(1,Math.round(image.naturalHeight*scale));
                context.drawImage(image,0,0,canvas.width,canvas.height);area.hidden=false;
                URL.revokeObjectURL(url);notify('Tap a color in the photo.');
            };
            image.onerror=()=>{URL.revokeObjectURL(url);notify('Unable to open this image.');};
            image.src=url;
        });
        canvas.addEventListener('click',event=>{
            const rect=canvas.getBoundingClientRect();
            const x=Math.min(canvas.width-1,Math.max(0,Math.floor((event.clientX-rect.left)*canvas.width/rect.width)));
            const y=Math.min(canvas.height-1,Math.max(0,Math.floor((event.clientY-rect.top)*canvas.height/rect.height)));
            const pixel=context.getImageData(x,y,1,1).data;choose(rgbToHex(pixel[0],pixel[1],pixel[2]));
        });
        $('[data-close-image]').addEventListener('click',()=>{area.hidden=true;upload.value='';});
        updateUI(window.uselessPreferences.getCustomAccent());renderSaved();
    });
    // Theme selection does not depend on the advanced color editor initializing.
    document.addEventListener("click", event => {
        const customToggle = event.target.closest("[data-custom-color-enabled]");
        if (customToggle) {
            window.uselessPreferences.setCustomAccentEnabled(!customEnabled);
            return;
        }
        const button = event.target.closest("[data-theme-option]");
        if (button) {
            window.uselessPreferences.setTheme(button.dataset.themeOption);
            const menu = button.closest("[data-theme-menu]");
            menu.open = false;
            menu.querySelector("summary").focus();
        } else {
            document.querySelectorAll("[data-theme-menu][open]").forEach(menu => {
                if (!menu.contains(event.target)) menu.open = false;
            });
        }
    });
    document.addEventListener("toggle", event => {
        const category = event.target;
        if (!category.matches?.(".theme-category") || !category.open) return;
        document.querySelectorAll(".theme-category[open]").forEach(other => {
            if (other !== category) other.open = false;
        });
    }, true);
    document.addEventListener("keydown", event => {
        if (event.key !== "Escape") return;
        document.querySelectorAll("[data-theme-menu][open]").forEach(menu => {
            menu.open = false;
            menu.querySelector("summary").focus();
        });
    });
    window.uselessPreferences.refresh = () => { apply(); initializeColorEditors(); };
    const startColorEditors=()=>{
        apply();
        initializeColorEditors();
        new MutationObserver(initializeColorEditors).observe(document.body,{childList:true,subtree:true});
        if(window.Blazor?.addEventListener) window.Blazor.addEventListener('enhancedload',()=>{apply();initializeColorEditors();});
    };
    if(document.readyState==='loading') document.addEventListener('DOMContentLoaded',startColorEditors,{once:true});
    else startColorEditors();
    document.addEventListener('blazor:enhancedload',()=>{apply();initializeColorEditors();});
})();