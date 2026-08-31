"""Create an offline canvas preview from a converted *.frames.json (stdlib only)."""
import argparse
import json
from pathlib import Path

HTML = r'''<!doctype html><html lang="en"><meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<title>Tekken to SF2 · motion study</title>
<style>
*{box-sizing:border-box}body{margin:0;background:#10161e;color:#e9edf4;font:15px system-ui,sans-serif}main{max-width:1250px;margin:auto;padding:34px}h1{font-size:28px;margin:5px 0 10px}p{color:#a8b6c7;line-height:1.6;margin:8px 0}.eyebrow{font:12px monospace;color:#8de0c2;letter-spacing:2px}.controls{display:flex;gap:18px;align-items:center;flex-wrap:wrap;margin:24px 0}button,select{color:#e9edf4;background:#273443;border:1px solid #405067;border-radius:7px;padding:8px 14px;font:inherit}input[type=range]{flex:1;min-width:180px;accent-color:#8de0c2}canvas{display:block;width:100%;background:#161f2b;border:1px solid #2b394a;border-radius:12px}.legend{display:flex;gap:28px;margin:15px 0;font:13px monospace}.green{color:#8de0c2}.blue{color:#76a7e9}.note{font-size:13px}.footer{display:flex;justify-content:space-between;margin-top:18px;border-top:1px solid #2b394a;padding-top:18px}#readout{font:14px monospace;min-width:130px}a{color:#8de0c2}
</style><main><div class="eyebrow">ECLIPSE / ANIMATION RESEARCH</div>
<h1>Tekken → Shadow Fight 2</h1>
<p>Kazuya ff3 · 60 Hz source motion retargeted to the 67-node SF2 skeleton.</p>
<p class="note">Diagnostic geometry, not the game's rendered character. Main limb lengths and SF2 hand/foot shapes are preserved.</p>
<div class="controls"><button id="play">Play</button><input id="frame" aria-label="Frame" type="range" min="0" max="59" value="20" step="1"><span id="readout"></span>
<select id="speed" aria-label="Playback speed"><option value="0.25">0.25× speed</option><option value="0.5" selected>0.5× speed</option><option value="1">1× speed</option></select>
<select id="view" aria-label="View"><option value="0">Side view</option><option value="0.65">Oblique view</option><option value="1.57079632679">Front view</option></select>
<label><input id="mirror" type="checkbox"> Mirror</label><label><input id="nodes" type="checkbox"> Nodes</label></div>
<canvas id="canvas" width="1180" height="470"></canvas>
<div class="legend"><span class="green">● Left / SF2 _1</span><span class="blue">● Right / SF2 _2</span><span>White: torso and head</span></div>
<canvas id="sheet" width="1180" height="290"></canvas>
<div class="footer"><span class="note">Experimental · grounded move · no combat rules installed</span><span class="note">Scrub to inspect the raised leg and recovery.</span></div></main>
<script>
const data=__DATA__,names=data.names,frames=data.frames;
const frame=document.getElementById('frame'), play=document.getElementById('play'),speed=document.getElementById('speed'),view=document.getElementById('view'),mirror=document.getElementById('mirror'),nodes=document.getElementById('nodes'),readout=document.getElementById('readout');
frame.max=frames.length-1;
const canvas=document.getElementById('canvas'),ctx=canvas.getContext('2d'),sheet=document.getElementById('sheet'),sc=sheet.getContext('2d');
const edges=[['NPivot','NStomach',19],['NStomach','NChest',24],['NChest','NNeck',22],['NNeck','NHead',11],['NHead','NTop',24]];
for(const s of ['2','1'])for(const [a,b,w] of [['NNeck','NShoulder',9],['NShoulder','NElbow',12],['NElbow','NWrist',9],['NWrist','NKnuckles',10],['NKnuckles','NFingertips',10],['NPivot','NHip',12],['NHip','NKnee',16],['NKnee','NAnkle',12],['NAnkle','NHeel',8],['NHeel','NToe',8],['NToe','NToeTip',6]])edges.push([a==='NNeck'||a==='NPivot'?a:a+'_'+s,b+'_'+s,w]);
function draw(c,i,ox,oy,scale,angle,grid=true){
 const f=frames[i],p=Object.fromEntries(names.map((n,k)=>[n,f[k]])), sign=mirror.checked?-1:1;
 const proj=v=>[ox+sign*(v[0]*Math.cos(angle)+v[2]*Math.sin(angle))*scale,oy-v[1]*scale];
 if(grid){c.strokeStyle='#273747';c.lineWidth=1;for(let x=0;x<1180;x+=50){c.beginPath();c.moveTo(x,0);c.lineTo(x,470);c.stroke()}for(let y=oy;y>0;y-=50){c.beginPath();c.moveTo(0,y);c.lineTo(1180,y);c.stroke()}}
 c.strokeStyle='#657385';c.lineWidth=1;c.beginPath();c.moveTo(0,oy);c.lineTo(1180,oy);c.stroke();
 const sorted=[...edges].sort((a,b)=>(p[a[0]][2]+p[a[1]][2])-(p[b[0]][2]+p[b[1]][2]));
 for(const [a,b,w]of sorted){const A=proj(p[a]),B=proj(p[b]);c.strokeStyle=b.endsWith('_1')?'#8de0c2':b.endsWith('_2')?'#76a7e9':'#e3e7ee';c.lineWidth=w*scale;c.lineCap='round';c.beginPath();c.moveTo(...A);c.lineTo(...B);c.stroke()}
 if(nodes.checked){c.fillStyle='#ebbe75';for(let k=0;k<54;k++){if(names[k].startsWith('Weapon'))continue;const q=proj(f[k]);c.beginPath();c.arc(...q,2,0,Math.PI*2);c.fill()}}
}
function render(){const i=Number(frame.value);ctx.clearRect(0,0,1180,470);draw(ctx,i,mirror.checked?830:340,415,1.15,Number(view.value));readout.textContent=`${i} / ${frames.length-1} · ${(i/data.fps).toFixed(2)} s`;sc.clearRect(0,0,1180,290);[0,10,20,30,40,59].map(x=>Math.min(x,frames.length-1)).forEach((n,k)=>{sc.save();sc.beginPath();sc.rect(k*196,0,196,290);sc.clip();const xs=edges.flatMap(e=>[frames[n][names.indexOf(e[0])][0],frames[n][names.indexOf(e[1])][0]]),center=(Math.min(...xs)+Math.max(...xs))/2;draw(sc,n,k*196+98-(mirror.checked?-1:1)*center*.65,248,.65,0,false);sc.fillStyle='#a8b6c7';sc.font='12px monospace';sc.fillText(`FRAME ${n}`,k*196+16,25);sc.restore()})}
let playing=false,previous=0,acc=0;play.onclick=()=>{playing=!playing;play.textContent=playing?'Pause':'Play';previous=0};frame.oninput=()=>{playing=false;play.textContent='Play';render()};view.onchange=mirror.onchange=nodes.onchange=render;
function tick(now){if(playing){if(previous)acc+=(now-previous)/1000*data.fps*Number(speed.value);if(acc>=1){frame.value=(Number(frame.value)+Math.floor(acc))%frames.length;acc%=1;render()}previous=now}else previous=0;requestAnimationFrame(tick)}
render();requestAnimationFrame(tick);
</script></html>'''

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('frames',type=Path)
    p.add_argument('--output',type=Path)
    args=p.parse_args()
    data=json.loads(args.frames.read_text())
    output=args.output or args.frames.with_suffix('.html')
    # Escape script terminators even when future external node names contain '<'.
    payload=json.dumps(data,separators=(',',':')).replace('<','\\u003c')
    output.write_text(HTML.replace('__DATA__',payload),encoding='utf-8')
    print(output.resolve())

if __name__=='__main__':
    main()
