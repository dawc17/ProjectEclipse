using System.Collections.Generic;
using System.Xml;

public static class AnimationData
{
	// best guess for name
	private static Dictionary<string, TemplateAnimation> _TemplatesByName = new Dictionary<string, TemplateAnimation>();

	// best guess for name
	private static readonly Dictionary<string, InfoAnimation> _AnimationsByName = new Dictionary<string, InfoAnimation>();

	// best guess for name
	private static readonly List<InfoAnimation> _Animations = new List<InfoAnimation>();

	private static readonly List<Trick> AGBJABJNGEA = new List<Trick>();

	private static readonly List<Trigger> NMILPLHGCMA = new List<Trigger>();

	private static List<string> _WeaponTypeList = new List<string>();

	// best guess for name
	public static List<InfoAnimation> Animations
	{
		get
		{
			return CCANGHENJAE();
		}
	}

	public static int BCIGIMOHBJH
	{
		get
		{
			return DJDLCMCLOJN();
		}
	}

	public static List<Trick> GCBKPAHELLI
	{
		get
		{
			return BFNFDDLNHPA();
		}
	}

	public static List<Trigger> ANPKEANHHGE
	{
		get
		{
			return GFPPKEAMEBO();
		}
	}

	public static List<string> EDIPADGDGPM
	{
		get
		{
			return LOJEMPOAAKF();
		}
	}

	public static List<InfoAnimation> CCANGHENJAE()
	{
		return _Animations;
	}

	public static int DJDLCMCLOJN()
	{
		return _Animations.Count;
	}

	public static List<Trick> BFNFDDLNHPA()
	{
		return AGBJABJNGEA;
	}

	public static List<Trigger> GFPPKEAMEBO()
	{
		return NMILPLHGCMA;
	}

	public static List<string> LOJEMPOAAKF()
	{
		if (_WeaponTypeList.Count != 0)
		{
			return _WeaponTypeList;
		}
		for (int i = 0; i < _Animations.Count; i++)
		{
			List<string> list = _Animations[i].OIDIJEOMJCB();
			if (list.Count == 0)
			{
				continue;
			}
			bool flag = true;
			foreach (string item in _WeaponTypeList)
			{
				if (list.IndexOf(item) != -1)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			foreach (string item2 in list)
			{
				_WeaponTypeList.Add(item2);
			}
		}
		return _WeaponTypeList;
	}

	public static void Load(string PMFEIPCHENB, bool OOJAEKEOEFJ)
	{
		MovesParser.Parse(PMFEIPCHENB, _Animations, _TemplatesByName, AGBJABJNGEA, NMILPLHGCMA, OOJAEKEOEFJ);
		InfoAnimation pJAHIOELGGD = null;
		for (int i = 0; i < _Animations.Count; i++)
		{
			pJAHIOELGGD = _Animations[i];
			_AnimationsByName[pJAHIOELGGD.Name] = pJAHIOELGGD;
		}
		CreateCapabilityTables();
	}

	internal static int AddExternalMoves(XmlDocument document)
	{
		int before = _Animations.Count;
		int added = MovesParser.ParseAdditional(document, _Animations, _TemplatesByName, AGBJABJNGEA, NMILPLHGCMA);
		if (added == 0) return 0;

		List<InfoAnimation> newMoves = _Animations.GetRange(before, added);
		for (int i = 0; i < newMoves.Count; i++)
			_AnimationsByName.Add(newMoves[i].Name, newMoves[i]);

		// Existing lower-priority moves may now transition into newly added moves. Only
		// compare old entries against the new tail to avoid duplicating established tables.
		for (int i = 0; i < before; i++)
			CreateCapabilityTable(_Animations[i], newMoves);
		for (int i = 0; i < newMoves.Count; i++)
			CreateCapabilityTable(newMoves[i], _Animations);
		_WeaponTypeList.Clear();
		return added;
	}

	internal sealed class ExternalMoveReplacementLifetime : System.IDisposable
	{
		internal sealed class Entry
		{
			internal int Index;
			internal InfoAnimation Original;
			internal InfoAnimation Replacement;
		}

		internal readonly List<Entry> Entries = new List<Entry>();
		internal readonly List<System.Action> TemplateUndo = new List<System.Action>();
		private bool _disposed;

		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;
			for (int i = TemplateUndo.Count - 1; i >= 0; i--) TemplateUndo[i]();
			bool restored = TemplateUndo.Count != 0;
			for (int i = Entries.Count - 1; i >= 0; i--)
			{
				Entry entry = Entries[i];
				if (entry.Index >= _Animations.Count ||
					!ReferenceEquals(_Animations[entry.Index], entry.Replacement)) continue;
				_Animations[entry.Index] = entry.Original;
				if (_AnimationsByName.TryGetValue(entry.Original.Name, out InfoAnimation current) &&
					ReferenceEquals(current, entry.Replacement)) _AnimationsByName[entry.Original.Name] = entry.Original;
				restored = true;
			}
			if (restored) RebuildCapabilityTables();
		}
	}

