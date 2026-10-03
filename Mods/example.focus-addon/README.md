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
Managed production-Lua tests pass with controlled combat/UI sources. Full-game
contact timing, rendering and save/menu acceptance remain unverified.
