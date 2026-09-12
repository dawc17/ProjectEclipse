# Execute production candidate exclusion/lifetime/filtering with controlled item/perk services.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$source = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Recipe.cs')
function Extract([string]$pattern) {
    $match = [regex]::Match($source, $pattern)
    if (!$match.Success) { throw "Recipe source boundary changed: $pattern" }
    return $match.Value
}
$methods = @(
    (Extract '(?ms)^\tprivate sealed class DeviationOverride.*?^\t\}'),
    (Extract '(?ms)^\tprivate PerkStruct CopyCandidateForItem\(.*?^\t\}'),
    (Extract '(?ms)^\tprivate sealed class NativeCandidateExclusion.*?^\t\}'),
    (Extract '(?ms)^\tinternal bool TryExcludeNativeCandidate\(.*?^\t\}'),
    (Extract '(?ms)^\tprivate bool IsNativeCandidateExcluded\(.*?^\t\}'),
    (Extract '(?ms)^\tpublic List<PerkStruct> GetPossibleEnchantments\(.*?^\t\}'),
    (Extract '(?ms)^\tprivate sealed class ExternalCandidate.*?^\t\}')
)
$fixture = @'
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace ForgeCandidateFixture {
public class PerkStruct {
 public List<KeyValuePair<string,string>> Pairs=new List<KeyValuePair<string,string>>();
 public string Name; public PerkStruct(string name){Name=name;} public PerkStruct(PerkStruct other){Name=other.Name;}
 public string get_Name(){return Name;}
}
public class ItemInfo { public string Type; }
public class RecipeItem { public string ItemType; public int MinDeviation,MaxDeviation; }
public class UserItem { public ItemInfo Info; public List<PerkStruct> JAJNJAIJOPA=new List<PerkStruct>(); }
public class Variation {
 public List<PerkStruct> Enchantments=new List<PerkStruct>();
 public bool CheckConditions(UserItem item,int level){return level>=1;}
}
public class Recipe {
 readonly Dictionary<string,DeviationOverride> _deviationOverrides=new Dictionary<string,DeviationOverride>(StringComparer.Ordinal);
 readonly Dictionary<string,HashSet<string>> _excludedNativeCandidates=new Dictionary<string,HashSet<string>>(StringComparer.Ordinal);
 readonly List<Variation> _variations=new List<Variation>();
 readonly Dictionary<string,List<ExternalCandidate>> _externalEnchantments=new Dictionary<string,List<ExternalCandidate>>(StringComparer.Ordinal);
 object GetRecipeItemByType(string kind){return kind=="Weapon"||kind=="Armor"?this:null;}
 static ItemInfo CurrentInfo(UserItem item){return item.Info;}
 static bool IsPerkReadyToEnchant(PerkStruct perk){return perk!=null;}
 static bool IsEnchantmentAlreadyExists(PerkStruct perk,List<PerkStruct> existing){return existing.Any(p=>p.Name==perk.Name);}
 public void Native(params string[] names){var row=new Variation();row.Enchantments.AddRange(names.Select(n=>new PerkStruct(n)));_variations.Add(row);}
 public void External(string kind,string name,int min,int max){_externalEnchantments[kind]=new List<ExternalCandidate>{new ExternalCandidate{Perk=new PerkStruct(name),MinLevel=min,MaxLevel=max}};}
 public int NativeCount(){return _variations.Sum(v=>v.Enchantments.Count);}
 /* METHODS */
}
public static class Tests {
 static int checks;
 static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
 static string Pool(Recipe recipe,UserItem item,int level=10,bool required=false){return string.Join(",",recipe.GetPossibleEnchantments(item,level,required).Select(p=>p.Name));}
 public static void Run(){
  var recipe=new Recipe();recipe.Native("set","ordinary");recipe.Native("set");
  var weapon=new UserItem{Info=new ItemInfo{Type="Weapon"}};var armor=new UserItem{Info=new ItemInfo{Type="Armor"}};
  Check(Pool(recipe,weapon)=="set,ordinary,set","Unexpected baseline pool");
  Check(recipe.TryExcludeNativeCandidate("Weapon","set",out var first),"Valid exclusion rejected");
  Check(Pool(recipe,weapon)=="ordinary","All native occurrences were not excluded");
  Check(Pool(recipe,armor)=="set,ordinary,set","Exclusion leaked across equipment categories");
  Check(recipe.NativeCount()==3,"Exclusion mutated native variation data");
  Check(!recipe.TryExcludeNativeCandidate("Weapon","set",out var duplicate)&&duplicate==null,"Conflicting exclusion accepted");
  Check(!recipe.TryExcludeNativeCandidate("Weapon","unknown",out var missing)&&missing==null,"Missing native candidate accepted");
  Check(!recipe.TryExcludeNativeCandidate("Invalid","set",out missing)&&missing==null,"Missing equipment category accepted");
  recipe.External("Weapon","set",5,20);
  Check(Pool(recipe,weapon)=="ordinary,set","Native exclusion hid an independently added replacement");
  Check(Pool(recipe,weapon,21)=="ordinary","External replacement lost level eligibility");
  weapon.JAJNJAIJOPA.Add(new PerkStruct("ordinary"));
  Check(Pool(recipe,weapon,10,true)=="set","Existing enchantment filter changed");
  Check(Pool(recipe,weapon,0)=="","Native/external eligibility bypassed");
  Check(recipe.TryExcludeNativeCandidate("Weapon","ordinary",out var second),"Independent exclusion rejected");
  Check(Pool(recipe,weapon)=="set","Independent exclusions did not compose");
  first.Dispose();
  Check(Pool(recipe,weapon)=="set,set,set","Restoring one exclusion disturbed another");
  Check(recipe.TryExcludeNativeCandidate("Weapon","set",out var renewed),"Restored candidate could not be excluded again");
  first.Dispose();Check(Pool(recipe,weapon)=="set","Old lifetime disposed a newer exclusion");
  second.Dispose();renewed.Dispose();renewed.Dispose();
  Check(Pool(recipe,weapon)=="set,ordinary,set,set","Disposal did not restore original native order and duplicates");
  Check(recipe.GetPossibleEnchantments(null,10).Count==0,"Null item handling changed");
  Console.WriteLine("PASS: "+checks+" native forge candidate exclusion, composition, restoration and filtering checks; item/perk services controlled.");
 }
}
}
'@
Add-Type -TypeDefinition $fixture.Replace('/* METHODS */',($methods -join "`n"))
[ForgeCandidateFixture.Tests]::Run()
