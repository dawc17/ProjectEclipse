using System.Collections.Generic;
using System.Diagnostics;
using Nekki.Audio;
using UnityEngine;

public static class Sound
{
	private static int FINECDOIGAH = 0;

	private static int OBCGCEHIFBH = 1;

	private static int AEAIOKEFHGG = 10;

	private static string NEBOIGHENOB = "sounds/";

	private static string BLMBLOKPMEC = "music/";

	private static string LBHNIGEOLPG = ".wav";

	private static string EJGKHALAMAG = ".ogg";

	private static float JLGDLCJBGAC = 1f;

	private static float IOJKBCBFHKJ = 1f;

	private static bool CFMLCAOIFIM = false;

	private static bool KDIFBILDPCK = false;

	private static bool MLELLNBHONP = false;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool DBHAKEPIFCD;

	private static List<KeyValuePair<string, uint>> BEKCBIJGMPE = new List<KeyValuePair<string, uint>>();

	private static List<KeyValuePair<string, uint>> HLLDPAAADKK = new List<KeyValuePair<string, uint>>();

	private static readonly HashSet<string> MissingAudioWarnings = new HashSet<string>();

	// The migrated stage data uses modern, descriptive music ids while the
	// recovered classic soundtrack keeps its original numbered filenames.
	private static readonly Dictionary<string, string> RecoveredMusicAliases =
		new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
		{
			{ "samurai_spirit", "fight1_samurai_spirit" },
			{ "blade_dance", "fight2_blade_dance" },
			{ "vengeance", "fight3_vengeance" },
			{ "forest_of_death", "fight4_forest_of_death" },
			{ "ninja_in_the_night", "fight5_ninja_in_the_night" },
			{ "ninja_in_the_night_old", "fight5_ninja_in_the_night" },
			{ "sparring", "fight6_sparring" },
			{ "fat_boss", "fight7_fat_boss" },
			{ "final_boss", "fight8_final_boss" },
			{ "master_skills", "fight9_master_skills" },
			{ "black_warrior", "fight10_black_warrior" },
			{ "ronin", "fight11_ronin" },
			{ "deadly_smoke", "fight12_deadly_smoke" },
			{ "deadly_smoke_old", "fight12_deadly_smoke" },
			{ "old_sensei", "fight13_old_sensei" },
			{ "old_sensei_old", "fight13_old_sensei" },
			{ "ship_battle", "fight14_ship_battle" },
			{ "shadow_lady", "fight15_shadow_lady" },
			{ "the_battlefield_flowers", "fight16_the_battlefield_flowers" },
			{ "cave", "fight17_cave" },
			{ "fuji", "fight18_fuji" },
			{ "volcano", "fight19_volcano" },
			{ "bridge_to_the_other_side", "fight20_bridge_to_the_other_side" },
			{ "lesson_in_the_dark_room", "fight21_lesson_in_the_dark_room" },
			{ "heavenly_clouds", "fight22_heavenly_clouds" },
			{ "burning_town", "fight23_burning_town" },
			{ "burning_town_old", "fight23_burning_town" },
			{ "ruins_village", "fight24_ruins_village" },
			{ "hive", "fight25_hive" },
			{ "factory", "fight27_factory" },
			{ "flying_rocks", "fight28_flying_rocks" },
			{ "gates_of_shadows", "fight30_gates_of_shadows" },
			{ "graveyard_ships", "fight31_graveyard_ships" },
			{ "starship", "fight32_starship" },
			{ "stone_forest", "fight33_stone_forest" },
			{ "halls_of_the_dead_heroes", "fight34_halls_of_the_dead_heroes" },
			{ "stardocks", "fight36_stardocks" },
			// These newer ids have no matching clip in the recovered pack. Use the
			// closest location/theme track rather than collapsing them all to fight 1.
			{ "deep", "fight34_halls_of_the_dead_heroes" },
			{ "dao_temple", "fight21_lesson_in_the_dark_room" },
			{ "fight38_sakura_forest", "fight4_forest_of_death" },
			{ "sky_isles", "fight22_heavenly_clouds" },
			{ "spaceship", "fight32_starship" },
			{ "stone_dragon", "fight19_volcano" },
			{ "the_monastery", "fight13_old_sensei" }
		};

	public static uint MaxPlayableSounds = 10u;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string HLMKAFLMMNL;

