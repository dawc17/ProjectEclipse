// Unity CLI eval_file method body. Runs without a profile and restores preferences.
if (ListSF.CCDKHLAMKKO() != null) throw new Exception("Run before loading a player profile.");
var floatKeys = new[] { "Eclipse.MusicVolume", "Eclipse.SoundVolume" };
var intKeys = new[] { "Eclipse.MusicMuted", "Eclipse.SoundMuted", "Eclipse.LargeControls" };
var saved = new System.Collections.Generic.Dictionary<string, float>();
foreach (var key in floatKeys) if (PlayerPrefs.HasKey(key)) saved[key] = PlayerPrefs.GetFloat(key);
foreach (var key in intKeys) if (PlayerPrefs.HasKey(key)) saved[key] = PlayerPrefs.GetInt(key);
float music = Sound.EAIGFAPKILL(), sound = Sound.NBHPABEBLOP();
bool musicMuted = Sound.ELHMADOKHHE(), soundMuted = Sound.AAFLCDKJEPL(), listMuted = ListSF.GKAOOOICJAI;
int passed = 0;
Action<bool> check = ok => { if (!ok) throw new Exception("Startup options regression " + passed); passed++; };
try
{
    foreach (var value in new[] { .4f, 0f, .7f })
    {
        SoundController.SetMusicVolume(value);
        SoundController.SetSoundVolume(value);
        check(Mathf.Abs(SoundController.GetMusicVolume() - value) < .001f);
        check(Mathf.Abs(SoundController.GetSoundVolume() - value) < .001f);
    }
    Sound.OAFCOFNOIJK(1f); Sound.JOFLPDCONNC(1f);
    typeof(SoundController).GetMethod("ApplySavedVolumes", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, null);
    check(Mathf.Abs(SoundController.GetMusicVolume() - .7f) < .001f);
    check(Mathf.Abs(SoundController.GetSoundVolume() - .7f) < .001f);
    PlayerPrefs.DeleteKey("Eclipse.LargeControls");
    bool initialSize = GraphicsController.LargeControlsEnabled();
    GraphicsController.ToggleControlSize();
    check(GraphicsController.LargeControlsEnabled() != initialSize);
    GraphicsController.ToggleControlSize();
    check(GraphicsController.LargeControlsEnabled() == initialSize);
    return new { passed, profileLoaded = false };
}
finally
{
    foreach (var key in floatKeys) { if (saved.ContainsKey(key)) PlayerPrefs.SetFloat(key, saved[key]); else PlayerPrefs.DeleteKey(key); }
    foreach (var key in intKeys) { if (saved.ContainsKey(key)) PlayerPrefs.SetInt(key, (int)saved[key]); else PlayerPrefs.DeleteKey(key); }
    PlayerPrefs.Save();
    Sound.OAFCOFNOIJK(music); Sound.JOFLPDCONNC(sound);
    Sound.FMLHEDIPGAF(musicMuted); Sound.FLOFHMBDHNM(soundMuted); ListSF.GKAOOOICJAI = listMuted;
}
