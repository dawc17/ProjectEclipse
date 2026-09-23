-- Inputs for sensei_guard_opponents.register. The prince's archived `Sphere1`
-- item (Assets/DExml/list.xml, /List/Items/Item[@Name='Sphere1']) is the record
-- restored as Minor Charge of Darkness: same image, model, Magic Sphere1
-- family, level 23, price 69, ACT_4 gate and Weakness 798 enchantment.
-- Guard_Girl/Guard_Man are synthesized; see sensei_guard_templates.lua.
local function resolve()
    local templates = require("content.sensei_guard_templates").resolve()
    return {
        guard_girl = templates.guard_girl,
        guard_man = templates.guard_man,
        sphere1 = require("content.restored_equipment").minor_charge_of_darkness,
    }
end
return { resolve = resolve }