	public static string CHLDAFKHCBL
	{
		get
		{
			return AJLAODNPHFB();
		}
		private set
		{
			IDLDLAEOBBE(value);
		}
	}

	public static string NEJNHNCNOOE
	{
		get
		{
			return MEPDBBPHNMD();
		}
		private set
		{
			JJIFAIOCFFM(value);
		}
	}

	public static string AGGFPFFFBNH
	{
		get
		{
			return GMGNDGCHFFG();
		}
		private set
		{
			KPKNIEIMACC(value);
		}
	}

	public static string EDEIODDLONI
	{
		get
		{
			return OFDOCPONLGM();
		}
		private set
		{
			BBBCFOKHGCA(value);
		}
	}

	public static float ECCOGGCFLPF
	{
		get
		{
			return NBHPABEBLOP();
		}
		set
		{
			JOFLPDCONNC(value);
		}
	}

	public static float JBNOHFLLGPL
	{
		get
		{
			return EAIGFAPKILL();
		}
		set
		{
			OAFCOFNOIJK(value);
		}
	}

	public static bool JNCMBHENOIM
	{
		get
		{
			return BLCCOPHEKGL();
		}
		set
		{
			AGPDEMFFICJ(value);
		}
	}

	public static bool BMDKHPCCFGB
	{
		get
		{
			return AAFLCDKJEPL();
		}
		set
		{
			FLOFHMBDHNM(value);
		}
	}

	public static bool DBLLOGFKAGN
	{
		get
		{
			return ELHMADOKHHE();
		}
		set
		{
			FMLHEDIPGAF(value);
		}
	}

	public static bool ADKBINPKGLO
	{
		get
		{
			return AGCEHOJAJBK();
		}
	}

	public static bool IALIMAPIFHP
	{
		get
		{
			return ABIACJPDIKP();
		}
		private set
		{
			BOLPPMPALJJ(value);
		}
	}

	public static string DLFOPJKPMNC
	{
		get
		{
			return PJDJEAPBNLF();
		}
		private set
		{
			MDAIMFGPCEG(value);
		}
	}

	public static string AJLAODNPHFB()
	{
		return NEBOIGHENOB;
	}

	private static void IDLDLAEOBBE(string value)
	{
		NEBOIGHENOB = value;
		if (NEBOIGHENOB[NEBOIGHENOB.Length - 1] != '/')
		{
			NEBOIGHENOB += "/";
		}
	}

	public static string MEPDBBPHNMD()
	{
		return BLMBLOKPMEC;
	}

	private static void JJIFAIOCFFM(string value)
	{
		BLMBLOKPMEC = value;
		if (BLMBLOKPMEC[BLMBLOKPMEC.Length - 1] != '/')
		{
			BLMBLOKPMEC += "/";
		}
	}

	public static string GMGNDGCHFFG()
	{
		return LBHNIGEOLPG;
	}

	private static void KPKNIEIMACC(string value)
	{
		LBHNIGEOLPG = value;
		if (LBHNIGEOLPG[0] != '.')
		{
			LBHNIGEOLPG.Insert(0, ".");
		}
	}

	public static string OFDOCPONLGM()
	{
		return EJGKHALAMAG;
	}

	private static void BBBCFOKHGCA(string value)
	{
		EJGKHALAMAG = value;
		if (EJGKHALAMAG[0] != '.')
		{
			EJGKHALAMAG.Insert(0, ".");
		}
	}

	public static float NBHPABEBLOP()
	{
		return JLGDLCJBGAC;
	}

	public static void JOFLPDCONNC(float value)
	{
		JLGDLCJBGAC = Mathf.Clamp(value, 0f, 1f);
		FAEHODPALBB(JLGDLCJBGAC);
	}

	public static float EAIGFAPKILL()
	{
		return IOJKBCBFHKJ;
	}

	public static void OAFCOFNOIJK(float value)
	{
		IOJKBCBFHKJ = Mathf.Clamp(value, 0f, 1f);
		SetVolumeToChannel(FINECDOIGAH, value, MLELLNBHONP);
	}

	public static bool BLCCOPHEKGL()
	{
		return CFMLCAOIFIM;
	}

	public static void AGPDEMFFICJ(bool value)
	{
		CFMLCAOIFIM = value;
		FLOFHMBDHNM(value);
		FMLHEDIPGAF(value);
	}

	public static bool AAFLCDKJEPL()
	{
		return KDIFBILDPCK;
	}

