# Evaluate the live quest XML with the production expression parser/math implementation.
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$source = Get-Content -Raw (Join-Path $root 'Assets/Plugins/Assembly-CSharp-firstpass/ConditionExtension.cs')
$fixture = @'
public static class LLLOJBFMONN
{
    public static void Write(string text, params object[] args) { }
    public static void Error(string text, params object[] args) { throw new System.Exception(text); }
}
public static class NekkiMath
{
    public static int randomInt(int min, int max) { throw new System.NotSupportedException(); }
}
public class StrangerExpressionRegression : ConditionExtension
{
    public int Level;
    public int Trigger;
    protected override void IDHOFHMDIPL(string value, CompareResult result)
    {
        if (value != "_StrangerTriggerLevel") throw new System.Exception("Unexpected variable: " + value);
        result.resultNumber = Trigger;
    }
    protected override void AIFNPKLNPEE(QuestFunctions function, CompareResult result)
    {
        switch (function.FJLOLCPJACB)
        {
            case "Player":
                if (function.HBDLDIKHFEG != "Level") throw new System.Exception("Unknown player property: " + function.HBDLDIKHFEG);
                result.resultNumber = Level;
                break;
            case "Mod": MDENBJJAPMH(function, result, KDEAPAPEEAO.MATH_MOD); break;
            case "Sum": MDENBJJAPMH(function, result, KDEAPAPEEAO.MATH_SUM); break;
            default: throw new System.Exception("Unexpected function: " + function.FJLOLCPJACB);
        }
    }
    public static void Run(string condition, string action)
    {
        var evaluator = new StrangerExpressionRegression();
        for (int level = 1; level <= 52; level++)
        {
            evaluator.Level = level;
            evaluator.Trigger = level + 1;
            var result = new CompareResult();
            evaluator.MCPIOGALBMK(evaluator.ClearGaps(condition), result);
            if (!result.INCOIAANDCO() || result.resultNumber != level % 6)
                throw new System.Exception("Wrong level remainder at " + level);
            result = new CompareResult();
            evaluator.MCPIOGALBMK(action, result);
            if (!result.INCOIAANDCO() || result.resultNumber != evaluator.Trigger + 1)
                throw new System.Exception("Wrong trigger increment at " + level);
        }
        System.Console.WriteLine("PASS: live Stranger quest expressions evaluate correctly at all 52 levels.");
    }
}
'@
Add-Type -TypeDefinition ($source + "`n" + $fixture) -IgnoreWarnings
[xml]$quests = Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/quests.xml')
$quest = $quests.SelectSingleNode('//Quest[@Name="FinetuneStrangerTriggerLevel"]')
$condition = $quest.SelectSingleNode('Conditions/Equal[@Value2="5"]').GetAttribute('Value1')
$action = $quest.SelectSingleNode('Actions/SetVariable[@Name="StrangerTriggerLevel"]').GetAttribute('Value')
[StrangerExpressionRegression]::Run($condition, $action)
