using Nekki.SF2.GUI.Fight;
using UnityEngine;

public class FightHolder : MonoBehaviour
{
	[SerializeField]
	private PreFight preFight;

	private Fight fight;

	public static FightList fightList;

	private static FightHolder _Current;

	public Fight HGFJGOBMCFF
	{
		get
		{
			return get_Fight();
		}
	}

	public static FightHolder BLOOLFFMKFI
	{
		get
		{
			return get_Current();
		}
	}

	public Fight get_Fight()
	{
		return fight;
	}

	public static FightHolder get_Current()
	{
		return _Current;
	}

	private void Awake()
	{
		_Current = this;
	}

	private void Start()
	{
		if (fightList == null)
		{
			LLLOJBFMONN.Error("FightHolder - Start() - FightList is empty!");
			Module.DLOKJOHNDID(ScreenType.ModuleDojo);
		}
		if (preFight == null)
		{
			LLLOJBFMONN.Error("FightHolder.Start preFight is null");
		}
		fight = GameUtils.ABAIHGFPHMO(fightList, preFight);
	}

	private void FixedUpdate()
	{
		if (fight != null)
		{
			fight.Draw();
		}
	}
}
