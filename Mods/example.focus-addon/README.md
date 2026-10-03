# Focus Trial add-on

Install this folder together with `example.focus-framework`. Enter Act I
tournament battle 3 in normal or Eclipse mode. The add-on uses the framework's
saved resource, mounts a Focus HUD and adds a damage bonus every third positive,
unblocked outgoing hit. The bonus is half the pending damage, capped at one
normalized damage unit.

The add-on owns its combat capability and HUD. Requests and responses carry
plain typed data; fighter handles remain in this script. See the
[framework README](../example.focus-framework/README.md) and the
[API reference](../../Docs/Modding/src/content/docs/api/extensions.md).
Managed production-Lua tests and isolated Unity 6.6 HUD/font/pixel, removal/reinstall
state and teardown checks pass with controlled fight contacts and asset-host
inputs. Full-game contact timing and native save/menu acceptance remain unverified.
