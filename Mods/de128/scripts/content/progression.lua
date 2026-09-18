local sf2 = require("sf2")
local perks = require("content.combat_perks")

-- The corresponding DE moves omit these direct perk locks. Suplex has separate
-- combat and profile moves. Preserve all skeleton, equipment and screen locks.
for _, lock in ipairs({
    { move = "DoubleJumpKick", perk = "PERK_DOUBLE_JUMP_KICK" },
    { move = "ElbowStrike", perk = "PERK_ELBOW_STRIKE" },
    { move = "TwoFootJumpKick", perk = "PERK_TWO_FOOT_JUMP_KICK" },
    { move = "BackFlipKick", perk = "PERK_BACK_FLIP_KICK" },
    { move = "ThrowSuplex", perk = "PERK_SUPLEX" },
    { move = "ThrowSuplexProfile", perk = "PERK_SUPLEX" },
}) do
    sf2.moves.remove_perk_lock { move = lock.move, perk = sf2.perks.get("core:perks/" .. lock.perk) }
end

-- Assets/DExml/CharacterProgress.xml: PerkTree/Level Values 4,8,11,14,17.
sf2.progression.replace_perk_branch {
    level = 4,
    entries = {
        { action = "unlock", perk = perks.master_of_style },
        { action = "unlock", perk = perks.relentless },
    },
}
for _, level in ipairs({ 8, 11, 14, 17 }) do
    sf2.progression.replace_perk_branch {
        level = level,
        entries = {
            { action = "upgrade", perk = perks.master_of_style },
            { action = "upgrade", perk = perks.relentless },
        },
    }
end

return perks
