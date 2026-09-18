local sf2 = require("sf2")

-- These public gates suppress matching native quest/service groups.
-- Complete removal of their UI and compiled SDKs needs separate host coverage.
for _, service in ipairs({
    "paid_offers",
    "battle_pass",
    "ads",
    "rewarded_video",
    "online_services",
    "payments",
}) do
    sf2.services.disable(service)
end
