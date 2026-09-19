// Run with: unity command eval_file --file Tools/VerifyDE128MovePresentationAssets.cs --json
// Read-only import check, without playback, fighters, or save access.
var rows = new System.Collections.Generic.List<object>();
foreach (var name in new[] { "snd_swish_sword1", "snd_swish_sword2", "snd_swish_sword3", "snd_hit1", "snd_hit2", "snd_hit3", "snd_hit4", "snd_hit5", "snd_hit6" })
{
    var clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>("gamedata/sounds/" + name);
    if (clip == null || clip.samples <= 0 || clip.frequency <= 0) throw new Exception("Missing or empty imported sound: " + name);
    rows.Add(new { name, samples = clip.samples, frequency = clip.frequency, channels = clip.channels });
}
var icon = Nekki.SF2.GUI.ResolutionImage.GetSprite("UI/Skills/", "Trick7.super_slash");
if (icon == null || icon.vertices.Length < 3)
    throw new Exception("Missing imported profile icon: Trick7.super_slash");
rows.Add(new { name = "Trick7.super_slash", vertices = icon.vertices.Length });
return rows;
