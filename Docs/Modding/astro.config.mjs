import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import { base, repository, site } from './site.config.mjs';

export default defineConfig({
  site,
  base,
  trailingSlash: 'always',
  integrations: [
    starlight({
      title: 'Eclipse Modding',
      description: 'Create Shadow Fight 2 mods with the Eclipse Lua API.',
      social: [{ icon: 'github', label: 'GitHub', href: repository }],
      editLink: { baseUrl: `${repository}/edit/main/Docs/Modding/` },
      customCss: ['./src/styles/custom.css'],
      routeMiddleware: './src/routeMiddleware.ts',
      sidebar: [
        { label: 'Start here', items: [
          { slug: 'index' },
          { slug: 'guides/first-weapon' },
          { slug: 'api/installing-mods' },
          { slug: 'guides/troubleshooting' },
        ] },
        { label: 'Core concepts', items: [
          { slug: 'guides/core-concepts' },
          { slug: 'api/definitions-and-behavior' },
          { slug: 'api/core-assets' },
          { slug: 'api/save-compatibility' },
        ] },
        { label: 'Content API', items: [
          { slug: 'api/equipment-shop-logging' },
          { slug: 'api/sprites-and-textures' },
          { slug: 'api/content-graph' },
          { slug: 'api/items-progression-forge' },
          { slug: 'api/perks-and-enchantments' },
          { slug: 'api/localization-patches' },
          { slug: 'api/asset-replacement' },
          { slug: 'api/core-equipment' },
        ] },
        { label: 'Behavior and progression', items: [
          { slug: 'api/mod-state' },
          { slug: 'api/behavior-instances' },
          { slug: 'api/combat-callbacks' },
          { slug: 'api/timers-and-services' },
          { slug: 'api/events-and-modes' },
          { slug: 'api/offline-raids' },
          { slug: 'api/achievements' },
        ] },
        { label: 'Examples', items: [
          { slug: 'examples' },
          { slug: 'showcases/content' },
          { slug: 'showcases/combat-and-modes' },
          { slug: 'showcases/achievements-and-assets' },
        ] },
        { label: 'Compatibility and status', collapsed: true, items: [
          { slug: 'guides/compatibility' },
          { slug: 'api/legacy-compatibility' },
          { slug: 'api/overview' },
          { slug: 'api/combat-overview' },
          { slug: 'api/progression-overview' },
          { slug: 'api/combat-verification' },
          { slug: 'api/progression-verification' },
          { slug: 'api/configuration-scope' },
        ] },
      ],
    }),
  ],
});
