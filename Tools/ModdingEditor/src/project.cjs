const fs=require('node:fs/promises'),path=require('node:path'),lua=require('luaparse');
const api=require('../data/api.json');
const MAX_FILES=10000;
const slash=s=>s.replaceAll('\\','/');
const safe=s=>typeof s==='string'&&s.length>0&&!path.isAbsolute(s)&&!s.includes(':')&&!slash(s).split('/').some(p=>!p||p==='.'||p==='..');
function stripComment(line){let quoted=false,escape=false;for(let i=0;i<line.length;i++){const c=line[i];if(escape){escape=false;continue;}if(c==='\\'&&quoted){escape=true;continue;}if(c==='"')quoted=!quoted;if(c==='#'&&!quoted)return line.slice(0,i);}return line;}
function parseValue(raw){
    if(/^"(?:[^"\\]|\\.)*"$/.test(raw))return JSON.parse(raw);
    if(/^-?\d+$/.test(raw))return Number(raw);
    if(raw==='true'||raw==='false')return raw==='true';
    if(raw.startsWith('[')&&raw.endsWith(']')){const v=JSON.parse(raw.replace(/,\s*\]$/,']'));if(Array.isArray(v)&&v.every(x=>typeof x==='string'))return v;}
    throw new Error('Use a quoted string, whole number, boolean, or one-line string array.');
}
function manifest(text){
    const data={dependencies:[]},issues=[],positions={};let target=data;
    text.split(/\r?\n/).forEach((line,lineNo)=>{
        const clean=stripComment(line).trim();if(!clean)return;
        if(clean==='[[dependencies]]'){target={};data.dependencies.push(target);return;}
        const match=/^([a-z_]+)\s*=\s*(.+)$/.exec(clean);
        if(!match){issues.push({line:lineNo,message:'Expected a supported key = value or [[dependencies]] section.'});return;}
        const [,key,raw]=match;positions[key]??=lineNo;
        const allowed=target===data?['schema','id','name','version','api','authors','entrypoint','capabilities']:['id','version'];
        if(!allowed.includes(key)){issues.push({line:lineNo,message:`Unknown ${target===data?'manifest':'dependency'} field: ${key}.`});return;}
        if(Object.hasOwn(target,key)){issues.push({line:lineNo,message:`Duplicate field: ${key}.`});return;}
        try{target[key]=parseValue(raw);}catch(e){issues.push({line:lineNo,message:e.message});}
    });
    const issue=(key,message)=>issues.push({line:positions[key]??0,message});
    for(const key of ['schema','id','name','version','api','authors','entrypoint','capabilities'])if(data[key]===undefined)issue(key,`Missing required manifest field: ${key}.`);
    if(data.schema!==1)issue('schema','schema must be 1.');
    if(typeof data.id!=='string'||!/^[-a-z0-9_.]+$/.test(data.id)||['core','sf2de'].includes(data.id))issue('id','Choose a unique lowercase mod ID; core and sf2de are reserved.');
    if(typeof data.name!=='string'||!data.name.trim())issue('name','name must be a nonempty string.');
    if(typeof data.version!=='string'||!/^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?(?:\+[0-9A-Za-z.-]+)?$/.test(data.version))issue('version','Use a semantic version such as 1.0.0.');
    if(typeof data.api!=='string'||!data.api.trim().split(/\s+/).every(t=>/^(?:>=|<=|>|<|=)?\d+(?:\.\d+){0,2}(?:-[0-9A-Za-z.-]+)?(?:\+[0-9A-Za-z.-]+)?$/.test(t)))issue('api','Use comparison ranges such as >=0.7 <1.0.');
    for(const key of ['authors','capabilities'])if(!Array.isArray(data[key])||!data[key].every(v=>typeof v==='string'&&v.trim())||(key==='authors'&&!data[key].length))issue(key,`${key} must be a ${key==='authors'?'nonempty ':''}one-line array of nonempty strings.`);else if(new Set(data[key]).size!==data[key].length)issue(key,`${key} contains duplicates.`);
    if(!safe(data.entrypoint)||!slash(data.entrypoint).startsWith('scripts/')||!data.entrypoint.endsWith('.lua'))issue('entrypoint','Use a safe path inside scripts/, ending in .lua.');
    const ids=new Set();for(const d of data.dependencies){if(typeof d.id!=='string'||!/^[-a-z0-9_.]+$/.test(d.id)||typeof d.version!=='string')issue('id','Each dependency needs a valid id and version range.');if(ids.has(d.id))issue('id',`Duplicate dependency: ${d.id}.`);ids.add(d.id);}
    return {data,issues,positions};
}
async function walk(root){const files=[],pending=[root];while(pending.length){let entries;const folder=pending.pop();try{entries=await fs.readdir(folder,{withFileTypes:true});}catch(e){if(e.code==='ENOENT')continue;throw e;}for(const entry of entries){if(entry.isSymbolicLink())continue;const file=path.join(folder,entry.name);if(entry.isDirectory())pending.push(file);else if(entry.isFile())files.push(file);if(files.length>MAX_FILES)throw new Error(`Mod contains more than ${MAX_FILES} indexed files.`);}}return files;}
function assetKind(relative){const ext=path.extname(relative).toLowerCase(),id=slash(relative).toLowerCase();if(ext==='.asset')return 'sprite';if(ext==='.png')return id.startsWith('sprites/')?'sprite':'texture';if(ext==='.xml'&&id.startsWith('models/'))return 'model';if(ext==='.wav')return 'audio';if(['.ogg','.mp3'].includes(ext))return 'unsupported-audio';if(['.xml','.toml','.json','.txt','.lua'].includes(ext))return 'text';return 'binary';}
async function indexMod(root,read=async f=>fs.readFile(f,'utf8')){
    const parsed=manifest(await read(path.join(root,'mod.toml'))),assets=new Map(),localizations=new Map(),issues=[];
    for(const file of await walk(path.join(root,'assets'))){
        if(file.endsWith('.sprite.toml'))continue;
        const relative=slash(path.relative(path.join(root,'assets'),file));
        const id=relative.slice(0,path.extname(relative)?-path.extname(relative).length:undefined).toLowerCase(),kind=assetKind(relative);
        if(assets.has(id))issues.push({file,message:`Duplicate logical asset ID: ${id}.`});
        if(kind==='unsupported-audio')issues.push({file,message:'Loose mod audio supports PCM16 WAV, not OGG or MP3.'});
        assets.set(id,{id,kind,file});
        if(relative.endsWith('.asset')){
            const fields=Object.fromEntries((await read(file)).split(/\r?\n/).map(l=>/^\s*(\w+)\s*=\s*(.+?)\s*$/.exec(stripComment(l))).filter(Boolean).map(m=>[m[1],m[2].replace(/^"|"$/g,'')]));
            if(fields.type!=='sprite')issues.push({file,message:'Sprite descriptor requires type=sprite.'});
            if(!safe(fields.texture)||!fields.texture.toLowerCase().endsWith('.png'))issues.push({file,message:'texture must be a relative PNG path inside assets/.'});
            else {const texture=path.join(root,'assets',fields.texture);try{await fs.access(texture);}catch{issues.push({file,message:`Missing texture: ${fields.texture}.`});}}
        }
    }
    for(const file of await walk(path.join(root,'localizations'))){
        if(!file.endsWith('.toml'))continue;
        const language=path.basename(file,'.toml'),seen=new Set();
        (await read(file)).split(/\r?\n/).forEach((l,line)=>{
            const clean=stripComment(l).trim();if(!clean)return;
            const m=/^([^=]+)=\s*(.+)$/.exec(clean);if(!m){issues.push({file,line,message:'Expected localization key = "text".'});return;}
            const id=m[1].trim();let value;try{value=parseValue(m[2].trim());if(typeof value!=='string')throw Error();}catch{issues.push({file,line,message:'Localization values must be quoted strings.'});return;}
            if(seen.has(id))issues.push({file,line,message:`Duplicate localization key: ${id}.`});seen.add(id);
            const record=localizations.get(id)??{id,translations:[]};record.translations.push({language,value,file,line});localizations.set(id,record);
        });
    }
    if(safe(parsed.data.entrypoint))try{await fs.access(path.join(root,parsed.data.entrypoint));}catch{parsed.issues.push({line:parsed.positions.entrypoint??0,message:`Entrypoint file is missing: ${parsed.data.entrypoint}.`});}
    return {root,...parsed,assets,localizations,issues:[...issues,...parsed.issues.map(x=>({...x,file:path.join(root,'mod.toml')}))]};
}
const literal=n=>n?.type==='UnaryExpression'&&n.operator==='-'?-literal(n.argument):n?.type==='StringLiteral'?(n.value??n.raw.slice(1,-1).replace(/\\(["'\\])/g,'$1')):n?.type==='NumericLiteral'||n?.type==='BooleanLiteral'?n.value:undefined;
const fields=n=>Object.fromEntries((n?.fields??[]).filter(f=>f.type==='TableKeyString'||f.type==='TableKey').map(f=>[f.key.name??literal(f.key),f.value]));
function parse(text){try{return lua.parse(text,{luaVersion:'5.2',locations:true,ranges:true,comments:false});}catch{return null;}}
function analyze(text,mod){
    const ast=parse(text),issues=[],calls=[],contexts=[];if(!ast)return {issues,calls,contexts};
    function resolve(n,env){if(!n)return null;if(n.type==='Identifier')return env.get(n.name);if(n.type==='MemberExpression'){const base=resolve(n.base,env);return typeof base==='string'?`${base}.${n.identifier.name}`:null;}return null;}
    function add(n,code,message,capability){issues.push({range:n.range,code,message,capability});}
    const caps=new Set(mod.data.capabilities??[]),dependencies=new Set((mod.data.dependencies??[]).map(d=>d.id));
    function required(n,cap){if(Array.isArray(cap)){for(const item of cap)required(n,item);}else if(cap&&!caps.has(cap))add(n,'capability',`Declare "${cap}" in mod.toml to use this operation.`,cap);}
    function expression(n,env,callback){
        if(!n||typeof n!=='object')return;
        if(['CallExpression','TableCallExpression','StringCallExpression'].includes(n.type)){
            const symbol=resolve(n.base,env),args=Array.isArray(n.arguments)?n.arguments:n.arguments?[n.arguments]:n.argument?[n.argument]:[];
            const name=api.aliases[symbol]??symbol,info=api.functions[name];
            if(info){calls.push({name,node:n,args});required(n,info.capability);
                if(info.referenceKind&&typeof literal(args[0])==='string'){
                    const reference=literal(args[0]),colon=reference.indexOf(':'),owner=colon<0?mod.data.id:reference.slice(0,colon),id=colon<0?reference:reference.slice(colon+1);
                    if(owner!==mod.data.id&&!dependencies.has(owner))add(args[0],'dependency',`Declare dependency "${owner}" before referencing its content.`);
                    if(owner===mod.data.id){const collection=info.referenceKind==='localization'?mod.localizations:mod.assets;const localId=info.referenceKind==='localization'?id.replace(/^localization\//,''):id.toLowerCase();const target=collection.get(localId);if(!target)add(args[0],'missing-reference',`No ${info.referenceKind} found for "${reference}" in this mod.`);else if(info.referenceKind!=='localization'&&target.kind!==info.referenceKind)add(args[0],'asset-kind',`"${reference}" is ${target.kind}, but this call requires ${info.referenceKind}.`);}
                }
                if(info.bounds)for(const [key,[min,max]] of Object.entries(info.bounds)){const index=Object.keys(info.params).indexOf(key),value=literal(args[index]);if(typeof value==='number'&&(!Number.isInteger(value)||value<min||value>max))add(args[index],'range',`${key} must be an integer from ${min} through ${max}.`);}
                if(name==='sf2.behaviors.register'){
                    const definition=fields(args[0]);
                    for(const cb of api.callbacks){const handler=definition[cb];if(handler?.type!=='FunctionDeclaration')continue;const next=new Map(env);for(const p of handler.parameters)next.set(p.name,null);const parameters=handler.parameters[0]?.name,fighter=handler.parameters[1]?.name,event=handler.parameters[2]?.name;if(fighter)next.set(fighter,'fighter');
                        contexts.push({range:handler.range,callback:cb,parameters,fighter,event,stateful:!!definition.state,schema:fields(definition.parameters),stateSchema:fields(fields(definition.state).fields)});
                        block(handler.body,next,cb);
                    }
                }
            }
            if(typeof symbol==='string'&&symbol.startsWith('fighter.')){const method=symbol.split('.').at(-1),target=symbol.startsWith('fighter.opponent.');const spec=api.fighterMethods[method];if(spec){required(n,spec.capability);if(target)required(n,'combat.target');if(method==='scale_incoming_damage'&&callback!=='on_damage_resolving')add(n,'callback-timing','scale_incoming_damage is available only in on_damage_resolving.');}}
            for(const arg of args){if(info&&name==='sf2.behaviors.register'&&arg===args[0]){for(const f of arg.fields??[])if(!api.callbacks.includes(f.key?.name))expression(f.value,env,callback);}else expression(arg,env,callback);}return;
        }
        if(n.type==='FunctionDeclaration'){const next=new Map(env);for(const p of n.parameters)next.set(p.name,null);block(n.body,next,callback);return;}
        for(const [key,value] of Object.entries(n)){if(['loc','range','comments'].includes(key))continue;if(Array.isArray(value))for(const v of value)expression(v,env,callback);else if(value&&typeof value==='object')expression(value,env,callback);}
    }
    function block(body,env,callback){for(const node of body??[]){
        if(node.type==='LocalStatement'||node.type==='AssignmentStatement'){
            for(const init of node.init)expression(init,env,callback);
            node.variables.forEach((variable,i)=>{if(variable.type!=='Identifier')return;const init=node.init[i];const name=init?.type==='CallExpression'&&init.base.type==='Identifier'&&init.base.name==='require'&&literal(init.arguments[0])==='sf2'&&!env.has('require')?'sf2':resolve(init,env);env.set(variable.name,name);});
        }else if(node.type==='IfStatement'){for(const clause of node.clauses){expression(clause.condition,env,callback);block(clause.body,new Map(env),callback);}}
        else if(['DoStatement','WhileStatement','RepeatStatement','ForNumericStatement','ForGenericStatement'].includes(node.type)){const next=new Map(env);if(node.variable)next.set(node.variable.name,null);for(const v of node.variables??[])next.set(v.name,null);block(node.body,next,callback);}
        else expression(node,env,callback);
    }}
    block(ast.body,new Map(),null);return {issues,calls,contexts};
}
// Called only for incomplete-string completion. Parse a repaired document so
// aliases/comments/shadowed names are handled by the same AST analysis.
function completionContext(text,offset,mod){
    const prefix=text.slice(0,offset),match=/(["'])([^"'\n]*)$/.exec(prefix);if(!match)return null;
    const start=offset-match[2].length;
    let result;
    for(const ending of [match[1],match[1]+')',match[1]+')\n}',match[1]+')\n}\nend'] ){
        const repaired=prefix+ending+text.slice(offset).replace(/^[^"'\r\n]*["']/, '');
        result=analyze(repaired,mod).calls.find(c=>c.args[0]?.type==='StringLiteral'&&c.args[0].range[0]===start-1);if(result)break;
    }
    if(!result)return null;return {name:result.name,kind:api.functions[result.name]?.referenceKind,start,prefix:match[2]};
}
module.exports={manifest,indexMod,walk,assetKind,analyze,completionContext,literal,fields,safe,stripComment,parseValue};
