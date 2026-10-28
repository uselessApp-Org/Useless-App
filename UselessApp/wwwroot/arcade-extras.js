(()=>{'use strict';
const active=new Map();
const gym={
 assess(password){let n=password.length, pools=0;
 if(/[a-z]/.test(password))pools+=26;if(/[A-Z]/.test(password))pools+=26;
 if(/[0-9]/.test(password))pools+=10;if(/[^A-Za-z0-9]/.test(password))pools+=33;
 let bits=pools?Math.log2(pools)*n:0;
 if(/^(.)\1+$/.test(password))bits=Math.min(bits,5);
 if(/password|123456|qwerty|letmein|admin|welcome|iloveyou/i.test(password))bits=Math.min(bits,12);
 if(/^(?:[a-zA-Z]+|\d+)$/.test(password) && n<10) bits=Math.min(bits,30);
 let level=bits<28?0:bits<40?1:bits<60?2:bits<80?3:4;
 return {level,bits:Math.round(bits),weight:[5,25,100,315,1000][level],name:['Foam Dumbbell','Gym Beginner','Dedicated Lifter','Powerlifter','Legendary Spotter'][level]};
 },
 mount(id){let root=document.getElementById(id);if(!root)return;const input=root.querySelector('[data-private-password]');const result=root.querySelector('[data-gym-result]');const bar=root.querySelector('[data-gym-bar]');const lifter=root.querySelector('[data-gym-lifter]');let show=false;
 const update=()=>{const v=input.value;let r=this.assess(v);result.textContent=v?`${r.name} — lifts ${r.weight} lb (rough estimate: ${r.bits} bits)`:'Enter a made-up password to start lifting.';bar.style.width=v?`${Math.max(5,((r.level+1)/5)*100)}%`:'0%';lifter.style.setProperty('--lift',`${(r.level+1)*-10}px`);};
 const oninput=()=>update();input.addEventListener('input',oninput);update();active.set(id,()=>{input.removeEventListener('input',oninput);input.value='';});
 },
 clear(id){active.get(id)?.();active.delete(id)}
};
let audioCtx;
function sound(f=600){try{audioCtx??=new (window.AudioContext||window.webkitAudioContext)();const o=audioCtx.createOscillator(),g=audioCtx.createGain();o.connect(g).connect(audioCtx.destination);o.frequency.value=f;g.gain.setValueAtTime(.09,audioCtx.currentTime);g.gain.exponentialRampToValueAtTime(.001,audioCtx.currentTime+.12);o.start();o.stop(audioCtx.currentTime+.13)}catch{}}
const coin={async flip(id){const root=document.getElementById(id);if(!root)return null;const disk=root.querySelector('[data-coin-disk]');disk.classList.remove('flipping');void disk.offsetWidth;disk.classList.add('flipping');const heads=crypto.getRandomValues(new Uint32Array(1))[0]<2147483648;await new Promise(res=>setTimeout(res,1100));disk.textContent=heads?'H':'T';disk.classList.remove('flipping');sound(heads?660:440);return heads?'Heads':'Tails';}};
const platform={
 start(id){this.stop(id);const canvas=document.getElementById(id),ctx=canvas?.getContext('2d');if(!ctx)return;let on=true, frame=0,player={x:60,y:240,vy:0},score=0,dead=false,obstacles=[],holding=false,jumps=0,last=performance.now();
 const difficulty=document.getElementById('platformer-difficulty');
 const course=document.getElementById('platformer-course');
 const settings={easy:{speed:2.9,frequency:135},normal:{speed:4,frequency:95},hard:{speed:5.7,frequency:73},nightmare:{speed:7.3,frequency:58}};
 const reset=()=>{player={x:60,y:240,vy:0};obstacles=[];score=0;frame=0;dead=false;jumps=0;holding=false;};
 const change=()=>reset();difficulty?.addEventListener('change',change);course?.addEventListener('change',change);
 const button=document.getElementById(id+'-jump');const press=(ev)=>{if(ev?.repeat)return;ev?.preventDefault();if(dead){reset();return;}if(player.y>=240){player.vy=-12;jumps=1;sound(650);}else if(!holding&&jumps<2){player.vy=-9;jumps=2;sound(880);}holding=true;};const release=()=>holding=false;
 const key=(e)=>{if(['Space','ArrowUp'].includes(e.code))press(e)};button?.addEventListener('pointerdown',press);button?.addEventListener('pointerup',release);button?.addEventListener('pointercancel',release);window.addEventListener('keydown',key);window.addEventListener('keyup',release);window.addEventListener('pointerup',release);
 function draw(ts){if(!on)return;let dt=Math.min((ts-last)/16.667,2);last=ts;ctx.clearRect(0,0,600,300);ctx.fillStyle=getComputedStyle(canvas).getPropertyValue('--card-bg').trim()||'#26354b';ctx.fillRect(0,0,600,300);ctx.fillStyle='#62c2a8';ctx.fillRect(0,265,600,35);ctx.fillStyle='#eab35c';ctx.fillRect(44,player.y,24,25);ctx.fillStyle='#1b2439';ctx.fillRect(60,player.y+5,5,5);
 if(!dead){frame+=dt;player.vy+=.7*dt;player.y=Math.min(240,player.y+player.vy*dt);if(player.y>=240){player.vy=0;jumps=0;}
 const settingsNow=settings[difficulty?.value]||settings.normal;
 const track=course?.value||'classic';
 let frequency=settingsNow.frequency*(track==='hurdles'?.72:track==='chaos'?.83:track==='marathon'?.9:1);
 if(frame>frequency){frame=0;const width=(track==='hurdles'?15:track==='chaos'?35:22)+Math.floor(Math.random()*(track==='chaos'?22:16));obstacles.push({x:620,w:width});}
 for(const o of obstacles){o.x-=(settingsNow.speed+score/(track==='marathon'?12:18))*dt;if(o.x+o.w<0){score++;sound(550)}ctx.fillStyle='#e47c7c';ctx.fillRect(o.x,237,o.w,28);if(o.x<68&&o.x+o.w>44&&player.y+25>237){dead=true;sound(180);}}
 obstacles=obstacles.filter(o=>o.x+o.w>0);}
 ctx.fillStyle='#fff';ctx.font='bold 18px system-ui';ctx.fillText((course?.selectedOptions[0]?.textContent||'Classic Run')+' · '+(difficulty?.selectedOptions[0]?.textContent||'Normal')+' · Cleared: '+score,14,25);if(dead){ctx.fillStyle='#fff';ctx.font='bold 23px system-ui';ctx.fillText('GAME OVER — press Jump to restart',120,110)}requestAnimationFrame(draw)}requestAnimationFrame(draw);
 active.set(id,()=>{on=false;button?.removeEventListener('pointerdown',press);button?.removeEventListener('pointerup',release);button?.removeEventListener('pointercancel',release);window.removeEventListener('keydown',key);window.removeEventListener('keyup',release);window.removeEventListener('pointerup',release);difficulty?.removeEventListener('change',change);course?.removeEventListener('change',change)});
 },stop(id){active.get(id)?.();active.delete(id)}
};
window.uselessExtras={gym,coin,platform};
})();