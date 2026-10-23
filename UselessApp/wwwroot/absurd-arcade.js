/* Self-contained Web Audio effects. Distinct timbres/envelopes; no external assets. */
window.absurdArcade = (() => {
  let ctx;
  const audio = () => ctx ||= new (window.AudioContext || window.webkitAudioContext)();
  function osc(c,type,f,t,d,v,slide){let o=c.createOscillator(), g=c.createGain();o.type=type;o.frequency.setValueAtTime(f,t);if(slide)o.frequency.exponentialRampToValueAtTime(Math.max(25,slide),t+d);g.gain.setValueAtTime(.0001,t);g.gain.exponentialRampToValueAtTime(Math.max(.0002,v),t+.008);g.gain.exponentialRampToValueAtTime(.0001,t+d);o.connect(g);g.connect(c.destination);o.start(t);o.stop(t+d+.01)}
  function noise(c,t,d,v,filterType='lowpass',freq=1500){let buffer=c.createBuffer(1,Math.ceil(c.sampleRate*d),c.sampleRate),array=buffer.getChannelData(0);for(let i=0;i<array.length;i++)array[i]=Math.random()*2-1;let src=c.createBufferSource(),filter=c.createBiquadFilter(),gain=c.createGain();src.buffer=buffer;filter.type=filterType;filter.frequency.value=freq;gain.gain.setValueAtTime(Math.max(.0002,v),t);gain.gain.exponentialRampToValueAtTime(.0001,t+d);src.connect(filter);filter.connect(gain);gain.connect(c.destination);src.start(t);src.stop(t+d)}
  async function play(sound,volume){try{let c=audio();await c.resume();let t=c.currentTime+.015,v=Math.min(.35,Math.max(.008,Number(volume)||.2));switch(sound){
    case 'Pop':osc(c,'sine',480,t,.085,v,130);noise(c,t,.055,v*.7,'highpass',1600);break;
    case 'Boing':osc(c,'sine',170,t,.43,v,65);osc(c,'triangle',420,t,.2,v*.35,130);break;
    case 'Applause':for(let i=0;i<23;i++)noise(c,t+i*.032,.06,v*.30,'bandpass',1000+Math.random()*2600);break;
    case 'Explosion':noise(c,t,.95,v,'lowpass',350);osc(c,'sawtooth',95,t,.75,v*.5,32);break;
    case 'Record Scratch':for(let i=0;i<10;i++)noise(c,t+i*.04,.055,v*.55,'bandpass',300+((i*673)%3200));break;
    case 'Victory':for(let [i,f] of [523,659,784,1047,1318].entries())osc(c,'triangle',f,t+i*.11,.23,v*.6);break;
    case 'Failure':for(let [i,f] of [440,392,330,220].entries())osc(c,'square',f,t+i*.17,.19,v*.28);break;
    case 'Laser':osc(c,'sawtooth',1600,t,.42,v*.35,110);break;
    case 'Coin':osc(c,'sine',988,t,.10,v);osc(c,'sine',1319,t+.11,.28,v);break;
    case 'Drama':for(let i=0;i<3;i++){osc(c,'triangle',90,t+i*.23,.2,v,60);noise(c,t+i*.23,.12,v*.22,'lowpass',450)}break;
    case 'Squeak':osc(c,'square',850,t,.22,v*.23,1600);break;
    case 'Drum Roll':for(let i=0;i<20;i++)noise(c,t+i*.055,.06,v*.26,'lowpass',500);noise(c,t+1.15,.65,v,'lowpass',1200);break;
    default:osc(c,'sine',440,t,.2,v);break;
  }}catch(e){console.warn('Audio unavailable',e)}}
  return {play};
})();
