# Exercise the production comparer without initializing Unity or touching saves.
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$source = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/QuestStage.cs')
$method = [regex]::Match($source, '(?ms)^\tpublic int CompareTo\(QuestStage [^\r\n]+\)\r?\n\t\{.*?^\t\}')
if (!$method.Success) { throw 'Cannot find QuestStage.CompareTo' }
$fixture = @'
using System;
using System.Collections.Generic;
public class QuestStage : IComparable<QuestStage>
{
    public enum HPOLGFKCOOE { QUEST_UNCOMPLETE, QUEST_ACTIONS, QUEST_COMPLETE }
    public HPOLGFKCOOE State;
    public int DEFHBAPNPHI;
    public HPOLGFKCOOE MHFPGCBLGIP() { return State; }
    /* COMPARER */
}
public static class QuestStageSortRegression
{
    static void Check(bool ok, string message)
    {
        if (!ok) throw new Exception(message);
    }
    public static void Run()
    {
        var stages = new List<QuestStage>();
        foreach (QuestStage.HPOLGFKCOOE state in Enum.GetValues(typeof(QuestStage.HPOLGFKCOOE)))
            foreach (int priority in new[] { int.MinValue, -9, 0, 0, 15, int.MaxValue })
                stages.Add(new QuestStage { State = state, DEFHBAPNPHI = priority });
        foreach (var a in stages)
        {
            Check(a.CompareTo(a) == 0, "A quest must compare equal to itself");
            Check(a.CompareTo(null) > 0, "Null ordering");
            foreach (var b in stages)
            {
                Check(Math.Sign(a.CompareTo(b)) == -Math.Sign(b.CompareTo(a)), "Antisymmetry");
                foreach (var c in stages)
                    if (a.CompareTo(b) <= 0 && b.CompareTo(c) <= 0)
                        Check(a.CompareTo(c) <= 0, "Transitivity");
            }
        }
        var random = new Random(412);
        for (int pass = 0; pass < 100; pass++)
        {
            // Repeated references and mixed running/pending quests model nested activation.
            var queue = new List<QuestStage>(stages);
            for (int i = 0; i < 100; i++) queue.Add(stages[random.Next(stages.Count)]);
            for (int i = queue.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = queue[i]; queue[i] = queue[j]; queue[j] = temp;
            }
            queue.Sort();
            bool runningSeen = false;
            QuestStage previous = null;
            foreach (var stage in queue)
            {
                bool running = stage.State == QuestStage.HPOLGFKCOOE.QUEST_ACTIONS;
                Check(!runningSeen || running, "Pending quest sorted behind running quest");
                if (previous != null && running == runningSeen)
                    Check(previous.DEFHBAPNPHI >= stage.DEFHBAPNPHI, "Priority must descend within each group");
                runningSeen = running;
                previous = stage;
            }
        }
        Console.WriteLine("PASS: comparer laws, extreme/tied priorities, and 100 mixed quest queue sorts.");
    }
}
'@
Add-Type -TypeDefinition $fixture.Replace('/* COMPARER */', $method.Value)
[QuestStageSortRegression]::Run()