	public static void FLOFHMBDHNM(bool value)
	{
		if (value != KDIFBILDPCK)
		{
			KDIFBILDPCK = value;
			PPBDLGFBOKL(value);
		}
	}

	public static bool ELHMADOKHHE()
	{
		return MLELLNBHONP;
	}

	public static void FMLHEDIPGAF(bool value)
	{
		if (value != MLELLNBHONP)
		{
			MLELLNBHONP = value;
			SetMuteToChannel(FINECDOIGAH, MLELLNBHONP);
		}
	}

	public static bool AGCEHOJAJBK()
	{
		return AudioManager.IsPlaying(FINECDOIGAH);
	}

	public static bool ABIACJPDIKP()
	{
		return DBHAKEPIFCD;
	}

	private static void BOLPPMPALJJ(bool value)
	{
		DBHAKEPIFCD = value;
	}

	public static string PJDJEAPBNLF()
	{
		return HLMKAFLMMNL;
	}

	private static void MDAIMFGPCEG(string value)
	{
		HLMKAFLMMNL = value;
	}

	public static void NMKBJANLIEO(string EGPPPNJHNMF, string ADLELPHJADH)
	{
		IDLDLAEOBBE(EGPPPNJHNMF);
		JJIFAIOCFFM(ADLELPHJADH);
	}

	public static void HCFBIFOFGLC(string IBFNOCDNNDB, string NHAKIADKLPG)
	{
		KPKNIEIMACC(IBFNOCDNNDB);
		BBBCFOKHGCA(NHAKIADKLPG);
	}

	public static void SetVolume(float MJDCMAEEIPJ, float FEFBNAOBBBE)
	{
		JOFLPDCONNC(MJDCMAEEIPJ);
		OAFCOFNOIJK(FEFBNAOBBBE);
	}

	private static void FAEHODPALBB(float MJDCMAEEIPJ)
	{
		for (int i = OBCGCEHIFBH; i < AEAIOKEFHGG; i++)
		{
			SetVolumeToChannel(i, MJDCMAEEIPJ, KDIFBILDPCK);
		}
	}

	private static void SetVolumeToChannel(int LMGPAGINHGD, float ONHAHMIHGJC, bool NGHNGOJHJDE)
	{
		AudioManager.SetVolume(ONHAHMIHGJC, LMGPAGINHGD);
		SetMuteToChannel(LMGPAGINHGD, NGHNGOJHJDE);
	}

	private static void PPBDLGFBOKL(bool JFIDKIMPPDH)
	{
		for (int i = OBCGCEHIFBH; i < AEAIOKEFHGG; i++)
		{
			SetMuteToChannel(i, JFIDKIMPPDH);
		}
	}

	private static void SetMuteToChannel(int ADNDLGKIJJK, bool JFIDKIMPPDH)
	{
		if (JFIDKIMPPDH)
		{
			AudioManager.Mute(ADNDLGKIJJK);
		}
		else
		{
			AudioManager.UnMute(ADNDLGKIJJK);
		}
	}

	public static int IFKCCDAIADF(string DPBKBKDCIOI, bool KKHJAJFEPPA = false, float JIJAJFEJJHK = 1f)
	{
		bool flag = AudioManager.CheckAudioLoaded(DPBKBKDCIOI);
		if (!flag)
		{
			flag = IOIEJHLMBLI(DPBKBKDCIOI, JIJAJFEJJHK);
		}
		int num = -1;
		if (flag)
		{
			num = AONDDFIDNJE();
			MDGPOBCKJMJ(num, DPBKBKDCIOI, KKHJAJFEPPA, KDIFBILDPCK);
			KCLJDFLJMDO(DPBKBKDCIOI, (uint)num);
			if (KKHJAJFEPPA)
			{
				EOIPDBEELIJ(DPBKBKDCIOI, (uint)num);
			}
		}
		else
		{
			// Newer animation data references a handful of optional sounds that
			// were not present in the recovered client. Missing sound effects must
			// not be treated as gameplay errors (or be logged every animation
			// frame), but retain one useful diagnostic with the real resource name.
			string text = string.IsNullOrEmpty(DPBKBKDCIOI) ? "<empty>" : DPBKBKDCIOI;
			if (MissingAudioWarnings.Add("sound:" + text))
			{
				UnityEngine.Debug.LogWarning("[Audio] Missing optional sound '" + text + "'; skipping it.");
			}
		}
		return num;
	}

