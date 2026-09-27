# Chiaroscuro - Cinematic Visuals

Light, shadow and depth for Shadow Fight 2 fights: enhanced background depth,
weapon trails, depth haze, a rim light that turns to ink while casting magic,
light shafts with drifting dust, a slow-motion knockout fade that keeps only
the blood's colour, film halation and grain, blood stains that stay on the
floor for the round, light from fire weapons, electric weapons and magic
that falls on the stage and the fighters, dust kicked up by landings, knockdowns
and skids, and wall impacts that throw off debris with a brief jolt of the picture. Enable the mod in the Mods menu and Apply & Restart. Each effect can
then be switched off separately under Options > Mod settings.

The effects are drawn by Eclipse; this mod only turns them on and tunes them
through the public `sf2.fx`, `sf2.visuals` and `sf2.settings` API. Nothing is written to
your profile.

Floor blood uses directional droplets that follow the strike impulse and land
as flattened splats, with five drops per hit and eight extra on a knockout.
Airborne drops are large enough to read at normal fight zoom.
Repeated nearby landings accumulate into darker, larger
pools, capped separately for ordinary hits and knockouts. Fighter silhouettes
mask the pools so foreground floor art cannot hide them or paint blood over
feet. The Stains switch removes existing drops and pools when disabled.

The rim light uses a warm ivory tint and a 1.25-pixel feathered outer edge. Its existing alpha (0.85) and lightening (0.35) are unchanged; casting ink and nearby colored lights still take precedence.
