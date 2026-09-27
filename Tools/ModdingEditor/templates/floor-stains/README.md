# Floor stains

Copy this folder into `Mods/` and choose your own manifest ID, name and author.
This presentation-only example launches droplets along hit impulses. They land
on the sampled arena floor and accumulate into capped pools within one effect ID.
The Options > Mod settings switch clears existing drops and pools when disabled.

Set `speed_min` and `speed_max` to zero for instant splats, or `merge_radius` to
zero for independent splats. `max_pool_size` must be at least `size_max` when
merging. No custom art is required.

See the [visual effects reference](https://dawc17.github.io/ProjectEclipse/api/visuals/#sf2fxstain)
for units, defaults, rendering requirements and capacity limits.
