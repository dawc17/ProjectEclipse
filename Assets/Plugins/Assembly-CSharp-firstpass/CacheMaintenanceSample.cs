using System;
using UnityEngine;

public sealed class CacheMaintenanceSample : MonoBehaviour
{
	private enum KNMPMKLLPMH
	{
		Days = 0,
		Hours = 1,
		Mins = 2,
		Secs = 3
	}

	private KNMPMKLLPMH PFPLMJNDEKI = KNMPMKLLPMH.Secs;

	private int value = 10;

	private int EBFOBGKCGJP = 5242880;

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Delete cached entities older then");
			GUILayout.Label(value.ToString(), GUILayout.MinWidth(50f));
			value = (int)GUILayout.HorizontalSlider(value, 1f, 60f, GUILayout.MinWidth(100f));
			GUILayout.Space(10f);
			PFPLMJNDEKI = (KNMPMKLLPMH)GUILayout.SelectionGrid((int)PFPLMJNDEKI, new string[4] { "Days", "Hours", "Mins", "Secs" }, 4);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Cache Size (bytes): ", GUILayout.Width(150f));
			GUILayout.Label(EBFOBGKCGJP.ToString("N0"), GUILayout.Width(70f));
			EBFOBGKCGJP = (int)GUILayout.HorizontalSlider(EBFOBGKCGJP, 1024f, 10485760f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			if (GUILayout.Button("Maintenance"))
			{
				TimeSpan hJELDPKENPC = TimeSpan.FromDays(14.0);
				switch (PFPLMJNDEKI)
				{
				case KNMPMKLLPMH.Days:
					hJELDPKENPC = TimeSpan.FromDays(value);
					break;
				case KNMPMKLLPMH.Hours:
					hJELDPKENPC = TimeSpan.FromHours(value);
					break;
				case KNMPMKLLPMH.Mins:
					hJELDPKENPC = TimeSpan.FromMinutes(value);
					break;
				case KNMPMKLLPMH.Secs:
					hJELDPKENPC = TimeSpan.FromSeconds(value);
					break;
				}
				HTTPCacheService.JJFFGOABNOA(new HTTPCacheMaintananceParams(hJELDPKENPC, (ulong)EBFOBGKCGJP));
			}
		});
	}
}
