$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$fixture = Join-Path $root ('Temp/ContentNumberCulture-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
# Isolate the production readers from Unity/resource boot. The native title mesh
# test separately covers their use by real rigs, physics and mesh interpolation.
$xml = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/XmlUtils.cs')
$vector = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Vector3f.cs')
$parse = [regex]::Match($xml, '(?ms)^\tpublic static float ParseFloat\(.*?^\t\}').Value
$create = [regex]::Match($vector, '(?ms)^\tpublic static Vector3f Create\(.*?^\t\}').Value
if (!$parse -or !$create) { throw 'Production numeric reader extraction failed.' }
$program = @'
using System;
using System.Globalization;
using System.Xml;
static class XmlUtils { PARSE }
class Vector3f { public float X, Y; CREATE }
static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static XmlAttribute Attribute(string text)
    {
        var doc = new XmlDocument();
        var attribute = doc.CreateAttribute("Value");
        attribute.Value = text;
        return attribute;
    }
    static void Main()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            foreach (var name in new[] { "hr-HR", "en-US", "de-DE", "fr-FR", "pl-PL", "nb-NO", "ar-EG" })
            {
                var culture = CultureInfo.GetCultureInfo(name);
                CultureInfo.CurrentCulture = culture;
                // NTop and Wicked Veil cloth values from the packaged vanilla models.
                foreach (var sample in new[] { ("-20.5197906494141", -20.5197906494141f),
                    ("283.477600097656", 283.477600097656f), ("0.772934436798096", .772934436798096f),
                    ("9.99999974737875E-6", 9.99999974737875E-6f), ("0.05", .05f),
                    ("0.25", .25f), (" -1.25e+2 ", -125f), ("0", 0f) })
                    Check(Attribute(sample.Item1).ParseFloat(-99f) == sample.Item2,
                        name + " changed model value " + sample.Item1);
                Check(((XmlAttribute)null).ParseFloat(7f) == 7f, "Missing attribute default changed");
                foreach (var invalid in new[] { "", "invalid", "0,25", "1,000.25" })
                    Check(Attribute(invalid).ParseFloat(7f) == 7f, name + " accepted a non-content number " + invalid);
                var doc = new XmlDocument();
                doc.LoadXml("<Point X='-20.5197906494141' Y='283.477600097656' />");
                var point = Vector3f.Create(doc.DocumentElement);
                Check(point.X == -20.5197906494141f && point.Y == 283.477600097656f, name + " changed XML vector coordinates");
                Check(Vector3f.Create(null) == null, "Missing vector default changed");
                Check(CultureInfo.CurrentCulture == culture, "Reader changed the player's numeric culture");
            }
        }
        finally { CultureInfo.CurrentCulture = previous; }
        Console.WriteLine("PASS: " + checks + " production XML numeric-reader checks across seven cultures.");
    }
}
'@
$program.Replace('PARSE', $parse).Replace('CREATE', $create) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
@'
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NuGetAudit>false</NuGetAudit></PropertyGroup></Project>
'@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Content number culture regression failed.' }
