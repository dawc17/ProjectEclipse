local sf2 = require("sf2")

assert(type(sf2.services) == "table" and type(sf2.services.disable) == "function",
    "DE128 requires the public sf2.services.disable API.")
assert(type(sf2.timers) == "table" and type(sf2.timers.set) == "function",
    "DE128 requires the public sf2.timers.set API.")

-- Module order is explicit. Eclipse commits these declarations together.
require("content.services")
require("content.timers")
require("content.forge")
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
require("content.wasp_fly")
require("content.butcher_earthquake")
require("content.hermit_storm")
require("content.war_whirl")
require("content.gatekeeper_power_field")
require("content.blackness_grasp")
require("content.saturn_blaster")
require("content.dandy_lightning_chain")
require("content.shop")
require("content.underworld_equipment")
require("content.titan_reward_equipment")
require("content.rewards")
require("content.progression")
require("content.campaign_music")
require("content.dojo_changer")
-- Sensei story: guard templates and RaidCharge availability are synthesized from
-- archive evidence (see sensei_guard_templates.lua, sensei_raid_charge_state.lua).
require("content.sensei_story").install_default()
-- Underworld: all eight archived tiers, sharing the story's RaidCharge rule, plus
-- the archived intro (after Lynx 2), toggle gating and the 32 bosses' dialogues.
local underworld = require("content.underworld").install(
    require("content.sensei_raid_charge").register(require("content.sensei_raid_charge_state").create()))
require("content.underworld_story").install(underworld)
-- Disabled at the owner's request. Keep the prototype out of the active mod.
-- require("content.ascension")

sf2.log.info("DE128 " .. sf2.mod.version .. " policies, Titan reward set, XML-evidenced combat perks, campaign music, dojo changer, Sensei story and Underworld registered.")
