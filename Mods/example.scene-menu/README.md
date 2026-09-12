# Scene Menu

Enable this example on API 0.31, then enter map, shop, profile or dojo. A menu
using Eclipse's original-game UI styling opens. Choose a destination; BACK or
Escape closes the menu. The menu opens again when you enter another supported
scene. Choosing the current scene simply closes it.

The native quest/tab gates remain enabled. Requests blocked by dialogs, loading,
encounter preparation or native quest interception report unavailable. This menu
does not open during combat and cannot surrender or skip a result screen.

Lua button handling is tested with a controlled navigation host. Full-game
navigation, rendered layout and keyboard/controller acceptance remain pending.

The isolated Unity fixture also runs this script through the real renderer and
input bridge: original font/sprites, button bounds, directional selection/submit,
native-dialog blocking, rejection labels and cleanup/remounting pass. The fixture
controls navigation responses, so it does not prove actual native scene transitions.
