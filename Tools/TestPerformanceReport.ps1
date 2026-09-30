$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/PerformanceReport-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/UI/PerformanceOverlay.cs')
$sample = [regex]::Match($source, '(?ms)^\t\tprivate struct Sample\s*\{.*?^\t\t\}').Value
$append = [regex]::Match($source, '(?ms)^\t\tprivate static void AppendSample\(.*?^\t\t\}').Value
if (!$sample -or !$append) { throw 'Production report writer extraction failed.' }
$program = @'
using System;
using System.Globalization;
using System.Text;
static class Program
{
SAMPLE
APPEND
    static void Main()
    {
        int checks = 0;
        var previous = CultureInfo.CurrentCulture;
        try
        {
            foreach (string name in new[] { "hr-HR", "en-US", "de-DE", "fr-FR", "pl-PL", "nb-NO", "ar-EG" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
                var sample = new Sample {
                    FrameMs = 16.25f, AiMs = .125f, SimMs = .375f, CpuMainMs = 8.5f,
                    RenderThreadMs = 2.25f, GpuMs = 1.75f, AllocKb = 31.5f,
                    Gc = true, Loading = false, InFight = true, SnapshotMs = 4.125f,
                    RestoreMs = .625f, SimTicks = 2
                };
                var text = new StringBuilder();
                AppendSample(text, sample);
                var columns = text.ToString().Split(',');
                double[] expected = { 16.25, .125, .375, 8.5, 2.25, 1.75, 31.5, 1, 0, 1, 4.125, .625 };
                if (columns.Length != expected.Length) throw new Exception(name + " broke CSV column boundaries: " + text);
                checks++;
                for (int i = 0; i < expected.Length; i++)
                {
                    if (double.Parse(columns[i], CultureInfo.InvariantCulture) != expected[i])
                        throw new Exception(name + " changed column " + i + ": " + text);
                    checks++;
                }
                if (CultureInfo.CurrentCulture.Name != name) throw new Exception("Report writer changed the player's culture.");
                checks++;
            }
        }
        finally { CultureInfo.CurrentCulture = previous; }
        Console.WriteLine("PASS: " + checks + " production performance CSV checks across seven cultures.");
    }
}
'@
$program.Replace('SAMPLE', $sample).Replace('APPEND', $append) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
@'
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NuGetAudit>false</NuGetAudit></PropertyGroup></Project>
'@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Performance report regression failed.' }