	internal static ExternalMoveReplacementLifetime ReplaceExternalMoves(XmlDocument document,
		IReadOnlyDictionary<string, string> expectedFiles)
	{
		if (document == null || document["Movesxml"]?["Moves"] == null || expectedFiles == null)
			throw new System.ArgumentException("Native move replacements require moves and expected filenames.");
		if (document.SelectNodes("/Movesxml/Templates/Template").Count != 0 ||
			document.SelectNodes("/Movesxml/Triggers/Trigger").Count != 0)
			throw new System.InvalidOperationException("Native move replacements cannot add templates or triggers.");
		// Parsing appends each replacement to the live template lists it names.
		// Keep a snapshot so a rejected batch leaves template membership unchanged.
		var templateSnapshot = new List<KeyValuePair<List<InfoAnimation>, InfoAnimation[]>>();
		foreach (TemplateAnimation template in _TemplatesByName.Values)
			templateSnapshot.Add(new KeyValuePair<List<InfoAnimation>, InfoAnimation[]>(
				template.LDEBJOPLCKO(), template.LDEBJOPLCKO().ToArray()));
		var templateNames = new HashSet<string>(_TemplatesByName.Keys, System.StringComparer.Ordinal);
		var parsed = new List<InfoAnimation>();
		var lifetime = new ExternalMoveReplacementLifetime();
		try
		{
			MovesParser.ParseAdditional(document, parsed, _TemplatesByName, new List<Trick>(), new List<Trigger>());
			if (parsed.Count != expectedFiles.Count)
				throw new System.InvalidOperationException("Native replacement count does not match its guards.");
			var names = new HashSet<string>(System.StringComparer.Ordinal);
			foreach (InfoAnimation replacement in parsed)
			{
				if (!names.Add(replacement.Name) ||
					!expectedFiles.TryGetValue(replacement.Name, out string expectedFile))
					throw new System.InvalidOperationException("Unguarded or duplicate native replacement: " + replacement.Name);
				int index = -1;
				for (int i = 0; i < _Animations.Count; i++)
					if (_Animations[i].Name == replacement.Name)
					{
						if (index >= 0) throw new System.InvalidOperationException("Ambiguous native replacement target: " + replacement.Name);
						index = i;
					}
				if (index < 0 || _Animations[index].FileName != expectedFile ||
					!_AnimationsByName.TryGetValue(replacement.Name, out InfoAnimation mapped) ||
					!ReferenceEquals(mapped, _Animations[index]))
					throw new System.InvalidOperationException("Native replacement expected filename or target mismatch: " + replacement.Name);
				lifetime.Entries.Add(new ExternalMoveReplacementLifetime.Entry {
					Index = index, Original = _Animations[index], Replacement = replacement });
			}
		}
		catch
		{
			foreach (var list in templateSnapshot)
			{
				list.Key.Clear();
				list.Key.AddRange(list.Value);
			}
			foreach (string name in new List<string>(_TemplatesByName.Keys))
				if (!templateNames.Contains(name)) _TemplatesByName.Remove(name);
			throw;
		}
		try
		{
			foreach (var entry in lifetime.Entries)
			{
				_Animations[entry.Index] = entry.Replacement;
				_AnimationsByName[entry.Replacement.Name] = entry.Replacement;
				foreach (TemplateAnimation template in _TemplatesByName.Values)
					SwapTemplateMember(template, entry.Original, entry.Replacement, lifetime);
			}
			RebuildCapabilityTables();
			return lifetime;
		}
		catch
		{
			lifetime.Dispose();
			throw;
		}
	}

	// A replacement takes the original's place in templates it also declares,
	// and in the per-move template named after the move, so template lookups keep
	// their native order. Templates only the original declared drop it.
	private static void SwapTemplateMember(TemplateAnimation template, InfoAnimation original,
		InfoAnimation replacement, ExternalMoveReplacementLifetime lifetime)
	{
		List<InfoAnimation> members = template.LDEBJOPLCKO();
		int originalIndex = members.IndexOf(original);
		int replacementIndex = members.IndexOf(replacement);
		if (originalIndex < 0)
		{
			if (replacementIndex >= 0)
				lifetime.TemplateUndo.Add(() => members.Remove(replacement));
			return;
		}
		if (replacementIndex >= 0 || template.get_Name() == original.Name)
		{
			if (replacementIndex >= 0) members.RemoveAt(replacementIndex);
			originalIndex = members.IndexOf(original);
			members[originalIndex] = replacement;
			lifetime.TemplateUndo.Add(() =>
			{
				int current = members.IndexOf(replacement);
				if (current >= 0) members[current] = original;
			});
			return;
		}
		members.RemoveAt(originalIndex);
		lifetime.TemplateUndo.Add(() =>
		{
			if (!members.Contains(original)) members.Insert(System.Math.Min(originalIndex, members.Count), original);
		});
	}

