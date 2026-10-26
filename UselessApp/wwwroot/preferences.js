(() => {
    const themes = ["garden", "midnight", "bubblegum", "blueprint", "sunset", "terminal", "lavender", "coffee", "ocean", "paper"];
    const read = (key, fallback) => { try { return localStorage.getItem(key) || fallback; } catch { return fallback; } };
    const save = (key, value) => { try { localStorage.setItem(key, value); } catch { /* Preferences still work for this visit. */ } };
    let theme = read("useless.theme", "midnight");
    if (!themes.includes(theme)) theme = "midnight";
    let customColor = read("useless.customAccent", "");
    const validColor = value => /^#[0-9a-fA-F]{6}$/.test(value);
    const luminance = hex => {
        const rgb = [1,3,5].map(i => parseInt(hex.slice(i,i+2),16)/255).map(v => v <= .04045 ? v/12.92 : ((v+.055)/1.055)**2.4);
        return .2126*rgb[0]+.7152*rgb[1]+.0722*rgb[2];
    };
    const readableInk = hex => {
        const l=luminance(hex);
        return (1.05/(l+.05)) >= ((l+.05)/.05) ? '#ffffff' : '#111111';
    };
    const apply = () => {
        document.documentElement.dataset.theme = theme;
        if (validColor(customColor)) {
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
        getCustomAccent: () => validColor(customColor) ? customColor : "#35a884",
        setCustomAccent: value => { if (validColor(value)) {customColor=value;save("useless.customAccent",value);}else{customColor="";try{localStorage.removeItem("useless.customAccent");}catch{}}apply(); },
        setTheme: value => { theme = themes.includes(value) ? value : "midnight"; apply(); save("useless.theme", theme); },
        getSavedColors: savedColors,
        saveColor: color => { if (!validColor(color)) return savedColors(); const list=[color.toLowerCase(),...savedColors().filter(c => c.toLowerCase() !== color.toLowerCase())].slice(0,20);save('useless.savedColors',JSON.stringify(list));return list; },
        deleteColor: color => {const list=savedColors().filter(c => c.toLowerCase() !== color.toLowerCase());save('useless.savedColors',JSON.stringify(list));return list;},
        pickScreenColor: async () => { if (!window.EyeDropper) return null; try {const result=await new window.EyeDropper().open();return result.sRGBHex;}catch{return null;} },
        getPins: () => { try { const pins = JSON.parse(read("useless.pins", "[]")); return Array.isArray(pins) ? pins.filter(x => typeof x === "string") : []; } catch { return []; } },
        setPins: pins => save("useless.pins", JSON.stringify(pins))
    };
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
    const initializeColorEditors = () => document.querySelectorAll('[data-color-editor]').forEach(root => {
        if(root.dataset.initialized==='true') return;
        root.dataset.initialized='true';
        const $ = s => root.querySelector(s);
        const panel=$('#accent-panel'),toggle=$('[data-color-toggle]'),hex=$('[data-hex]'),dot=$('[data-accent-dot]'),preview=$('[data-color-preview]');
        const hue=$('[data-hue]'),sat=$('[data-saturation]'),light=$('[data-lightness]'),status=$('[data-color-status]');
        const swatches=$('[data-saved-colors]');
        const notify=text=>status.textContent=text;
        const updateUI=color=>{
            hex.value=color;dot.style.backgroundColor=color;preview.style.backgroundColor=color;
            const hsl=hexToHsl(color);[hue.value,sat.value,light.value]=hsl;
            hue.style.setProperty('--current-hue',hue.value);
        };
        const choose=color=>{
            if(!validColor(color)) {notify('Enter a six-digit hex color, like #35a884.');return;}
            window.uselessPreferences.setCustomAccent(color);updateUI(color);notify('Color applied and saved on this device.');
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
        [hue,sat,light].forEach(slider=>slider.addEventListener('input',()=>choose(hslToHex(Number(hue.value),Number(sat.value),Number(light.value)))));
        hex.addEventListener('change',()=>choose(hex.value.trim()));
        $('[data-save-color]').addEventListener('click',()=>{window.uselessPreferences.saveColor(hex.value);renderSaved();notify('Saved to favorites on this device.');});
        $('[data-reset-color]').addEventListener('click',()=>{window.uselessPreferences.setCustomAccent('');updateUI('#35a884');notify('Theme default restored.');});
        const themeChoice=$('#theme-choice');themeChoice.value=window.uselessPreferences.getTheme();
        themeChoice.addEventListener('change',()=>window.uselessPreferences.setTheme(themeChoice.value));
        const screen=$('[data-screen-pick]');
        {
            screen.hidden=false;
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
    if(document.readyState==='loading') document.addEventListener('DOMContentLoaded',initializeColorEditors);
    else initializeColorEditors();
    document.addEventListener('blazor:enhancedload',initializeColorEditors);
    // Blazor enhanced navigation can replace document attributes.
    document.addEventListener("blazor:enhancedload", apply);
})();
