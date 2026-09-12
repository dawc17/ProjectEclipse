# Eclipse Item Reward

Requires API 0.33. Enable in the title-screen Mods menu and Apply & Restart.
Use a test profile with Lynx's Eclipse replay available and without Monk's Katars.
Win the first bodyguard encounter in that replay. The example adds Monk's Katars
to its one-win Eclipse reward scope. Native shared rewards, money and experience
remain. No new map icon or custom screen is added.

The target is BOSS_LYNX_ECLIPSEMODE, not BOSS_LYNX. Normal campaign fights are
unchanged. This mod does not unlock/reset encounters, bypass reward eligibility,
or force another equipment grant when you already own the item. Native result
handling skips owned equipment; repeatable mod consumables have separate behavior.

Disable and restart to restore the original reward definition. Items already
awarded remain in inventory. Other mods replacing this exact reward scope conflict;
different modes/level scopes remain independent additions.

The actual manifest/Lua is tested against canonical stages and items. Native
builder/parser/result-selection fixtures cover item handling with controlled host
services. Full-game reward display, inventory granting and save/reload acceptance
remain pending. Test on a disposable profile because the example awards real loot.