	internal static void RebuildCapabilityTables()
	{
		foreach (InfoAnimation move in _Animations)
			move.PriorityConflicts.HigherPriorityMoves.Clear();
		CreateCapabilityTables();
		_WeaponTypeList.Clear();
	}

	public static void BCILLFEBJHK()
	{
		_Animations.Clear();
		_AnimationsByName.Clear();
		InfoAnimation.EGLKBMCHPNN();
		AGBJABJNGEA.Clear();
		_TemplatesByName.Clear();
		NMILPLHGCMA.Clear();
		MovesParser.CHILAIJNEHG();
	}

	public static void CreateCapabilityTables()
	{
		foreach (InfoAnimation lNKJIIGBEDum in _Animations)
		{
			CreateCapabilityTable(lNKJIIGBEDum, _Animations);
		}
	}

	public static void CreateCapabilityTable(InfoAnimation DBOLBEOCEME, List<InfoAnimation> MAHEJFLCCHP)
	{
		List<ConditionKeys> list = DBOLBEOCEME.MOPMGFIIFGA();
		int count = list.Count;
		if (0 >= count)
		{
			return;
		}
		foreach (InfoAnimation item in MAHEJFLCCHP)
		{
			if (DBOLBEOCEME.Priority >= item.Priority)
			{
				continue;
			}
			List<ConditionKeys> list2 = item.MOPMGFIIFGA();
			int count2 = list2.Count;
			if (0 >= count2)
			{
				continue;
			}
			bool flag = false;
			foreach (ConditionKeys item2 in list)
			{
				KeyData fONEJOKEIEN = item2.RequiredKeys;
				foreach (ConditionKeys item3 in list2)
				{
					KeyData fONEJOKEIEN2 = item3.RequiredKeys;
					if (fONEJOKEIEN2.IsVariable(fONEJOKEIEN))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				DBOLBEOCEME.PriorityConflicts.HigherPriorityMoves.Add(item);
			}
		}
	}

	public static void AKJLPGMEFFD(List<InfoAnimation> MAHEJFLCCHP, List<ItemInfo> HELFDCAIJNE, bool ABGINCCBACK = false, List<string> JHJPMONBIDI = null, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight, List<PerkInfoItem> MAFPBEFKNGE = null, List<PerkInfoItem> CFKCGBEONAM = null)
	{
		List<ConditionAnimation> list = new List<ConditionAnimation>();
		ModelConditions dGJJDPIAEAO = new ModelConditions();
		dGJJDPIAEAO.OJIAKDDCGLB = HELFDCAIJNE;
		dGJJDPIAEAO.FDELMAHAAJD = ABGINCCBACK;
		dGJJDPIAEAO.IBBALIJOJMC = NFNJJIGAKNN;
		dGJJDPIAEAO.POBNMMADAJJ = MAFPBEFKNGE;
		dGJJDPIAEAO.CFPLPALGCMK = CFKCGBEONAM;
		foreach (InfoAnimation lNKJIIGBEDum in _Animations)
		{
			list = lNKJIIGBEDum.MoveData.Locks;
			if (lNKJIIGBEDum.HPPGNJJCEGF(dGJJDPIAEAO, list) && (JHJPMONBIDI == null || !lNKJIIGBEDum.CheckAnimationName(JHJPMONBIDI)))
			{
				MAHEJFLCCHP.Add(lNKJIIGBEDum);
			}
		}
	}

	public static void FMDFKKEDMJG(List<Trigger> CMHFKBKKKOK, List<ItemInfo> HELFDCAIJNE, bool ABGINCCBACK = false, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight, List<PerkInfoItem> MAFPBEFKNGE = null, List<PerkInfoItem> CFKCGBEONAM = null)
	{
		CMHFKBKKKOK.Clear();
		List<ConditionAnimation> list = new List<ConditionAnimation>();
		ModelConditions dGJJDPIAEAO = new ModelConditions();
		dGJJDPIAEAO.OJIAKDDCGLB = HELFDCAIJNE;
		dGJJDPIAEAO.FDELMAHAAJD = ABGINCCBACK;
		dGJJDPIAEAO.IBBALIJOJMC = NFNJJIGAKNN;
		dGJJDPIAEAO.POBNMMADAJJ = MAFPBEFKNGE;
		dGJJDPIAEAO.CFPLPALGCMK = CFKCGBEONAM;
		foreach (Trigger item in NMILPLHGCMA)
		{
			list = item.IDEMFOLJIFE.HIFPHBNGIPO;
			if (item.HPPGNJJCEGF(dGJJDPIAEAO, list))
			{
				CMHFKBKKKOK.Add(item);
			}
		}
	}

	public static void OCMIKNOMINM(List<string> NIKHAICFGNM, List<InfoAnimation> OEMALIFPGPO)
	{
		for (int i = 0; i < NIKHAICFGNM.Count; i++)
		{
			NEBELEFIDMB(NIKHAICFGNM[i], OEMALIFPGPO);
		}
	}

	public static void NEBELEFIDMB(string name, List<InfoAnimation> OEMALIFPGPO)
	{
		if (_TemplatesByName.ContainsKey(name))
		{
			if (OEMALIFPGPO.Count == 0)
			{
				OEMALIFPGPO.AddRange(_TemplatesByName[name].LDEBJOPLCKO());
			}
			else
			{
				OEMALIFPGPO.AddIfNotExist(_TemplatesByName[name].LDEBJOPLCKO());
			}
		}
	}

	public static TemplateAnimation ANEMJNGKFDB(string name)
	{
		if (_TemplatesByName.ContainsKey(name))
		{
			return _TemplatesByName[name];
		}
		return null;
	}

	public static InfoAnimation KCHIFIDKLOC(ItemInfo LGCMGHAFEDD)
	{
		if (LGCMGHAFEDD == null)
		{
			return BCIFKBJAFEC("StanceIdle");
		}
		string item = "Stance";
		string mENAJEAJJBE = LGCMGHAFEDD.Name;
		foreach (InfoAnimation lNKJIIGBEDum in _Animations)
		{
			List<string> list = lNKJIIGBEDum.FOLOOGCLPNE();
			List<string> list2 = lNKJIIGBEDum.OIDIJEOMJCB();
			if (((list2.Count == 0 && string.IsNullOrEmpty(mENAJEAJJBE)) || (list2.Count != 0 && list2.IndexOf(mENAJEAJJBE) != -1)) && list.IndexOf(item) != -1)
			{
				return lNKJIIGBEDum;
			}
		}
		return BCIFKBJAFEC("StanceIdle");
	}

	public static void PHNMANPDPKG(List<Trick> IAGDAAPCDNI, List<ItemInfo> HELFDCAIJNE, bool ABGINCCBACK = false, List<string> JHJPMONBIDI = null, List<PerkInfoItem> JOGBKOJCINM = null, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight)
	{
		List<InfoAnimation> list = new List<InfoAnimation>();
		AKJLPGMEFFD(list, HELFDCAIJNE, ABGINCCBACK, JHJPMONBIDI, NFNJJIGAKNN, JOGBKOJCINM);
		foreach (Trick item in AGBJABJNGEA)
		{
			foreach (InfoAnimation item2 in list)
			{
				if (item.KJHMOGGECBN == item2)
				{
					IAGDAAPCDNI.Add(item);
				}
			}
		}
	}

	public static InfoAnimation BCIFKBJAFEC(string name, bool ADCNNABFIDL = true)
	{
		InfoAnimation value = null;
		if (_AnimationsByName.TryGetValue(name, out value))
		{
			return value;
		}
		if (ADCNNABFIDL)
		{
			LLLOJBFMONN.Error("Animation " + name + " not found");
		}
		else
		{
			LLLOJBFMONN.Write("Animation " + name + " not found");
		}
		return null;
	}

	public static void GAPACJBBJKL(List<string> GKHEPKGMEFI, List<InfoAnimation> FKFEKLNOAGE = null)
	{
		List<InfoAnimation> list = ((FKFEKLNOAGE != null) ? FKFEKLNOAGE : _Animations);
		foreach (InfoAnimation item in list)
		{
			InfoAnimation.MovePivot iLOEBFFAEAN = item.MoveData.ILOEBFFAEAN;
			if (iLOEBFFAEAN.CKBGFODEBAJ != InfoAnimation.DOLCEABGNGA.ObjectNodes || iLOEBFFAEAN.EDBLMNIEKBD != ModelType.KEIDBIOIFGA.MODEL_THIS || iLOEBFFAEAN.HHPAGAOGGLP != InfoAnimation.DOLCEABGNGA.ObjectPivot)
			{
				continue;
			}
			string bLODCIGDJFK = item.MoveData.ILOEBFFAEAN.BLODCIGDJFK;
			if (string.IsNullOrEmpty(bLODCIGDJFK))
			{
				continue;
			}
			bool flag = true;
			foreach (string item2 in GKHEPKGMEFI)
			{
				if (item2 == bLODCIGDJFK)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				GKHEPKGMEFI.Add(bLODCIGDJFK);
			}
		}
	}
}
