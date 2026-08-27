using System;

namespace GooglePlayGames.BasicApi.Quests
{
	[Flags]
	public enum QuestFetchFlags
	{
		Upcoming = 1,
		Open = 2,
		Accepted = 4,
		Completed = 8,
		CompletedNotClaimed = 0x10,
		Expired = 0x20,
		EndingSoon = 0x40,
		Failed = 0x80,
		All = -1
	}
}
