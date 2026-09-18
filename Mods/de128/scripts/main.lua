local sf2 = require("sf2")

assert(type(sf2.services) == "table" and type(sf2.services.disable) == "function",
    "DE128 requires the public sf2.services.disable API.")
assert(type(sf2.timers) == "table" and type(sf2.timers.set) == "function",
    "DE128 requires the public sf2.timers.set API.")

-- Module order is explicit. Eclipse commits these declarations together.
require("content.services")
require("content.timers")
require("content.equipment")
require("content.rewards")
require("content.progression")
-- Disabled at the owner's request. Keep the prototype out of the active mod.
-- require("content.ascension")

sf2.log.info("DE128 " .. sf2.mod.version .. " policies, Desolator reward and XML-evidenced combat perks registered.")
