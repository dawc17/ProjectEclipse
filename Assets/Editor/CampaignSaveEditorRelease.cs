using Eclipse.Saves;
using UnityEditor;

// A player releases its campaign lock when the process exits, but the editor keeps
// running after Play mode stops, so the slot stayed locked ("Sharing violation")
// for any other game process until the next domain reload. EnteredEditMode fires
// after the play session's scenes and profile have been torn down.
[InitializeOnLoad]
internal static class CampaignSaveEditorRelease
{
	static CampaignSaveEditorRelease()
	{
		EditorApplication.playModeStateChanged += state =>
		{
			if (state == PlayModeStateChange.EnteredEditMode)
			{
				CampaignSaveSession.Clear();
			}
		};
	}
}
