# Eclipse Modding wiki

The Astro Starlight documentation site lives in `Docs/Modding/`, outside Unity's
`Assets/` import tree and inside the main Git repository. The site sources,
configuration, build scripts, and npm lockfile are tracked. Dependencies and
generated output are ignored. No Unity editor or game build is needed.

## Run locally

Use Node.js **22.12 or newer** (CI uses Node 22). From the repository root:

```powershell
cd Docs/Modding
npm ci
npm run dev
```

Open the URL printed by Astro, including `/ProjectEclipse/`.

```powershell
npm run build
npm run preview
```

The build audits reference coverage against the Lua bindings, runs Astro's checks, builds the static
site and search index, then checks every local HTML link, fragment, and asset
path against the GitHub Pages base. Output is in `Docs/Modding/dist/`.

## Where to edit

| Content | Tracked source |
| --- | --- |
| Homepage, beginner guides, examples | `src/content/docs/` (`.md` or `.mdx`) |
| Public API reference | `src/content/docs/api/` |
| First-weapon starter and displayed snippets | `public/tutorial/training-blade/` |
| Function coverage audit and alphabetical index generator | `scripts/check-reference.mjs` |
| Sidebar and theme | `astro.config.mjs`, `src/styles/custom.css` |
| GitHub repository and Pages URL | `site.config.mjs` |

Public pages are authored for mod creators, independently of engineering notes
in `Mods/`. Verify function names, fields, types, defaults, capabilities, return
values, and callback timing against the actual Lua bindings and runtime. Do not
copy internal milestone or sweep labels into titles, prose, or code examples.
Keep source links descriptive even when repository paths retain historical names.

Every public Lua function, alias, fighter method, and combat callback needs a
separate level-two heading containing its exact name, followed by **Signature**,
**Returns**, **When**, **Requires**, and a Lua example. Explain shared argument
tables and defaults on the same page. Distinguish complete examples from fragments
that require handles or files created earlier.

`npm run check:reference` inventories the tracked `MoonSharpScriptRuntime*.cs`
bindings, checks dedicated sections, rejects internal milestone labels, and
regenerates the alphabetical `src/content/docs/reference.md` index. Only that
index is ignored; all authored reference pages must be tracked. When adding a
new binding pattern, update the audit to recognize it. This structural check does
not replace checking documentation accuracy against implementation.

`predev` and `prebuild` run this audit automatically. Rerun it when adding function
sections during a development session. Use `pageUrl()` for internal MDX links;
relative links work for neighboring Markdown pages. Add new pages to the sidebar.
The first-weapon tutorial imports its displayed text from the downloadable starter
files so they cannot drift apart.

Keep API and example changes documented in the same change, as required by the
root `AGENTS.md`. Run `npm run build` before handing off. The build also checks
rendered internal links, fragments, assets, GitHub Pages paths, and search output.

## Deploy to GitHub Pages

The supplied workflow is `.github/workflows/modding-docs.yml`. It validates
relevant pushes to `main` and pull requests. **Deployment is manual** so the
repository owner chooses when to publish.

1. Commit and push the site, the workflow, and its `package-lock.json`.
2. In the GitHub repository, open **Settings > Pages** and choose
   **GitHub Actions** as the build/deployment source.
3. Open **Actions > Modding documentation > Run workflow** on `main`.
4. After the deployment job succeeds, visit
   <https://dawc17.github.io/ProjectEclipse/>.

The workflow checks out documentation, sample mods, linked tools, and the Lua binding source. It
does not download Git LFS game bundles or build Unity. GitHub Pages receives
only the generated static `dist/` artifact.

For a repository rename or custom domain, update `site` and `base` in
`site.config.mjs` (use an empty `base` for a domain root), update the repository
URL if needed, and rebuild. Configure a custom domain in GitHub Pages separately.

Framework references: [Starlight](https://starlight.astro.build/),
[Astro on GitHub Pages](https://docs.astro.build/en/guides/deploy/github/).
