# Grid UI Showcase

Enable **Grid UI Showcase** (`example.grid-ui`), restart Eclipse, and enter the
map, shop, profile or dojo. The equipment selector opens automatically on scene
entry; it does not appear as a map battle and never opens in combat. Disable other
auto-opening UI examples while testing to avoid competing windows.

The grid demonstrates a reusable selector for equipment, characters or rewards.
These sample buttons select a label only: no items, currency or save data change.
Buttons, text and the panel use the original game's UI styling.

1. Confirm three columns of equipment buttons. The viewport shows about two rows;
   scroll to see all twelve choices. Click one and check the selected-name label.
2. Use arrows/D-pad/stick to navigate rows/columns and Enter/controller submit to
   select. Tab/Shift+Tab follow order. Focus on lower rows should scroll into view.
3. Click HIDE BLADE: later cells should move forward. SHOW BLADE restores it.
4. Click DISABLE SPEAR: its cell stays in place but cannot activate or receive
   navigation focus. ENABLE SPEAR restores it.
5. BACK or Escape closes the window and releases input. Enter another non-fight
   scene to reopen with the initial choices. Confirm normal game controls work.

This is a fixed-column grid, not a virtualized inventory. The view still shares
the API's 256-node limit. Full-game appearance and physical-controller acceptance
must be checked; managed example validation does not establish those outcomes.

The isolated Unity UI fixture executes this exact Lua example and verifies scene entry, native fonts/button art, columns, selection, hide/disable, lower-row focus/scrolling, keyboard activation, closing and reopening. Physical input and full-game rendering remain manual acceptance checks.
