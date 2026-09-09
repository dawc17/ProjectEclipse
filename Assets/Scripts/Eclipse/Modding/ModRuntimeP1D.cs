using System;
using UnityEngine;

namespace Eclipse.Modding
{
    public static partial class ModRuntime
    {
        public static void ApplyLocaleMetadata()
        {
            if (_legacyContent == null) return;
            try
            {
                ExternalLocaleRuntime.Apply();
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to bind mod locales; disabling external mods without restarting the game parser. " + exception);
                Shutdown();
            }
        }

        public static void ApplyP1DContent()
        {
            if (_legacyContent == null) return;
            try
            {
                _legacyContent.ApplyP1DContent();
                Debug.Log("[ModContent] Applied P1D content: " + Scripts.Content.LocaleMetadata.Count +
                    " locales, " + Scripts.Content.Locations.Count + " locations, " +
                    Scripts.Content.MoveTemplates.Count + " move templates, " + Scripts.Content.Moves.Count +
                    " moves, " + Scripts.Content.MoveTriggers.Count + " move triggers, " +
                    Scripts.Content.Tactics.Count + " tactics.");
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to apply P1D content; mod startup is invalid. " + exception);
                throw;
            }
        }
    }
}
