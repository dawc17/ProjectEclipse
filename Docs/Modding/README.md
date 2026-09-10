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

The build synchronizes the reference, runs Astro's checks, builds the static
site and search index, then checks every local HTML link, fragment, and asset
path against the GitHub Pages base. Output is in `Docs/Modding/dist/`.

## Where to edit

| Content | Tracked source |
| --- | --- |
| Homepage, guides, example index | `src/content/docs/` (`.md` or `.mdx`) |
| Current API contracts | `../../Mods/README.md`, `P1C_API.md`, `P2_API.md`, `P3_API.md` |
| Integrated showcase walkthroughs | `../../Mods/example.phase*/README.md` |
| First-weapon code snippets | Actual files in `../../Mods/example.weapon/` |
| Reference section routes | `scripts/sync-docs.mjs` |
| Sidebar and theme | `astro.config.mjs`, `src/styles/custom.css` |
| GitHub repository and Pages URL | `site.config.mjs` |

The sync script splits the maintained API documents by heading and rewrites
links between them to wiki routes. Unpublished roadmap/audit links lead to their
source files on GitHub. Reference **Edit page** links target the original file.
Unknown or removed mapped sections fail the build so new content cannot silently
disappear from navigation. Add its route mapping and sidebar entry together.

Do not edit `src/content/docs/api/` or `src/content/docs/showcases/`: they are
ignored generated copies. Run `npm run sync:docs` after editing the source
documents during a development session, or restart `npm run dev`. Every build
synchronizes them automatically. Keep hand-authored pages in their existing
folders or a new folder outside those two generated directories.

Use `pageUrl()` from `site.config.mjs` for internal MDX links so the repository
base is included. Add new pages to the sidebar. Write API details from the
implemented public Lua contract and examples; label legacy and planned behavior
clearly. API work is incomplete until the wiki is updated in the same change.

## Deploy to GitHub Pages

The supplied workflow is `.github/workflows/modding-docs.yml`. It validates
relevant pushes to `main` and pull requests. **Deployment is manual** so the
repository owner chooses when to publish.

1. Commit and push the site, the workflow, and its `package-lock.json`.
2. In the GitHub repository, open **Settings → Pages** and choose
   **GitHub Actions** as the build/deployment source.
3. Open **Actions → Modding documentation → Run workflow** on `main`.
4. After the deployment job succeeds, visit
   <https://dawc17.github.io/ProjectEclipse/>.

The workflow checks out only documentation, sample mods, and linked tools. It
does not download Git LFS game bundles or build Unity. GitHub Pages receives
only the generated static `dist/` artifact.

For a repository rename or custom domain, update `site` and `base` in
`site.config.mjs` (use an empty `base` for a domain root), update the repository
URL if needed, and rebuild. Configure a custom domain in GitHub Pages separately.

Framework references: [Starlight](https://starlight.astro.build/),
[Astro on GitHub Pages](https://docs.astro.build/en/guides/deploy/github/).
