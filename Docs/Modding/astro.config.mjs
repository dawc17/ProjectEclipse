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
          { slug: 'guides/lua-basics' },
          { slug: 'guides/manifest' },
          { slug: 'guides/first-battle' },
          { slug: 'api/installing-mods' },
          { slug: 'guides/troubleshooting' },
        ] },
        { label: 'Understand modding', items: [
          { slug: 'guides/core-concepts' },
          { slug: 'api/definitions-and-behavior' },
          { slug: 'api/core-assets' },
          { slug: 'api/core-equipment' },
          { slug: 'api/sprites-and-textures' },
          { slug: 'api/save-compatibility' },
          { slug: 'guides/compatibility' },
          { slug: 'api/legacy-compatibility' },
        ] },
        { label: 'Function index', slug: 'reference' },
        { label: 'Content reference', items: [
          { slug: 'api/logging' },
          { slug: 'api/assets' },
          { slug: 'api/localization-patches' },
          { slug: 'api/equipment-shop-logging' },
          { slug: 'api/shop' },
          { slug: 'api/content-graph' },
          { slug: 'api/rules' },
          { slug: 'api/quests' },
          { slug: 'api/items-progression-forge' },
          { slug: 'api/locations-and-locales' },
          { slug: 'api/moves-and-tactics' },
          { slug: 'api/asset-replacement' },
        ] },
        { label: 'Behavior and progression', items: [
          { slug: 'api/perks-and-enchantments' },
          { slug: 'api/behavior-instances' },
          { slug: 'api/combat-callbacks' },
          { slug: 'api/fighter' },
          { slug: 'api/mod-state' },
          { slug: 'api/achievements' },
          { slug: 'api/events-and-modes' },
          { slug: 'api/offline-raids' },
          { slug: 'api/timers-and-services' },
        ] },
        { label: 'Working examples', slug: 'examples' },
      ],
    }),
  ],
});
