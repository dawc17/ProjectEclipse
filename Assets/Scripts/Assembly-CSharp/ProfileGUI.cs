using System.Xml;

public static class ProfileGUI
{
	public static int AnimationSpeed = 1;

	public static MinMaxValue OJEAKFALOGE = new MinMaxValue();

	public static MinMaxValue OMILCNNEBIL = new MinMaxValue();

	public static float SpeedScrollAchievements = 0f;

	public static void Parse(XmlNode node)
	{
		AnimationSpeed = node["AnimationSpeed"].PNJPEDPDMCP().ParseInt(1);
		OJEAKFALOGE.Parse(node["PerkOpacity"], 1f, 1f);
		OMILCNNEBIL.Parse(node["SelectOpacity"], 1f, 1f);
		SpeedScrollAchievements = node["SpeedScrollAchievements"].PNJPEDPDMCP().ParseFloat() / 60f;
	}
}
