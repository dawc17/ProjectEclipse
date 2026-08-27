using System.Collections.Generic;

public class DirectoryController
{
	private static DirectoryController _instance;

	private List<string> _searchDirectory = new List<string>();

	public static DirectoryController BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	private DirectoryController()
	{
		KBOEEMIOFOG();
	}

	public static DirectoryController ELEBLBJKDBI()
	{
		if (_instance == null)
		{
			_instance = new DirectoryController();
		}
		return _instance;
	}

	public static int KGHANHJHINK(string path, string MNMPGNFFOGA)
	{
		int num = path.IndexOf(MNMPGNFFOGA);
		return (num != -1) ? (num + MNMPGNFFOGA.Length) : 0;
	}

	public static string BJHBMEEAHIM()
	{
		return string.Empty;
	}

	public static string BECKNKJNFJB(string path)
	{
		if (IsPathWithDrive(path))
		{
			return path;
		}
		string empty = string.Empty;
		return empty + path;
	}

	public static bool IsPathWithDrive(string path)
	{
		string value = BJHBMEEAHIM();
		return path.Contains(value);
	}

	public static string BAANOCLBLKM(string path)
	{
		int num = KGHANHJHINK(path, "://");
		string result = path;
		if (num < path.Length)
		{
			result = path.Substring(num, path.Length);
		}
		return result;
	}

	private void KBOEEMIOFOG()
	{
		_searchDirectory.Clear();
		_searchDirectory.Add(string.Empty);
	}
}
