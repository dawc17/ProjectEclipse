// Isolated smoke fixture for the project-owned TAR/LZ4 art path. It has no ResearchSources,
// recovered Android AssetBundles, or loose decoded sprite/audio payload tree.
using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Content;
using Eclipse.Modding;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif

public static class ValidatePackagedArt
{
    private static int checks;

    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidDataException(message);
        checks++;
    }

    private static void CheckPackagedBundles()
    {
        TextAsset manifest = Resources.Load<TextAsset>(PackagedArtCatalog.CatalogResourcePath);
        Require(manifest != null, "Packaged catalog missing");
        PackagedArtCatalog.Catalog catalog = PackagedArtCatalog.ReadCatalog(manifest.text);
        Resources.UnloadAsset(manifest);
        Require(catalog.version == 3, "Expected TAR/LZ4 catalog v3");

        int archives = catalog.bundles.Count(x => !string.IsNullOrEmpty(x.file));
        int fonts = catalog.bundles.Sum(x => x.assets.Count(a => !string.IsNullOrEmpty(a.font)));
        Require(catalog.bundles.Length == 95 && archives == 94 && fonts == 10,
            "Unexpected TAR/LZ4 catalog coverage");

        Sprite agnis = PackagedArtCatalog.Load<Sprite>("UI/Items/AgnisSeal.helm");
        Require(agnis != null && agnis.texture != null && agnis.rect.width > 0 && agnis.rect.height > 0,
            "AgnisSeal sprite lookup failed");
        Require(agnis.vertices.Length >= 3 && agnis.uv.Length == agnis.vertices.Length,
            "AgnisSeal sprite geometry invalid");

        Texture2D texture = PackagedArtCatalog.Load<Texture2D>("UI/Items/AgnisSeal");
        Require(texture != null && texture.width > 0 && texture.height > 0,
            "AgnisSeal texture lookup failed");

        AudioClip audio = PackagedArtCatalog.Load<AudioClip>("gamedata/music/fight_independence_day");
        Require(audio != null && audio.samples > 0 && audio.channels > 0 && audio.frequency > 0,
            "TAR PCM audio lookup failed");

        Font font = PackagedArtCatalog.Load<Font>("UI/Fonts/notoserif-regular");
        Require(font != null && font.material != null, "Loose font lookup failed");

        Require(PackagedArtCatalog.Load<TextAsset>("gamedata/list") == null,
            "Art catalog overrides gameplay XML");

        Require(PackagedArtCatalog.ContainsExactAddress("UI/Items/AgnisSeal"),
            "Exact packaged address is not indexed");
        Require(!PackagedArtCatalog.ContainsExactAddress("UI/Items/AgnisSeal.helm"),
            "Exact-address API accepted a sprite-member compatibility alias");
        var coreProvider = new CoreAssetProvider();
        AssetMetadata coreAsset;
        Require(coreProvider.TryDescribe(AssetId.Parse("core:UI/Items/AgnisSeal"), out coreAsset) &&
            coreAsset.SourceKind == AssetSourceKind.Core,
            "CoreAssetProvider did not expose packaged logical address");
        Require(!coreProvider.TryDescribe(AssetId.Parse("core:UI/Items/does_not_exist"), out coreAsset),
            "CoreAssetProvider resolved an unknown packaged address");
        CheckModHost(coreProvider);

        string model = PackagedArtCatalog.LoadModelText("gamedata/models/mdl_skeleton");
        Require(!string.IsNullOrEmpty(model) && model.Contains("<Scene") && model.Contains("<Figures>"),
            "TAR model lookup failed");

        string atlas = PackagedArtCatalog.LoadLocationDataText("Textures/Locations/arena/arena_bg_xml.xml");
        Require(!string.IsNullOrEmpty(atlas) && atlas.Contains("<plist"),
            "TAR location atlas-data lookup failed");

        Sprite[] location = PackagedArtCatalog.LoadWithSubAssets<Sprite>("Textures/Locations/arena/arena_bg");
        Require(location != null && location.Any(x => x != null && x.texture != null),
            "TAR location-art lookup failed");

        PackagedArtCatalog.BundleRecord coreLocations = catalog.bundles.FirstOrDefault(x =>
            string.Equals(x.name, "CORE_LOCATIONS", StringComparison.OrdinalIgnoreCase));
        Require(coreLocations != null && string.Equals(coreLocations.file, "CORE_LOCATIONS.tar.lz4",
            StringComparison.OrdinalIgnoreCase), "CORE_LOCATIONS group missing");

        string[] moonAtlases =
        {
            "moon_bg", "moon_clouds", "moon_atlas_layer1", "moon_atlas_layer2",
            "moon_atlas_layer3", "moon_atlas_layer4"
        };
        foreach (string moonAtlas in moonAtlases)
        {
            string address = "Textures/Locations/moon/" + moonAtlas;
            Require(coreLocations.assets.Any(x => string.Equals(x.address, address,
                StringComparison.OrdinalIgnoreCase)), "Moon TAR atlas address missing: " + moonAtlas);
        }

        // OverrideGeometry is only supported reliably in play/player context. Keep editor
        // command-mode checks to safe full-rect sprites; the standalone smoke below walks
        // every CORE_LOCATIONS address and validates Moon's tight mesh inside Start().
        if (!Application.isEditor || Application.isPlaying)
        {
            foreach (string moonAtlas in moonAtlases)
            {
                Sprite[] moonSprites = PackagedArtCatalog.LoadWithSubAssets<Sprite>(
                    "Textures/Locations/moon/" + moonAtlas);
                Require(moonSprites != null && moonSprites.Any(x => x != null && x.texture != null),
                    "Moon TAR atlas missing: " + moonAtlas);
            }
            Sprite moonLayer3 = PackagedArtCatalog.Load<Sprite>("Textures/Locations/moon/layer3");
            Require(moonLayer3 != null && moonLayer3.texture != null && moonLayer3.vertices.Length == 98,
                "Moon layer3 exact-path/tight-mesh lookup failed");
        }
        Sprite moonBackground = PackagedArtCatalog.Load<Sprite>("Textures/Locations/moon/background_1");
        Sprite dojoBackground = PackagedArtCatalog.Load<Sprite>("Textures/Locations/dojo/background_1");
        Require(moonBackground != null && dojoBackground != null &&
            moonBackground.texture != dojoBackground.texture,
            "Moon background incorrectly resolved to Dojo by basename");

        Debug.Log("[PackagedArtTest] PASS " + (Application.isEditor ? "editor" : "standalone") +
            ": " + checks + " checks; " + catalog.bundles.Length + " groups; " + archives +
            " TAR/LZ4 archives; " + fonts + " loose fonts.");
    }

    private static void CheckModHost(CoreAssetProvider coreProvider)
    {
        string modsRoot = Path.Combine(Application.temporaryCachePath,
            "sf2de-modhost-" + Guid.NewGuid().ToString("N"));
        try
        {
            string example = Path.Combine(modsRoot, "example.weapon");
            Directory.CreateDirectory(Path.Combine(example, "assets", "sprites"));
            Directory.CreateDirectory(Path.Combine(example, "assets", "models"));
            Directory.CreateDirectory(Path.Combine(example, "assets", "audio"));
            File.WriteAllBytes(Path.Combine(example, "assets", "sprites", "weapon.png"), CreateTestPng());
            File.WriteAllText(Path.Combine(example, "assets", "sprites", "weapon.sprite.toml"),
                "pivot = [0.25, 0.75]\n" +
                "pixels_per_unit = 50\n" +
                "filter = \"point\"\n" +
                "wrap = \"clamp\"\n");
            File.WriteAllText(Path.Combine(example, "assets", "models", "mdl_weapon_example.xml"),
                "<Scene><Figures /></Scene>");
            File.WriteAllBytes(Path.Combine(example, "assets", "audio", "test.wav"), CreateTestWav());
            File.WriteAllText(Path.Combine(example, "mod.toml"),
                "schema = 1\n" +
                "id = \"example.weapon\"\n" +
                "name = \"Example Weapon\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            string broken = Path.Combine(modsRoot, "broken.mod");
            Directory.CreateDirectory(broken);
            File.WriteAllText(Path.Combine(broken, "mod.toml"),
                "schema = 1\n" +
                "id = \"broken.mod\"\n" +
                "name = \"Broken\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"missing.mod\"\n" +
                "version = \">=1.0 <2.0\"\n");

            ModHost host = ModHost.Build(modsRoot);
            Require(host.HasErrors, "Broken loose mod did not surface diagnostics");
            Require(host.EnabledMods.Count == 1 && host.EnabledMods[0].Id.Value == "example.weapon",
                "Broken loose mod disabled the independent valid mod");
            AssetMetadata metadata;
            Require(host.Assets.TryDescribe(AssetId.Parse("example.weapon:sprites/weapon"), out metadata) &&
                metadata.Kind == AssetKind.Sprite, "ModHost did not mount loose sprite");
            AssetBytes bytes;
            Require(host.Assets.TryRead(AssetId.Parse("example.weapon:sprites/weapon"), out bytes) &&
                bytes.Data.Length > 4, "ModHost did not read loose sprite bytes");
            Require(host.Assets.TryDescribe(AssetId.Parse("core:UI/Items/AgnisSeal"), out metadata),
                "ModHost did not mount core provider");
            Require(host.FormatReport().Contains("DEP005"), "ModHost report omitted dependency diagnostic");

            Sprite looseSprite = host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:sprites/weapon"));
            Require(looseSprite != null && looseSprite.texture != null && looseSprite.texture.width == 2 &&
                looseSprite.texture.height == 2 && looseSprite.texture.filterMode == FilterMode.Point,
                "Typed loose sprite decode failed");
            Require(Mathf.Abs(looseSprite.pixelsPerUnit - 50f) < 0.001f &&
                (looseSprite.pivot - new Vector2(0.5f, 1.5f)).sqrMagnitude < 0.0001f,
                "Loose sprite sidecar metadata was not applied");

            string looseModel = host.TypedAssets.LoadModelText(
                AssetId.Parse("example.weapon:models/mdl_weapon_example"));
            Require(looseModel != null && looseModel.Contains("<Scene") && looseModel.Contains("<Figures"),
                "Typed loose model XML decode failed");
            AudioClip looseAudio = host.TypedAssets.LoadAudio(AssetId.Parse("example.weapon:audio/test"));
            Require(looseAudio != null && looseAudio.samples == 4 && looseAudio.channels == 1 &&
                looseAudio.frequency == 8000, "Typed loose WAV decode failed");

            string coreModel = host.TypedAssets.LoadModelText(AssetId.Parse("core:gamedata/models/mdl_skeleton"));
            Require(!string.IsNullOrEmpty(coreModel) && coreModel.Contains("<Scene"),
                "Typed core model did not delegate to PackagedArtCatalog");
            host.Dispose();

            ModRuntime.Initialize(modsRoot);
            Sprite bridgeLoose = ResourcesAndBundles.Load<Sprite>("example.weapon:sprites/weapon");
            Require(bridgeLoose != null && bridgeLoose.texture != null,
                "ResourcesAndBundles did not route qualified loose sprite through ModRuntime");
            Sprite bridgeCore = ResourcesAndBundles.Load<Sprite>("core:Textures/Locations/moon/background_1");
            Require(bridgeCore != null && bridgeCore.texture != null,
                "ResourcesAndBundles did not route qualified core sprite through ModRuntime");
            Sprite legacyCore = ResourcesAndBundles.Load<Sprite>("Textures/Locations/moon/background_1");
            Require(legacyCore != null && legacyCore.texture != null,
                "Namespaced bridge regressed unqualified legacy resource loading");
            ModRuntime.Shutdown();
        }
        finally
        {
            ModRuntime.Shutdown();
            if (Directory.Exists(modsRoot)) Directory.Delete(modsRoot, true);
        }
    }

    private static byte[] CreateTestPng()
    {
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            texture.SetPixels(new[] { Color.red, Color.green, Color.blue, Color.white });
            texture.Apply();
            return texture.EncodeToPNG();
        }
        finally
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEngine.Object.DestroyImmediate(texture);
            else UnityEngine.Object.Destroy(texture);
#else
            UnityEngine.Object.Destroy(texture);
#endif
        }
    }

    private static byte[] CreateTestWav()
    {
        short[] samples = { 0, 1000, -1000, 0 };
        using (var output = new MemoryStream())
        using (var writer = new BinaryWriter(output))
        {
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + samples.Length * 2);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(8000);
            writer.Write(16000);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(samples.Length * 2);
            foreach (short sample in samples) writer.Write(sample);
            return output.ToArray();
        }
    }

    private static int CheckLocationCoverage(PackagedArtCatalog.Catalog catalog)
    {
        var seen = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int count = 0;
        foreach (PackagedArtCatalog.BundleRecord bundle in catalog.bundles)
        {
            if (string.Equals(bundle.name, "LOCATION_DATA", StringComparison.OrdinalIgnoreCase))
                continue;
            foreach (PackagedArtCatalog.ArtRecord asset in bundle.assets)
            {
                string address = asset.address ?? string.Empty;
                if (!address.StartsWith("Textures/Locations/", StringComparison.OrdinalIgnoreCase) &&
                    !address.StartsWith("Textures/Location_effects/", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!seen.Add(address)) continue;
                count++;
            }
        }
        return count;
    }

    private static int CheckCoreLocationRuntimeCoverage(PackagedArtCatalog.Catalog catalog)
    {
        PackagedArtCatalog.BundleRecord group = catalog.bundles.FirstOrDefault(x =>
            string.Equals(x.name, "CORE_LOCATIONS", StringComparison.OrdinalIgnoreCase));
        Require(group != null, "CORE_LOCATIONS group missing during runtime coverage");
        int count = 0;
        foreach (PackagedArtCatalog.ArtRecord asset in group.assets)
        {
            Sprite[] sprites = PackagedArtCatalog.LoadWithSubAssets<Sprite>(asset.address);
            Require(sprites != null && sprites.Any(x => x != null && x.texture != null),
                "CORE_LOCATIONS runtime lookup failed: " + asset.address);
            count++;
        }
        return count;
    }

    private static int CheckLocationDataCoverage(PackagedArtCatalog.Catalog catalog)
    {
        PackagedArtCatalog.BundleRecord group = catalog.bundles.FirstOrDefault(x =>
            string.Equals(x.name, "LOCATION_DATA", StringComparison.OrdinalIgnoreCase));
        Require(group != null, "LOCATION_DATA group missing");
        int count = 0;
        foreach (PackagedArtCatalog.ArtRecord asset in group.assets)
        {
            string text = PackagedArtCatalog.LoadLocationDataText(asset.address + ".xml");
            Require(!string.IsNullOrEmpty(text), "TAR location data missing: " + asset.address);
            var xml = new XmlDocument();
            xml.LoadXml(text);
            Require(xml.DocumentElement != null &&
                (xml.DocumentElement.Name == "plist" || xml.DocumentElement.Name == "dict"),
                "TAR location data is not plist XML: " + asset.address);
            count++;
        }
        return count;
    }

#if UNITY_EDITOR
    public static void RunEditor()
    {
        try
        {
            PackagedArtCatalog.ValidateProjectFiles(Application.dataPath);
            CheckPackagedBundles();
            TextAsset manifest = Resources.Load<TextAsset>(PackagedArtCatalog.CatalogResourcePath);
            PackagedArtCatalog.Catalog catalog = PackagedArtCatalog.ReadCatalog(manifest.text);
            Resources.UnloadAsset(manifest);
            int locationAddresses = CheckLocationCoverage(catalog);
            Debug.Log("[PackagedArtTest] location coverage PASS: " + locationAddresses + " unique addresses.");
            int locationData = CheckLocationDataCoverage(catalog);
            Debug.Log("[PackagedArtTest] location data PASS: " + locationData + " atlas records.");
            if (Environment.GetCommandLineArgs().Contains("-buildContentSmoke"))
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/ContentSmoke.unity");
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { "Assets/ContentSmoke.unity" },
                    locationPathName = "Build/ContentSmoke.exe",
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.Development
                });
                Require(report.summary.result == BuildResult.Succeeded, "Content smoke player build failed");
            }
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogError("[PackagedArtTest] " + exception);
            EditorApplication.Exit(1);
        }
    }
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallPlayerSmoke()
    {
        new GameObject("PackagedArtPlayerSmoke").AddComponent<PackagedArtPlayerSmoke>();
    }

    private sealed class PackagedArtPlayerSmoke : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                CheckPackagedBundles();
                Sprite custom = PackagedArtCatalog.Load<Sprite>("UI/Items/gift_box_red_n_gold");
                Require(custom != null && custom.texture != null, "Custom-mesh sprite lookup failed");
                Require(custom.vertices.Length == 81 && custom.triangles.Length >= 3,
                    "Custom-mesh sprite geometry was not preserved");
                Debug.Log("[PackagedArtTest] custom mesh PASS: " + custom.vertices.Length + " vertices.");
                TextAsset manifest = Resources.Load<TextAsset>(PackagedArtCatalog.CatalogResourcePath);
                PackagedArtCatalog.Catalog catalog = PackagedArtCatalog.ReadCatalog(manifest.text);
                Resources.UnloadAsset(manifest);
                int coreLocations = CheckCoreLocationRuntimeCoverage(catalog);
                Debug.Log("[PackagedArtTest] core location runtime PASS: " + coreLocations + " exact addresses.");
                Sprite moonLayer3 = PackagedArtCatalog.Load<Sprite>("Textures/Locations/moon/layer3");
                Require(moonLayer3 != null && moonLayer3.vertices.Length == 98,
                    "Moon layer3 runtime mesh regression");
                Debug.Log("[PackagedArtTest] Moon layer3 PASS: " + moonLayer3.vertices.Length + " vertices.");
                Application.Quit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("[PackagedArtTest] " + exception);
                Application.Quit(1);
            }
        }
    }
#endif
}
