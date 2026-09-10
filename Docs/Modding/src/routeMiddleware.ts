import { defineRouteMiddleware } from '@astrojs/starlight/route-data';

export const onRequest = defineRouteMiddleware((context) => {
  const route = context.locals.starlightRoute;
  if (route.id !== '404') return;
  // Pages serves this document at any missing path; it has no canonical URL.
  route.head = route.head.filter(({ attrs }) =>
    attrs?.rel !== 'canonical' && attrs?.property !== 'og:url');
});
