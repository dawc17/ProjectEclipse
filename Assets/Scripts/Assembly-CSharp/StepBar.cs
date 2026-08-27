using System.Collections.Generic;

public class StepBar : ProgressBar
{
	private List<int> EOHNNHKBLON;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void Init()
	{
		base.Init();
		SetValueBorders(0f, 100f);
	}

	public void SetValue(int value, float _Duration = 0f)
	{
		base.SetValue(GetPercentValue(value), _Duration);
	}

	public override float GetValue()
	{
		float value = base.GetValue();
		return GetPercentIndex(value);
	}

	public void SetPercent(List<int> ONDOPPJBEEF)
	{
		EOHNNHKBLON = ONDOPPJBEEF;
		SetValue(0);
	}

	public int GetPercentIndex(float AMBMJABLPFE)
	{
		int num = 0;
		foreach (int item in EOHNNHKBLON)
		{
			if ((float)item == AMBMJABLPFE)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	private float GetPercentValue(int value)
	{
		int count = EOHNNHKBLON.Count;
		if (count == 0)
		{
			return 0f;
		}
		if (value < 0)
		{
			return EOHNNHKBLON[0];
		}
		if (count <= value)
		{
			return EOHNNHKBLON[count - 1];
		}
		return EOHNNHKBLON[value];
	}
}
