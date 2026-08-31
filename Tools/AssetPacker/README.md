# SF2DE AssetPacker

Small authoring tool for the runtime `.tar.lz4` art format.

```powershell
dotnet run --project Tools/AssetPacker -- pack .\my_asset .\my_asset.tar.lz4
dotnet run --project Tools/AssetPacker -- list .\my_asset.tar.lz4
dotnet run --project Tools/AssetPacker -- verify .\my_asset.tar.lz4
dotnet run --project Tools/AssetPacker -- extract .\my_asset.tar.lz4 .\unpacked
dotnet run --project Tools/AssetPacker -- info .\my_asset.tar.lz4
dotnet run --project Tools/AssetPacker -- compress .\my_asset.tar .\my_asset.tar.lz4
```

Bundles are ordinary USTAR archives compressed as a standard LZ4 Frame. The runtime only discovers
logical assets through `*.meta` files. Other files are payloads and are ignored unless a descriptor
references them. References are always relative to the same archive.

## v1 descriptor format

Descriptors deliberately use a tiny `key=value` format. Empty lines and lines starting with `#` are
ignored. Unknown fields are safe for future tooling to preserve.

Sprite:

```ini
type=sprite
namespace=core
address=UI/Items/AgnisSeal
name=AgnisSeal
texture=textures/AgnisSeal.png
rect=0,0,256,256
pivot=0.5,0.5
border=0,0,0,0
pixels_per_unit=100
filter=1
aniso=1
wrap_u=1
wrap_v=1
mipmaps=false
vertices=-1,-1;1,-1;1,1;-1,1
triangles=0,1,2,2,3,0
uv=0,0;1,0;1,1;0,1
```

Audio:

```ini
type=audio
namespace=core
address=Sounds/UI/click
name=click
file=audio/click.wav
```

`sound` and `music` are accepted aliases for `audio`. The current player decoder intentionally only
accepts PCM16 WAV because that is what the recovered native art currently contains. XML and model
descriptors are rejected by v1 so configuration/model loading cannot accidentally migrate into this
experiment.

`AssetPacker verify` rejects unsafe paths, case-colliding entries, non-regular TAR entries, XML,
unknown asset types, missing required fields, and references to payloads outside the archive.
`AssetPacker info` prints the compressed size, decoded TAR size, and SHA-256 values used by
`Assets/Resources/SF2Content/Art/catalog.json`.
