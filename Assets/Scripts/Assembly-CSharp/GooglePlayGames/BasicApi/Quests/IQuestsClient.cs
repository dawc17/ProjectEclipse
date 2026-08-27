using System;
using System.Collections.Generic;

namespace GooglePlayGames.BasicApi.Quests
{
	[Obsolete("Quests are being removed in 2018.")]
	public interface IQuestsClient
	{
		void Fetch(DataSource source, string questId, Action<ResponseStatus, IQuest> callback);

		void FetchMatchingState(DataSource source, QuestFetchFlags flags, Action<ResponseStatus, List<IQuest>> callback);

		void ShowAllQuestsUI(Action<QuestUiResult, IQuest, IQuestMilestone> callback);

		void ShowSpecificQuestUI(IQuest quest, Action<QuestUiResult, IQuest, IQuestMilestone> callback);

		void Accept(IQuest quest, Action<QuestAcceptStatus, IQuest> callback);

		void ClaimMilestone(IQuestMilestone milestone, Action<QuestClaimMilestoneStatus, IQuest, IQuestMilestone> callback);
	}
}