	public static int IFKCCDAIADF(string DPBKBKDCIOI, float JIJAJFEJJHK)
	{
		return IFKCCDAIADF(DPBKBKDCIOI, false, JIJAJFEJJHK);
	}

	public static void GKMINHHAMAK()
	{
		for (int i = OBCGCEHIFBH; i < AEAIOKEFHGG; i++)
		{
			AudioManager.Stop(i);
		}
		HLLDPAAADKK.Clear();
		BEKCBIJGMPE.Clear();
	}

	public static void IBHIPOOHNFK()
	{
		foreach (KeyValuePair<string, uint> item in HLLDPAAADKK)
		{
			StopSound((int)item.Value);
		}
		HLLDPAAADKK.Clear();
	}

	public static void PMOECBEJGBL()
	{
		for (int i = OBCGCEHIFBH; i < AEAIOKEFHGG; i++)
		{
			AudioManager.Pause(true, i);
		}
	}

	public static void BPPCHJFPEHB()
	{
		for (int i = OBCGCEHIFBH; i < AEAIOKEFHGG; i++)
		{
			AudioManager.Pause(false, i);
		}
	}

	private static void StopSound(int LMGPAGINHGD)
	{
		AudioManager.Stop(LMGPAGINHGD);
	}

	public static void StopSound(string path)
	{
		KeyValuePair<string, uint> keyValuePair = BKDELGBHEDP(path);
		if (keyValuePair.Key != string.Empty)
		{
			HOEPOMDNEML(path);
			StopSound((int)keyValuePair.Value);
			return;
		}
		foreach (KeyValuePair<string, uint> item in BEKCBIJGMPE)
		{
			if (keyValuePair.Key == path)
			{
				StopSound((int)keyValuePair.Value);
			}
		}
	}

	public static void PlayMusic(string LOJOJHIFCBL, bool KKHJAJFEPPA = true)
	{
		FAJONFGJBPD();
		if (string.IsNullOrEmpty(LOJOJHIFCBL))
		{
			if (MissingAudioWarnings.Add("music:<empty>"))
			{
				UnityEngine.Debug.LogWarning("[Audio] Fight requested an empty music name; using the recovered default track.");
			}
			LOJOJHIFCBL = "fight1_samurai_spirit";
		}
		string bLMBLOKPMEC = BLMBLOKPMEC;
		string text = LOJOJHIFCBL;
		bool flag = text.EndsWith(".ogg", System.StringComparison.OrdinalIgnoreCase);
		bLMBLOKPMEC = ((!flag && !SF2Paths.CGOHPKEBECD) ? (bLMBLOKPMEC + LOJOJHIFCBL + EJGKHALAMAG) : (bLMBLOKPMEC + LOJOJHIFCBL));
		AudioClip audioClip = ResourceManager.GetAudioClip(bLMBLOKPMEC);
		if (audioClip == null)
		{
			string fallback = ResolveRecoveredMusicName(LOJOJHIFCBL);
			audioClip = ResourceManager.GetAudioClip(BuildMusicPath(fallback));
			if (audioClip == null)
			{
				fallback = "fight1_samurai_spirit";
				audioClip = ResourceManager.GetAudioClip(BuildMusicPath(fallback));
			}
			if (audioClip == null)
			{
				if (MissingAudioWarnings.Add("music-missing:" + LOJOJHIFCBL))
				{
					UnityEngine.Debug.LogWarning("[Audio] Missing music '" + LOJOJHIFCBL + "'; continuing without music.");
				}
				return;
			}
			if (MissingAudioWarnings.Add("music-fallback:" + LOJOJHIFCBL))
			{
				UnityEngine.Debug.Log("[Audio] Resolved music '" + LOJOJHIFCBL + "' to recovered track '" + fallback + "'.");
			}
		}
		AudioManager.AddAudio(audioClip, LOJOJHIFCBL, 1f);
		MDGPOBCKJMJ(FINECDOIGAH, LOJOJHIFCBL, KKHJAJFEPPA, MLELLNBHONP);
		MDAIMFGPCEG(LOJOJHIFCBL);
	}

	private static string ResolveRecoveredMusicName(string requested)
	{
		string text = (requested ?? string.Empty).Trim();
		if (text.EndsWith(".ogg", System.StringComparison.OrdinalIgnoreCase) ||
			text.EndsWith(".mp3", System.StringComparison.OrdinalIgnoreCase))
		{
			text = text.Substring(0, text.Length - 4);
		}
		string recovered;
		if (RecoveredMusicAliases.TryGetValue(text, out recovered))
		{
			return recovered;
		}
		return "fight1_samurai_spirit";
	}

