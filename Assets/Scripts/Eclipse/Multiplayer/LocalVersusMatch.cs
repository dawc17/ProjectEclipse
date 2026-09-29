using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI.Fight;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    /// <summary>A detached encounter. Its roster node and equipment never belong to a save.</summary>
    public sealed class LocalVersusMatch : FightList
    {
        public LocalVersusSettings Settings { get; }
        public ModelParameters PlayerOne { get; }
        public ModelParameters PlayerTwo { get; }
        private bool _consumed;

        public LocalVersusMatch(LocalVersusSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (!settings.PlayerOneLoadout.IsValid || !settings.PlayerTwoLoadout.IsValid || !VersusRoster.IsArena(settings.Location))
                throw new ArgumentException("The selected matchup uses equipment or an arena that is not on the versus roster.", nameof(settings));
            Name = "1";
            Index = 0;
            FightId = new FightIDS("EclipseLocal", "versus", Name);
            set_Type(BattleType.FightPVP);
            RoundsToWin = settings.WinsRequired;
            RoundTime = settings.RoundTimeSeconds;
            RewardIndex = 1;
            HealthRecovery = 1f;
            Location = settings.Location;
            Music = "fight1_samurai_spirit";
            TrackFightProgress = false;
            Status = ConditionStatus.StatusOpen;
            Battle = new Battle("PVP", Vector2.zero, "versus", "", "", "Local Versus",
                0, 0, "", "", Location, Music, "", "");
            Battle.GetFights().Add(this);
            var rosterDocument = new XmlDocument();
            var rosterNode = rosterDocument.CreateElement("Fight");
            rosterNode.SetAttribute("Name", FightId.ToString());
            rosterDocument.AppendChild(rosterNode);
            SetRosterFight(new RosterFight(rosterNode));
            PlayerOne = PrepareFighter(settings.PlayerOneLoadout, true, settings.PlayerOneName);
            PlayerTwo = PrepareFighter(settings.PlayerTwoLoadout, false, settings.PlayerTwoName, settings.PlayerTwoTactic);
            AddOpponent(PlayerTwo);
        }

        /// <summary>A fighter wearing <paramref name="loadout"/>; also used by the menu's fighter previews.</summary>
        internal static ModelParameters PrepareFighter(VersusLoadout loadout, bool left, string displayName, string aiTactic = null)
        {
            var document = new XmlDocument();
            var warrior = document.CreateElement("Warrior");
            document.AppendChild(warrior);
            warrior.SetAttribute("FirstName", "NAME_SHADOW");
            warrior.SetAttribute("Avatar", "avatar_hero");
            warrior.SetAttribute("Voice", "Male");
            warrior.SetAttribute("Level", "52");
            warrior.SetAttribute("NotAI", "1");
            warrior.SetAttribute("Controlled", "1");
            warrior.SetAttribute("Tactic", "Player");
            // Equipment selects moves and appearance. Both sides use the same combat ratings.
            foreach (var rating in new[] { "HeadDefense", "BodyDefense", "UnarmedDamage", "WeaponDamage", "RangedDamage", "MagicDamage" })
                warrior.SetAttribute(rating, "0");
            var parameters = ListSF.GetInstance().CreateFormParameters(warrior, left);
            parameters.Skeleton = CopyItem(GameUtils.GetDefaultSkeleton());
            parameters.Armor = CopyItem(loadout.Armor);
            parameters.Helm = CopyItem(loadout.Helm);
            parameters.Weapon = CopyItem(loadout.Weapon);
            parameters.Ranged = CopyItem(loadout.Ranged);
            parameters.Magic = CopyItem(loadout.Magic);
            parameters.DisplayName = displayName;
            parameters.AttributeAlignments.Clear();
            parameters.AttributeAlignments.Add(new AttributesAlign());
            parameters.ShieldTotal = 0;
            parameters.HasShieldTotalOverride = true;
            parameters.WarriorPerks.Clear();
            parameters.LearnedPerks.Clear();
            parameters.Perks.Clear();
            GameUtils.InitializeLocalVersusParameters(parameters, left);
            if (!string.IsNullOrEmpty(aiTactic))
            {
                // A training dummy left to the game's own AI.
                var tactic = AiData.GetTacticByName(aiTactic);
                if (tactic != null)
                {
                    parameters.HBFMBOHLKPJ = tactic;
                    parameters.AiControlled = true;
                    parameters.UserControlled = false;
                }
                else Debug.LogWarning("[Versus] No AI tactic named " + aiTactic + "; the dummy stays scripted.");
            }
            ModelLoader.RequireModelDocuments(parameters.ModelDocuments);
            return parameters;
        }

        private static ItemInfo CopyItem(string name)
        {
            var source = ListSF.GetItems().GetItemByName(name);
            if (source == null) throw new InvalidOperationException("Local versus equipment is missing: " + name);
            var item = source.Clone();
            // No enchantments in versus. Innate perks stay: some items (mines, mind throw)
            // only work through them, and enchantments never come from them.
            item.IgnoreInventoryEnchantments = true;
            item.DefaultEnchantments.Clear();
            item.DefaultEnchantmentPreviews.Clear();
            item.ParsedPerks.Clear();
            if (source.NodeXML != null) item.NodeXML = source.NodeXML.CloneNode(true);
            return item;
        }

        internal Fight CreateFight(PreFight presentation, GameController controller)
        {
            if (_consumed) throw new InvalidOperationException("Create a fresh matchup for a rematch.");
            if (presentation == null || controller == null)
                throw new InvalidOperationException("Local versus requires the native fight scene and its controller.");
            _consumed = true;
            var fight = new Fight(this, PlayerOne, new List<ModelParameters> { PlayerTwo }, presentation, controller);
            controller.ConfigureLocalVersusInput(true, Settings.KeyboardPlayerOne);
            controller.gameObject.SetActive(true);
            VersusTickDriver.Begin(fight, LocalVersusSession.CreateInputSource(), Settings.Seed);
            LocalVersusSession.FightReady(fight);
            return fight;
        }
    }
}
