local sf2 = require("sf2")

assert(type(sf2.services) == "table" and type(sf2.services.disable) == "function",
    "DE128 requires the public sf2.services.disable API.")
assert(type(sf2.timers) == "table" and type(sf2.timers.set) == "function",
    "DE128 requires the public sf2.timers.set API.")

-- Module order is explicit. Eclipse commits these declarations together.
require("content.services")
require("content.timers")
require("content.equipment")
require("content.combat_equipment")
require("content.chinese_swords")
require("content.restored_weapons")
require("content.sphere1")
require("content.sphere2")
require("content.sphere3")
require("content.combo_sphere3")
require("content.mind_throw")
require("content.restored_equipment")
require("content.shared_moves")
require("content.shop")
require("content.rewards")
require("content.progression")
-- Sensei story: guard templates and RaidCharge availability are synthesized from
-- archive evidence (see sensei_guard_templates.lua, sensei_raid_charge_state.lua).
require("content.sensei_story").install_default()
-- Disabled at the owner's request. Keep the prototype out of the active mod.
-- require("content.ascension")

sf2.log.info("DE128 " .. sf2.mod.version .. " policies, Desolator reward, XML-evidenced combat perks and Sensei story registered.")
