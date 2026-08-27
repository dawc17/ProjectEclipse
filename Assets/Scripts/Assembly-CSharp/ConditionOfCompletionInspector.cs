using System.Collections.Generic;

public class ConditionOfCompletionInspector
{
	private List<ConditionOfCompletion> _conditions = new List<ConditionOfCompletion>();

	public bool OPKPFKJPHNN(FightIDS DIAIIPCBMFL)
	{
		for (int i = 0; i < _conditions.Count; i++)
		{
			if (!_conditions[i].IsComplete(DIAIIPCBMFL))
			{
				return false;
			}
		}
		return true;
	}

	public void CHDLHMGPDHL(ConditionOfCompletion IOFGGOCEIAM)
	{
		_conditions.Add(IOFGGOCEIAM);
	}

	public void EGGLLLLFMCO(List<ConditionOfCompletion> conditions)
	{
		_conditions.AddRange(conditions);
	}
}
