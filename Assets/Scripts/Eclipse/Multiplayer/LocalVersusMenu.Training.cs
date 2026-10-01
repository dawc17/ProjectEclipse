using System;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    // Training: the setup page (you, the dummy, arena and behaviour) and the in-fight training menu.
    public sealed partial class LocalVersusMenu
    {
        private VersusLoadout trainingPlayer, trainingDummy;
        private string trainingArena;
        private Text trainingArenaName;
        private RawImage trainingArenaThumb;
        /// <summary>The dummy's control when the current training fight was built (the AI is set at build time).</summary>
        private DummyControl trainingBuiltControl;

        public void ShowTraining()
        { ShowOnlineHome(); }

        private void EditTrainingPlayer() => ShowArmory("You", trainingPlayer, loadout => { trainingPlayer = loadout; VersusLoadouts.Save(VersusLoadouts.PlayerOne, loadout); ShowTraining(); });
        private void EditTrainingDummy() => ShowArmory("The dummy", trainingDummy, loadout => { trainingDummy = loadout; VersusLoadouts.Save(VersusLoadouts.Dummy, loadout); ShowTraining(); });

        private void OpenTrainingArenaPicker()
        {
            VersusStagePicker.Open(trainingArena, false, picked =>
            {
                trainingArena = picked;
                SetBackdropArena(picked);
                RefreshTrainingArena();
            });
        }

        private void RefreshTrainingArena()
        {
            if (trainingArenaName != null) trainingArenaName.text = VersusRoster.ArenaName(trainingArena).ToUpperInvariant();
            BindArenaPicture(trainingArenaThumb, trainingArena);
        }

        private static string DummySummary()
        {
            switch (VersusTraining.Control)
            {
                case DummyControl.Cpu: return "CPU  ·  " + VersusTraining.CpuNames[Mathf.Clamp(VersusTraining.CpuLevel, 0, VersusTraining.CpuNames.Length - 1)].ToUpperInvariant();
                case DummyControl.Playback: return "PLAYS BACK YOUR RECORDING";
                case DummyControl.Record: return "RECORDING";
                default: return (VersusTraining.Stance + (VersusTraining.Block != DummyBlock.Never ? "  ·  BLOCK " + VersusTraining.Block : "")).ToUpperInvariant();
            }
        }

        /// <summary>The dummy and rule choices, shared by the setup page and the in-fight menu.</summary>
        private void AddTrainingChoices(RectTransform list, float valueWidth)
        {
            AddChoice(list, "DUMMY", () => VersusTraining.Control == DummyControl.Cpu ? "CPU" : VersusTraining.Control == DummyControl.Playback ? "PLAYBACK" :
                VersusTraining.Control == DummyControl.Record ? "RECORDING" : "SCRIPTED", () =>
            {
                VersusTraining.Control = VersusTraining.Control == DummyControl.Script ? DummyControl.Cpu :
                    VersusTraining.Control == DummyControl.Cpu ? (VersusTraining.Recording.Count > 0 ? DummyControl.Playback : DummyControl.Script) : DummyControl.Script;
            }, valueWidth);
            AddChoice(list, "CPU LEVEL", () => VersusTraining.CpuNames[Mathf.Clamp(VersusTraining.CpuLevel, 0, VersusTraining.CpuNames.Length - 1)].ToUpperInvariant(),
                () => VersusTraining.CpuLevel = (VersusTraining.CpuLevel + 1) % VersusTraining.CpuNames.Length, valueWidth);
            AddChoice(list, "STANCE", () => StanceName(VersusTraining.Stance), () => VersusTraining.Stance = (DummyStance)(((int)VersusTraining.Stance + 1) % 5), valueWidth);
            AddChoice(list, "BLOCK", () => BlockName(VersusTraining.Block), () => VersusTraining.Block = (DummyBlock)(((int)VersusTraining.Block + 1) % 4), valueWidth);
            AddChoice(list, "HEALTH", () => VersusTraining.Health == TrainingHealth.Infinite ? "INFINITE" : VersusTraining.Health == TrainingHealth.RefillAfterCombo ? "REFILL AFTER COMBO" : "NORMAL",
                () => VersusTraining.Health = (TrainingHealth)(((int)VersusTraining.Health + 1) % 3), valueWidth);
        }

        private static string StanceName(DummyStance stance)
        {
            switch (stance)
            {
                case DummyStance.Crouch: return "CROUCH";
                case DummyStance.Jump: return "JUMP";
                case DummyStance.WalkIn: return "WALK IN";
                case DummyStance.WalkAway: return "WALK AWAY";
                default: return "STAND";
            }
        }

        private static string BlockName(DummyBlock block)
        {
            switch (block)
            {
                case DummyBlock.Always: return "ALWAYS";
                case DummyBlock.AfterFirstHit: return "AFTER FIRST HIT";
                case DummyBlock.Random: return "RANDOM";
                default: return "NEVER";
            }
        }

        private void StartTraining()
        {
            if (VersusTraining.Control == DummyControl.Record) VersusTraining.Control = DummyControl.Script;
            var settings = new LocalVersusSettings(trainingPlayer, trainingDummy, trainingArena, true, 1, 300, VersusMode.Training, "You", "Dummy",
                playerTwoTactic: VersusTraining.Control == DummyControl.Cpu ? VersusTraining.CpuTactic : null);
            trainingBuiltControl = VersusTraining.Control;
            try
            {
                LocalVersusSession.StartMatch(settings, () => new TrainingInputSource());
                TrainingHud.Begin();
            }
            catch (Exception exception) { Debug.LogException(exception); SetStatus(exception.Message); }
        }

        /// <summary>The training menu over the paused fight: every option live, plus reset and exit.</summary>
        private void ShowTrainingMenu(string reason)
        {
            page = Page.Pause;
            Rebuild("TRAINING", string.IsNullOrEmpty(reason) ? "Paused" : reason, body =>
            {
                // Two columns: the dummy on the left, the view and actions on the right.
                var columns = Rect(body, "Columns");
                columns.gameObject.AddComponent<LayoutElement>().preferredHeight = 266;
                var split = columns.gameObject.AddComponent<HorizontalLayoutGroup>();
                split.spacing = 40; split.childControlWidth = split.childControlHeight = true; split.childForceExpandWidth = true; split.childForceExpandHeight = true;
                var left = TrainingColumn(columns);
                AddTrainingChoices(left, 230);
                var right = TrainingColumn(columns);
                AddChoice(right, "SPEED", () => VersusTraining.Speeds[Mathf.Clamp(VersusTraining.SpeedIndex, 0, VersusTraining.Speeds.Length - 1)] + "x",
                    () => VersusTraining.SpeedIndex = (VersusTraining.SpeedIndex + 1) % VersusTraining.Speeds.Length, 230);
                AddChoice(right, "HITBOXES", () => Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes ? "SHOWN" : "HIDDEN",
                    () => Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes = !Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes, 230);
                AddChoice(right, "INPUTS", () => VersusTraining.ShowInputs ? "SHOWN" : "HIDDEN", () => VersusTraining.ShowInputs = !VersusTraining.ShowInputs, 230);
                AddButton(right, "RESET POSITIONS", () => { Fight.GetCurrentFight()?.TrainingPlaceFighters(false); VersusTraining.ResetReadouts(); ResumeTraining(); });
                AddButton(right, "RECORD DUMMY", () => { VersusTraining.Recording.Clear(); VersusTraining.Control = DummyControl.Record; ResumeTraining(); });
                var exits = AddRow(body);
                AddButton(exits, "RESUME", ResumeTraining, 0, UiSound.Back);
                AddButton(exits, "LOADOUTS & ARENA", () => { LocalVersusSession.ShowMultiplayerHome(); ShowTraining(); }, 0);
                AddButton(exits, "EXIT TRAINING", LocalVersusSession.ShowMultiplayerHome, 0, UiSound.Back);
            }, ResumeTraining, 1120);
            SetStatus("Esc resumes.   Record dummy: you play the dummy until Page Up, then it repeats your moves.");
        }

        private RectTransform TrainingColumn(RectTransform parent)
        {
            var column = Rect(parent, "Column");
            var layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 9; layout.childControlHeight = layout.childControlWidth = true; layout.childForceExpandHeight = false; layout.childForceExpandWidth = true;
            return column;
        }

        private void ResumeTraining()
        {
            // The game AI is chosen when the dummy is built; switching to or from it starts over,
            // as does resuming once a round with normal health has ended the match.
            bool wantsCpu = VersusTraining.Control == DummyControl.Cpu, builtCpu = trainingBuiltControl == DummyControl.Cpu;
            if (LocalVersusSession.HasResult || wantsCpu != builtCpu ||
                VersusTraining.Control == DummyControl.Cpu && LocalVersusSession.Settings?.PlayerTwoTactic != VersusTraining.CpuTactic)
            {
                LocalVersusSession.ShowMultiplayerHome();
                StartTraining();
                return;
            }
            Resume();
        }
    }
}
