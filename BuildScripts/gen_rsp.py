import re

DEFINES = ('DEBUG;TRACE;UNITY_5_3_OR_NEWER;UNITY_5_4_OR_NEWER;UNITY_5_5_OR_NEWER;UNITY_5_6_OR_NEWER;'
 'UNITY_5_6_2;UNITY_5_6;UNITY_5;UNITY_PURCHASING;UNITY_ANALYTICS;ENABLE_AUDIO;ENABLE_CACHING;ENABLE_CLOTH;'
 'ENABLE_DUCK_TYPING;ENABLE_GENERICS;ENABLE_PVR_GI;ENABLE_MICROPHONE;ENABLE_MULTIPLE_DISPLAYS;ENABLE_PHYSICS;'
 'ENABLE_RUNTIME_NAVMESH_BUILDING;ENABLE_SPRITERENDERER_FLIPPING;ENABLE_SPRITES;ENABLE_TERRAIN;ENABLE_RAKNET;'
 'ENABLE_UNET;ENABLE_LZMA;ENABLE_UNITYEVENTS;ENABLE_WEBCAM;ENABLE_WWW;ENABLE_CLOUD_SERVICES_COLLAB;'
 'ENABLE_CLOUD_SERVICES_ADS;ENABLE_CLOUD_HUB;ENABLE_CLOUD_PROJECT_ID;ENABLE_CLOUD_SERVICES_UNET;'
 'ENABLE_CLOUD_SERVICES_BUILD;ENABLE_CLOUD_LICENSE;ENABLE_EDITOR_METRICS;ENABLE_EDITOR_METRICS_CACHING;'
 'ENABLE_NATIVE_ARRAY;INCLUDE_DYNAMIC_GI;INCLUDE_GI;PLATFORM_SUPPORTS_MONO;RENDER_SOFTWARE_CURSOR;'
 'INCLUDE_PUBNUB;ENABLE_PLAYMODE_TESTS_RUNNER;ENABLE_SCRIPTING_NEW_CSHARP_COMPILER;ENABLE_VIDEO;'
 'UNITY_STANDALONE_WIN;UNITY_STANDALONE;ENABLE_SUBSTANCE;ENABLE_RUNTIME_GI;ENABLE_MOVIES;ENABLE_NETWORK;'
 'ENABLE_CRUNCH_TEXTURE_COMPRESSION;ENABLE_UNITYWEBREQUEST;ENABLE_CLOUD_SERVICES;'
 'ENABLE_CLOUD_SERVICES_ANALYTICS;ENABLE_CLOUD_SERVICES_PURCHASING;'
 'ENABLE_CLOUD_SERVICES_CRASH_REPORTING;ENABLE_EVENT_QUEUE;ENABLE_CLUSTERINPUT;ENABLE_VR;'
 'ENABLE_WEBSOCKET_HOST;ENABLE_MONO;NET_2_0;ENABLE_PROFILER;UNITY_ASSERTIONS;UNITY_EDITOR;'
 'UNITY_EDITOR_64;UNITY_EDITOR_WIN;ENABLE_NATIVE_ARRAY_CHECKS;UNITY_TEAM_LICENSE')

MONO_FACADES = ['mscorlib', 'System', 'System.Core', 'System.Xml',
                'System.Runtime.Serialization', 'System.Xml.Linq']
FACADE_DIR = 'F:/Unity/Editor/Data/MonoBleedingEdge/lib/mono/2.0-api/'

EXCLUDE_SOURCES = ('IAPDemo.cs', 'IAPButton.cs', 'AppStoreSettings.cs', 'PBIPLFGGOOP.cs')
EXCLUDE_REFS = ('Purchasing.Common', 'Plugins' + chr(92) + 'Security.dll', 'Plugins/Security.dll')
EXCLUDE_PLUGIN_DLLS = {'Apple.dll', 'FacebookStore.dll', 'Tizen.dll', 'Stores.dll', 'winrt.dll',
                       'UnityStore.dll', 'ChannelPurchase.dll', 'Purchasing.Common.dll',
                       'Security.dll', 'System.Security.dll'}


def csproj_items(csproj):
    cs = open(csproj, encoding='utf-8').read()
    compiles = [c.replace(chr(92), '/') for c in re.findall(r'<Compile Include="([^"]+)"', cs)]
    refs = re.findall(r'<Reference Include="([^"]+)">\s*<HintPath>([^"]+)</HintPath>', cs)
    return compiles, refs


def write_rsp(path, out_dll, compiles, refs, extra_refs=()):
    with open(path, 'w') as f:
        f.write('-target:library\n-out:%s\n-nowarn:0169,0649,0108,0109,0219,0414,0618,3021\n'
                '-langversion:7.3\n-nostdlib+\n' % out_dll)
        f.write('-define:' + DEFINES + '\n')
        for _, hp in refs:
            import os as _os
            if _os.path.basename(hp) in EXCLUDE_PLUGIN_DLLS:
                continue
            if any(x in hp for x in EXCLUDE_REFS) or ('Library' in hp and 'Purchasing' in hp):
                continue
            f.write('-r:%s\n' % hp)
        for e in extra_refs:
            f.write('-r:%s\n' % e)
        for c in compiles:
            if not any(x in c for x in EXCLUDE_SOURCES):
                f.write(c + '\n')


c1, r1 = csproj_items('Assembly-CSharp-firstpass.csproj')
write_rsp('BuildScripts/roslyn_fp.rsp', 'BuildScripts/out/Assembly-CSharp-firstpass.dll', c1, r1,
           extra_refs=['Assets/Plugins/UnityEngine.Purchasing.dll'])

c2, r2 = csproj_items('Assembly-CSharp.csproj')
write_rsp('BuildScripts/roslyn_main.rsp', 'BuildScripts/out/Assembly-CSharp.dll', c2, r2,
          extra_refs=['BuildScripts/out/Assembly-CSharp-firstpass.dll',
                      'Assets/Plugins/UnityEngine.Purchasing.dll'])
print('rsp written')
