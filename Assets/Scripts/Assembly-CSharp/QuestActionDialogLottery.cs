using System;
using System.Xml;
using Eclipse.Modding;

public class QuestActionDialogLottery : QuestAction
{
	private string fightName;
	private string spinNumber;
	private ModQuestLotteryAction presentation;

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		fightName = node.Attributes["FightName"].CIPOICEEIBK(string.Empty);
		spinNumber = node.Attributes["SpinNumber"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		// SpinNumber is an archived paid-spin continuation, not a free draw count.
		if (!string.IsNullOrEmpty(spinNumber)) throw new NotSupportedException("Paid lottery spin continuation is not implemented.");
		if (LKMGEKCOFMF() != KHLLOOHAMLC.LOCK_NONE) throw new NotSupportedException("Lottery dialogs own their input; omit the native Lock attribute.");
		presentation?.Dispose();
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		var result = new ConditionExtension.CompareResult();
		var condition = new QuestCondition();
		condition.LIMHBJBEEIA(GFIHPBCEEOB);
		condition.MCPIOGALBMK(fightName, result);
		presentation = new ModQuestLotteryAction(this, result.ToString(), OGIJONMKABB);
		presentation.Show();
	}

	public override void GKFMJKAAJCA()
	{
		presentation?.Dispose();
		presentation = null;
	}
}
