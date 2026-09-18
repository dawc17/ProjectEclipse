local sf2 = require("sf2")

-- New orders finish immediately. Saved deadlines currently survive this policy;
-- retain their normal skip path until the API supports completing pending orders.
sf2.timers.set {
    subsystem = "forge",
    seconds = 0,
    skip_enabled = true,
}
