$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$fixture = Join-Path $root ('Temp/LocalVersusRules-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture -Force | Out-Null

$program = @'
using System;
using Eclipse.Multiplayer;

// This fixture exercises settings validation and round resolution. The loadout
// is a controlled value holder; catalog loading and equipment are Unity concerns.
namespace Eclipse.Multiplayer
{
    public sealed class VersusLoadout
    {
        public string Weapon { get; }
        public VersusLoadout(string weapon) { Weapon = weapon; }
    }
}

static class Program
{
    static int checks;

    static LocalVersusSettings Settings(string left, string right, string arena, bool keyboard,
        int wins = 2, int seconds = 99) => new LocalVersusSettings(
            new VersusLoadout(left), new VersusLoadout(right), arena, keyboard, wins, seconds);

    static void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new Exception(message);
    }

    static void Throws<T>(Action action, string message) where T : Exception
    {
        checks++;
        try { action(); }
        catch (T) { return; }
        throw new Exception(message);
    }

    static void Main()
    {
        var settings = Settings("KATANA", "STAFF", "DOJO", true, 3, 120);
        Check(settings.PlayerOneWeapon == "KATANA" && settings.PlayerTwoWeapon == "STAFF" &&
              settings.Location == "DOJO" && settings.KeyboardPlayerOne &&
              settings.WinsRequired == 3 && settings.RoundTimeSeconds == 120,
              "Settings did not retain constructor values.");

        var type = typeof(LocalVersusSettings);
        foreach (var property in type.GetProperties())
            Check(!property.CanWrite, "Settings property is mutable: " + property.Name);

        foreach (string invalid in new[] { null, "", "   " })
        {
            Throws<ArgumentException>(() => Settings(invalid, "B", "ARENA", false), "Invalid P1 weapon accepted.");
            Throws<ArgumentException>(() => Settings("A", invalid, "ARENA", false), "Invalid P2 weapon accepted.");
            Throws<ArgumentException>(() => Settings("A", "B", invalid, false), "Invalid arena accepted.");
        }
        Throws<ArgumentException>(() => new LocalVersusSettings(null, settings.PlayerTwoLoadout, "ARENA", false), "Null P1 loadout accepted.");
        Throws<ArgumentException>(() => new LocalVersusSettings(settings.PlayerOneLoadout, null, "ARENA", false), "Null P2 loadout accepted.");

        foreach (int wins in new[] { 0, -1, 6, int.MaxValue })
            Throws<ArgumentOutOfRangeException>(() => Settings("A", "B", "ARENA", false, wins, 99), "Invalid wins bound accepted: " + wins);
        foreach (int seconds in new[] { 0, 29, 301, int.MaxValue })
            Throws<ArgumentOutOfRangeException>(() => Settings("A", "B", "ARENA", false, 2, seconds), "Invalid round time accepted: " + seconds);
        Check(Settings("A", "B", "ARENA", false, 1, 30).WinsRequired == 1, "Minimum bounds rejected.");
        Check(Settings("A", "B", "ARENA", false, 5, 300).RoundTimeSeconds == 300, "Maximum bounds rejected.");

        Check(LocalVersusRoundRules.ResolveWinner(1f, .25f) == 0, "P1 win resolved incorrectly.");
        Check(LocalVersusRoundRules.ResolveWinner(.1f, .9f) == 1, "P2 win resolved incorrectly.");
        Check(LocalVersusRoundRules.ResolveWinner(.75f, .75f) == -1, "Equal-health timeout was not a draw.");
        Check(LocalVersusRoundRules.ResolveWinner(0f, 0f) == -1, "Double KO was not a draw.");
        Check(LocalVersusRoundRules.ResolveWinner(.5f, .5000005f) == -1, "Near-equal health outside intended draw tolerance.");
        Check(LocalVersusRoundRules.ResolveWinner(.5f, .500002f) == 1, "Meaningful health difference incorrectly treated as draw.");

        foreach (float invalid in new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity })
        {
            Throws<ArgumentException>(() => LocalVersusRoundRules.ResolveWinner(invalid, .5f), "Invalid P1 health accepted.");
            Throws<ArgumentException>(() => LocalVersusRoundRules.ResolveWinner(.5f, invalid), "Invalid P2 health accepted.");
        }

        Console.WriteLine("PASS: " + checks + " LocalVersusSettings/LocalVersusRoundRules production checks (controlled loadouts; no Unity playtest).");
    }
}
'@

$program | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$source = [Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Multiplayer/LocalVersusSettings.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup>
  <ItemGroup><Compile Include="$source" /></ItemGroup>
</Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')

dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Local versus settings/rules regression failed.' }