	private static string BuildMusicPath(string musicName)
	{
		return BLMBLOKPMEC + musicName + ((!SF2Paths.CGOHPKEBECD) ? EJGKHALAMAG : string.Empty);
	}

	public static void MDGPOBCKJMJ(int ADNDLGKIJJK, string DPBKBKDCIOI, bool KKHJAJFEPPA, bool KPCIIDFJCOB)
	{
		AudioManager.Play(ADNDLGKIJJK, DPBKBKDCIOI, KKHJAJFEPPA, true);
		SetMuteToChannel(ADNDLGKIJJK, KPCIIDFJCOB);
	}

	public static void FAJONFGJBPD()
	{
		if (AGCEHOJAJBK())
		{
			AudioManager.Stop(FINECDOIGAH);
			if (PJDJEAPBNLF() != null)
			{
				AudioManager.UnloadAudio(PJDJEAPBNLF());
				MDAIMFGPCEG(null);
			}
		}
	}

	public static void CKIHDLJBGAE()
	{
		if (AudioManager.IsPlaying(FINECDOIGAH))
		{
			AudioManager.Pause(true, FINECDOIGAH);
			BOLPPMPALJJ(true);
		}
	}

	public static void MPAHNMFMHHK()
	{
		if (AudioManager.IsPlaying(FINECDOIGAH))
		{
			AudioManager.Pause(false, FINECDOIGAH);
			BOLPPMPALJJ(false);
		}
	}

	public static void PreloadSounds(List<string> NAECCPFPEHC)
	{
		foreach (string item in NAECCPFPEHC)
		{
			IOIEJHLMBLI(item, NBHPABEBLOP());
		}
	}

	public static bool IOIEJHLMBLI(string DPBKBKDCIOI, float JIJAJFEJJHK = 1f)
	{
		string text = AJLAODNPHFB();
		text += DPBKBKDCIOI;
		if (!SF2Paths.CGOHPKEBECD)
		{
			text += GMGNDGCHFFG();
		}
		AudioClip audioClip = ResourceManager.GetAudioClip(text);
		AudioManager.AddAudio(audioClip, DPBKBKDCIOI, JIJAJFEJJHK);
		return audioClip != null;
	}

	private static void KCLJDFLJMDO(string path, uint OKNNNLIPODI)
	{
		KeyValuePair<string, uint> item = new KeyValuePair<string, uint>(path, OKNNNLIPODI);
		if (BEKCBIJGMPE.Count == MaxPlayableSounds)
		{
			BEKCBIJGMPE.Remove(BEKCBIJGMPE[0]);
		}
		BEKCBIJGMPE.Add(item);
	}

	private static void EOIPDBEELIJ(string path, uint OKNNNLIPODI)
	{
		KeyValuePair<string, uint> item = new KeyValuePair<string, uint>(path, OKNNNLIPODI);
		HLLDPAAADKK.Add(item);
	}

	private static void HOEPOMDNEML(string path)
	{
		foreach (KeyValuePair<string, uint> item in HLLDPAAADKK)
		{
			if (item.Key == path)
			{
				HLLDPAAADKK.Remove(item);
				break;
			}
		}
	}

	private static KeyValuePair<string, uint> BKDELGBHEDP(string path)
	{
		KeyValuePair<string, uint> result = new KeyValuePair<string, uint>(string.Empty, 0u);
		foreach (KeyValuePair<string, uint> item in HLLDPAAADKK)
		{
			if (item.Key == path)
			{
				result = item;
				return result;
			}
		}
		return result;
	}

	public static void Init()
	{
		int[] array = new int[AEAIOKEFHGG - OBCGCEHIFBH + 1];
		for (int i = OBCGCEHIFBH; i < AEAIOKEFHGG; i++)
		{
			array[i - OBCGCEHIFBH] = i;
		}
		AudioManager.Init(null, FINECDOIGAH, array);
	}

	public static int AONDDFIDNJE()
	{
		for (int i = OBCGCEHIFBH; i < AEAIOKEFHGG; i++)
		{
			if (!AudioManager.IsPlaying(i))
			{
				return i;
			}
		}
		return AEAIOKEFHGG;
	}
}
