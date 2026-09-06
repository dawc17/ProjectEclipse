# Title background artwork

`Assets/Resources/EclipseTitleBackground.png` is project-owned generated artwork,
edited with the built-in image generation tool. It is not recovered vanilla art.
The original Unity meta GUID is preserved.

Latest edit prompt:

> Edit this game title background. Remove the entire black ninja swordsman, his sword, ribbons, feet and shadow. Seamlessly fill his former area with the existing parchment, muted red sun disk, ink-wash mountains and ground. Preserve the existing composition, warm tan palette, red sun position, mountain placement, trees, and large blank left and upper area. No people, no weapons, no text, no logo, no new objects. Improve crispness of fine paper grain and ink strokes without halos or excessive contrast. Deliver at 3840x2160 resolution, landscape 16:9, suitable for sharp fullscreen display. This attached image is the edit target.

The tool returned **1672 x 941**, despite the requested output resolution.
Import at native dimensions (NPOT scaling off), without texture compression or
mipmaps. Bilinear sampling is retained to avoid pixel stair-stepping as the UI
scales. The 4096 import limit leaves room for a future higher-resolution source;
it does not increase the current source resolution.
