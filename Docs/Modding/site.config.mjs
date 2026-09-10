// GitHub Pages project URL. Change both values when moving to a custom domain.
export const site = 'https://dawc17.github.io';
export const base = '/ProjectEclipse';
export const repository = 'https://github.com/dawc17/ProjectEclipse';
export const branch = 'main';
export const sourceUrl = (path) => `${repository}/blob/${branch}/${path}`;
export const pageUrl = (path = '') => `${base}/${path.replace(/^\/+|\/+$/g, '')}${path ? '/' : ''}`;
