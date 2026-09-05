using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Nekki.SF2.Core.Quests;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;
using UnityEngine;

public class QuestAction : global::EventDispatcher<object>
{
	public enum KHLLOOHAMLC
	{
		LOCK_NONE = 0,
		LOCK_SILENT = 1,
		LOCK_VISIBLE = 2
	}

	public enum GMNIJDEMPNF
	{
		OnRun = 0,
		OnComplete = 1,
		OnCreateRosterQuest = 2
	}

	public enum PODELEEIBMP
	{
		QUEST_ACTION_NONE = 0,
		QUEST_ACTION_DIALOG = 1,
		QUEST_ACTION_DIALOG_CHECK_TICKETS = 2,
		QUEST_ACTION_DIALOG_LOTTERY = 3,
		QUEST_ACTION_FIGHT = 4,
		QUEST_ACTION_ACT = 5,
		QUEST_ACTION_UNLOCK_BATTLE = 6,
		QUEST_ACTION_CHECKPOINT = 7,
		QUEST_ACTION_VARIABLE = 8,
		QUEST_ACTION_GOTO_ZONE = 9,
		QUEST_ACTION_WAIT = 10,
		QUEST_ACTION_DOWNLOAD = 11,
		QUEST_ACTION_UPGRADES = 12,
		QUEST_ACTION_FORGE = 13,
		QUEST_ACTION_SHOP = 14,
		QUEST_ACTION_NEWS = 15,
		QUEST_ACTION_TOGGLE_ITEMS = 16,
		QUEST_ACTION_ACTIVATE = 17,
		QUEST_ACTION_DISCOUNT = 18,
		QUEST_ACTION_CHANGE_SCENE = 19,
		QUEST_ACTION_CHANGE_TAB = 20,
		QUEST_ACTION_GIVE_ITEM = 21,
		QUEST_ACTION_FORCE_EXECUTION = 22,
		QUEST_ACTION_GIVE_CURRENCY = 23,
		QUEST_ACTION_TAKE_CURRENCY = 24,
		QUEST_ACTION_MAP_FOCUS = 25,
		QUEST_ACTION_VERSION = 26,
		QUEST_ACTION_CLEAR_STACK = 27,
		QUEST_ACTION_FB = 28,
		QUEST_ACTION_FB_INDICATOR = 29,
		QUEST_ACTION_DELIVER = 30,
		QUEST_ACTION_ATTACH_FILE = 31,
		QUEST_ACTION_SET_PARAMETER = 32,
		QUEST_ACTION_FOREACH = 33,
		QUEST_ACTION_RECOUNT = 34,
		QUEST_ACTION_SHOW_AD = 35,
		QUEST_ACTION_SET_ENERGY = 36,
		QUEST_ACTION_OPEN_URL = 37,
		QUEST_ACTION_RESET_PERKS = 38,
		QUEST_ACTION_UPGRADES_CLEANUP = 39,
		QUEST_ACTION_SESSION_SETTINGS = 40,
		QUEST_ACTION_RESET_DUEL_TIMER = 41,
		QUEST_ACTION_SEND_STRANGER_STATS = 42,
		QUEST_ACTION_SEND_DISCOUNT_STATS = 43,
		QUEST_ACTION_SET_CURRENT_ZONE = 44,
		QUEST_ACTION_SET_LANGUAGE = 45,
		QUEST_ACTION_BUY_ITEM = 46,
		QUEST_ACTION_SHOW_STARTER_PACK_TIMER = 47,
		QUEST_ACTION_HIDE_STARTER_PACK_TIMER = 48,
		QUEST_ACTION_SHOW_VIDEO = 49,
		QUEST_ACTION_UPDATE_SCREEN = 50,
		QUEST_ACTION_DENOMINATION = 51,
		QUEST_ACTION_TAPJOY_CALL = 52,
		QUEST_ACTION_ACTIVATE_TIMER = 53,
		QUEST_ACTION_END_TIMER = 54,
		QUEST_ACTION_SHOW_MAP_BUTTON = 55,
		QUEST_ACTION_HIDE_MAP_BUTTON = 56,
		QUEST_ACTION_ECLIPSE_MODE = 57,
		QUEST_ACTION_ECLIPSE_MODE_TUTORIAL = 58,
		QUEST_ACTION_ECLIPSE_MODE_SWITCH_BACK_TUTORIAL = 59,
		QUEST_ACTION_ECLIPSE_MODE_REPLAY_TUTORIAL = 60,
		QUEST_ACTION_TOGGLE_GROUP = 61,
		QUEST_ACTION_REMOVE_PACK = 62,
		QUEST_ACTION_RESTART_APPLICATION = 63,
		QUEST_ACTION_SET_MAP_MASK = 64,
		QUEST_ACTION_RESUME_QUEST = 65,
		QUEST_ACTION_TOGGLE_BATTLE = 66,
		QUEST_ACTION_SHOW_FORGE_TUTORIAL = 67,
		QUEST_ACTION_SET_LOW_GRAPHICS = 68,
		QUEST_ACTION_RESET_CRASH_FLAG = 69,
		QUEST_ACTION_RESET_ENCHANTMENTS = 70,
		QUEST_ACTION_GIVE_PERK = 71,
		QUEST_ACTION_GIVE_FREE_RECIPE = 72,
		QUEST_ACTION_OPEN_FORGE = 73,
		QUEST_ACTION_CHANGE_PLAYER_AVATAR = 74,
		QUEST_ACTION_SHOW_SET_TUTORIAL = 75,
		QUEST_ACTION_SHOW_CREDITS = 76,
		QUEST_ACTION_GIVE_ACHIEVEMENT = 77,
		QUEST_ACTION_SHOW_RAID_TOGGLE_BTN = 78,
		QUEST_ACTION_SHOW_RAID_TUTORIAL = 79,
		QUEST_ACTION_OPEN_LEAGUE_DIALOG = 80,
		QUEST_ACTION_SHOW_RAID_FIGHT_TUTORIAL = 81,
		QUEST_ACTION_SHOW_RAID_LEAGUES_TUTORIAL = 82,
		QUEST_ACTION_SET_STORY_TUTORIAL_STEP = 83,
		QUEST_ACTION_SET_RAID_INFO_TUTORIAL_STEP = 84,
		QUEST_ACTION_SWITCH_TO_RAIDS = 85,
		QUEST_ACTION_UNZIP = 86,
		QUEST_ACTION_BUY_PACK = 87,
		QUEST_ACTION_MENU_BTN_FLASHING = 88,
		QUEST_ACTION_STORY_TUTORIAL_MOVE = 89,
		QUEST_ACTION_STORY_TUTORIAL_PUNCHBAG = 90,
		QUEST_ACTION_STORY_TUTORIAL_BUY_ITEM = 91,
		QUEST_ACTION_STORY_TUTORIAL_CLICK_FIGHT = 92,
		QUEST_ACTION_STORY_TUTORIAL_LEARN_PERK = 93,
		QUEST_ACTION_STORY_TUTORIAL_DOUBLE_SWEEP = 94,
		QUEST_ACTION_STORY_TUTORIAL_SHOW_BLOCK = 95,
		QUEST_ACTION_IF = 96,
		QUEST_ACTION_RUN = 97,
		QUEST_ACTION_CHANGE_DOJO_LOCATION = 98,
		QUEST_ACTION_UPDATE_ECLIPSE_BATTLES = 99
	}

	public int Index;

	public bool CheckPoint;

	public string EFJMDEMAGIM;

	public string ONGHPGEIJEN;

	public string AEHNKDOJALB;

	public int AMIMGEOENPL;

	public QuestParameters PAJDEKLLFNJ;

	private QuestStage DOKAIKMLLDK;

	private string DPBKBKDCIOI;

	private KHLLOOHAMLC JKIPOGOLAAI;

	public static PODELEEIBMP HJCIPCKMANH(string LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case "Dialog":
			return PODELEEIBMP.QUEST_ACTION_DIALOG;
		case "DialogCheckTickets":
			return PODELEEIBMP.QUEST_ACTION_DIALOG_CHECK_TICKETS;
		case "DialogLottery":
			return PODELEEIBMP.QUEST_ACTION_DIALOG_LOTTERY;
		case "Fight":
			return PODELEEIBMP.QUEST_ACTION_FIGHT;
		case "ActScreen":
			return PODELEEIBMP.QUEST_ACTION_ACT;
		case "ShowBattle":
		case "HideBattle":
			return PODELEEIBMP.QUEST_ACTION_UNLOCK_BATTLE;
		case "Checkpoint":
			return PODELEEIBMP.QUEST_ACTION_CHECKPOINT;
		case "SetVariable":
			return PODELEEIBMP.QUEST_ACTION_VARIABLE;
		case "OpenZone":
			return PODELEEIBMP.QUEST_ACTION_GOTO_ZONE;
		case "Wait":
			return PODELEEIBMP.QUEST_ACTION_WAIT;
		case "Download":
			return PODELEEIBMP.QUEST_ACTION_DOWNLOAD;
		case "ShowUpgrades":
			return PODELEEIBMP.QUEST_ACTION_UPGRADES;
		case "ShowForge":
			return PODELEEIBMP.QUEST_ACTION_FORGE;
		case "OpenShop":
			return PODELEEIBMP.QUEST_ACTION_SHOP;
		case "ShowNews":
			return PODELEEIBMP.QUEST_ACTION_NEWS;
		case "ToggleItems":
			return PODELEEIBMP.QUEST_ACTION_TOGGLE_ITEMS;
		case "Activate":
			return PODELEEIBMP.QUEST_ACTION_ACTIVATE;
		case "Discount":
			return PODELEEIBMP.QUEST_ACTION_DISCOUNT;
		case "ChangeScene":
			return PODELEEIBMP.QUEST_ACTION_CHANGE_SCENE;
		case "GiveItem":
			return PODELEEIBMP.QUEST_ACTION_GIVE_ITEM;
		case "ForceExecution":
			return PODELEEIBMP.QUEST_ACTION_FORCE_EXECUTION;
		case "GiveCurrency":
			return PODELEEIBMP.QUEST_ACTION_GIVE_CURRENCY;
		case "TakeCurrency":
			return PODELEEIBMP.QUEST_ACTION_TAKE_CURRENCY;
		case "SetMapFocus":
			return PODELEEIBMP.QUEST_ACTION_MAP_FOCUS;
		case "SetDataVersion":
			return PODELEEIBMP.QUEST_ACTION_VERSION;
		case "ClearQuestQueue":
			return PODELEEIBMP.QUEST_ACTION_CLEAR_STACK;
		case "FacebookAPICall":
			return PODELEEIBMP.QUEST_ACTION_FB;
		case "SetFBIndicator":
			return PODELEEIBMP.QUEST_ACTION_FB_INDICATOR;
		case "Deliver":
			return PODELEEIBMP.QUEST_ACTION_DELIVER;
		case "AttachQuestFile":
			return PODELEEIBMP.QUEST_ACTION_ATTACH_FILE;
		case "SetParameter":
			return PODELEEIBMP.QUEST_ACTION_SET_PARAMETER;
		case "Foreach":
			return PODELEEIBMP.QUEST_ACTION_FOREACH;
		case "Recount":
			return PODELEEIBMP.QUEST_ACTION_RECOUNT;
		case "ShowAd":
			return PODELEEIBMP.QUEST_ACTION_SHOW_AD;
		case "SetEnergy":
			return PODELEEIBMP.QUEST_ACTION_SET_ENERGY;
		case "OpenUrl":
			return PODELEEIBMP.QUEST_ACTION_OPEN_URL;
		case "ResetPerks":
			return PODELEEIBMP.QUEST_ACTION_RESET_PERKS;
		case "UpgradesCleanup":
			return PODELEEIBMP.QUEST_ACTION_UPGRADES_CLEANUP;
		case "SetSessionSettings":
			return PODELEEIBMP.QUEST_ACTION_SESSION_SETTINGS;
		case "ResetDuelTimer":
			return PODELEEIBMP.QUEST_ACTION_RESET_DUEL_TIMER;
		case "SendStrangerStats":
			return PODELEEIBMP.QUEST_ACTION_SEND_STRANGER_STATS;
		case "SendDiscountStats":
			return PODELEEIBMP.QUEST_ACTION_SEND_DISCOUNT_STATS;
		case "SetCurrentZone":
			return PODELEEIBMP.QUEST_ACTION_SET_CURRENT_ZONE;
		case "SetLanguage":
			return PODELEEIBMP.QUEST_ACTION_SET_LANGUAGE;
		case "BuyItem":
			return PODELEEIBMP.QUEST_ACTION_BUY_ITEM;
		case "ShowStarterPackTimer":
			return PODELEEIBMP.QUEST_ACTION_SHOW_STARTER_PACK_TIMER;
		case "HideStarterPackTimer":
			return PODELEEIBMP.QUEST_ACTION_HIDE_STARTER_PACK_TIMER;
		case "ChangeTab":
			return PODELEEIBMP.QUEST_ACTION_CHANGE_TAB;
		case "ShowVideo":
			return PODELEEIBMP.QUEST_ACTION_SHOW_VIDEO;
		case "UpdateScene":
			return PODELEEIBMP.QUEST_ACTION_UPDATE_SCREEN;
		case "Denomination":
			return PODELEEIBMP.QUEST_ACTION_DENOMINATION;
		case "TapjoyActionCall":
			return PODELEEIBMP.QUEST_ACTION_TAPJOY_CALL;
		case "ActivateTimer":
			return PODELEEIBMP.QUEST_ACTION_ACTIVATE_TIMER;
		case "EndTimer":
			return PODELEEIBMP.QUEST_ACTION_END_TIMER;
		case "ShowMapButton":
			return PODELEEIBMP.QUEST_ACTION_SHOW_MAP_BUTTON;
		case "HideMapButton":
			return PODELEEIBMP.QUEST_ACTION_HIDE_MAP_BUTTON;
		case "ToggleEclipseMode":
			return PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE;
		case "ShowEclipseModeTutorial":
			return PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE_TUTORIAL;
		case "ShowEclipseModeSwitchBackTutorial":
			return PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE_SWITCH_BACK_TUTORIAL;
		case "ShowEclipseModeReplayTutorial":
			return PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE_REPLAY_TUTORIAL;
		case "ToggleGroup":
			return PODELEEIBMP.QUEST_ACTION_TOGGLE_GROUP;
		case "RemovePack":
			return PODELEEIBMP.QUEST_ACTION_REMOVE_PACK;
		case "ApplicationRestart":
			return PODELEEIBMP.QUEST_ACTION_RESTART_APPLICATION;
		case "SetMapMask":
			return PODELEEIBMP.QUEST_ACTION_SET_MAP_MASK;
		case "ResumeQuests":
			return PODELEEIBMP.QUEST_ACTION_RESUME_QUEST;
		case "ToggleBattle":
			return PODELEEIBMP.QUEST_ACTION_TOGGLE_BATTLE;
		case "ShowForgeTutorial":
			return PODELEEIBMP.QUEST_ACTION_SHOW_FORGE_TUTORIAL;
		case "ResetCrashFlag":
			return PODELEEIBMP.QUEST_ACTION_RESET_CRASH_FLAG;
		case "SetLowGraphics":
			return PODELEEIBMP.QUEST_ACTION_SET_LOW_GRAPHICS;
		case "ResetEnchantments":
			return PODELEEIBMP.QUEST_ACTION_RESET_ENCHANTMENTS;
		case "GivePerk":
			return PODELEEIBMP.QUEST_ACTION_GIVE_PERK;
		case "GiveFreeRecipe":
			return PODELEEIBMP.QUEST_ACTION_GIVE_FREE_RECIPE;
		case "OpenForge":
			return PODELEEIBMP.QUEST_ACTION_OPEN_FORGE;
		case "ChangePlayerAvatar":
			return PODELEEIBMP.QUEST_ACTION_CHANGE_PLAYER_AVATAR;
		case "ShowSetTutorial":
			return PODELEEIBMP.QUEST_ACTION_SHOW_SET_TUTORIAL;
		case "ShowCredits":
			return PODELEEIBMP.QUEST_ACTION_SHOW_CREDITS;
		case "GiveAchievement":
			return PODELEEIBMP.QUEST_ACTION_GIVE_ACHIEVEMENT;
		case "ShowRaidsGag":
			return PODELEEIBMP.QUEST_ACTION_SHOW_RAID_TOGGLE_BTN;
		case "RaidsButtonTutorial":
			return PODELEEIBMP.QUEST_ACTION_SHOW_RAID_TUTORIAL;
		case "OpenLeagueDialog":
			return PODELEEIBMP.QUEST_ACTION_OPEN_LEAGUE_DIALOG;
		case "ShowRaidFightTutorial":
			return PODELEEIBMP.QUEST_ACTION_SHOW_RAID_FIGHT_TUTORIAL;
		case "ShowLeagueWindow":
			return PODELEEIBMP.QUEST_ACTION_SHOW_RAID_LEAGUES_TUTORIAL;
		case "SetStoryTutorialStep":
			return PODELEEIBMP.QUEST_ACTION_SET_STORY_TUTORIAL_STEP;
		case "SetRaidInfoTutorialStep":
			return PODELEEIBMP.QUEST_ACTION_SET_RAID_INFO_TUTORIAL_STEP;
		case "SwitchToRaids":
			return PODELEEIBMP.QUEST_ACTION_SWITCH_TO_RAIDS;
		case "Unzip":
			return PODELEEIBMP.QUEST_ACTION_UNZIP;
		case "BuyPack":
			return PODELEEIBMP.QUEST_ACTION_BUY_PACK;
		case "Timer":
			return PODELEEIBMP.QUEST_ACTION_ACTIVATE_TIMER;
		case "MenuBtnFlashing":
			return PODELEEIBMP.QUEST_ACTION_MENU_BTN_FLASHING;
		case "StoryTutorialMove":
			return PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_MOVE;
		case "StoryTutorialPunchbag":
			return PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_PUNCHBAG;
		case "StoryTutorialBuyItem":
			return PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_BUY_ITEM;
		case "StoryTutorialClickFight":
			return PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_CLICK_FIGHT;
		case "StoryTutorialLearnPerk":
			return PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_LEARN_PERK;
		case "StoryTutorialDoubleSweep":
			return PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_DOUBLE_SWEEP;
		case "StoryTutorialShowBlock":
			return PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_SHOW_BLOCK;
		case "If":
			return PODELEEIBMP.QUEST_ACTION_IF;
		case "Run":
			return PODELEEIBMP.QUEST_ACTION_RUN;
		case "ChangeDojoLocation":
			return PODELEEIBMP.QUEST_ACTION_CHANGE_DOJO_LOCATION;
		case "UpdateEclipseBattles":
			return PODELEEIBMP.QUEST_ACTION_UPDATE_ECLIPSE_BATTLES;
		default:
			LLLOJBFMONN.Error(string.Format("{0} {1}", "Unknown quest type: ", LFLGCDNKNJI));
			return PODELEEIBMP.QUEST_ACTION_NONE;
		}
	}

	public static QuestAction GetClassActionByName(string CNKBLODAFDO)
	{
		QuestAction compatibilityAction = Eclipse.Content.QuestCompatibility.CreateRuntimeAction(CNKBLODAFDO);
		if (compatibilityAction != null)
		{
			return compatibilityAction;
		}
		PODELEEIBMP lFLGCDNKNJI = HJCIPCKMANH(CNKBLODAFDO);
		return CAHMAJAIHFI(lFLGCDNKNJI);
	}

	public static QuestAction CAHMAJAIHFI(PODELEEIBMP LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case PODELEEIBMP.QUEST_ACTION_DIALOG:
			return new QuestActionDialog();
		case PODELEEIBMP.QUEST_ACTION_DIALOG_CHECK_TICKETS:
			return new QuestActionDialogCheckTickets();
		case PODELEEIBMP.QUEST_ACTION_DIALOG_LOTTERY:
			return new QuestActionDialogLottery();
		case PODELEEIBMP.QUEST_ACTION_FIGHT:
			return new QuestActionFight();
		case PODELEEIBMP.QUEST_ACTION_ACT:
			return new QuestActionAct();
		case PODELEEIBMP.QUEST_ACTION_UNLOCK_BATTLE:
			return new QuestActionUnlockBattle();
		case PODELEEIBMP.QUEST_ACTION_CHECKPOINT:
			return new QuestActionCheckPoint();
		case PODELEEIBMP.QUEST_ACTION_VARIABLE:
			return new QuestActionVariable();
		case PODELEEIBMP.QUEST_ACTION_GOTO_ZONE:
			return new QuestActionGotoZone();
		case PODELEEIBMP.QUEST_ACTION_WAIT:
			return new QuestActionWait();
		case PODELEEIBMP.QUEST_ACTION_DOWNLOAD:
			return new QuestActionDownload();
		case PODELEEIBMP.QUEST_ACTION_UPGRADES:
			return new QuestActionUpgrades();
		case PODELEEIBMP.QUEST_ACTION_FORGE:
			return new QuestActionForge();
		case PODELEEIBMP.QUEST_ACTION_SHOP:
			return new QuestActionShop();
		case PODELEEIBMP.QUEST_ACTION_NEWS:
			return new QuestActionNews();
		case PODELEEIBMP.QUEST_ACTION_TOGGLE_ITEMS:
			return new QuestActionToggleItems();
		case PODELEEIBMP.QUEST_ACTION_ACTIVATE:
			return new QuestActionActivate();
		case PODELEEIBMP.QUEST_ACTION_DISCOUNT:
			return new QuestActionDiscount();
		case PODELEEIBMP.QUEST_ACTION_CHANGE_SCENE:
			return new QuestActionChangeScene();
		case PODELEEIBMP.QUEST_ACTION_CHANGE_TAB:
			return new QuestActionChangeTab();
		case PODELEEIBMP.QUEST_ACTION_GIVE_ITEM:
			return new QuestActionGiveItem();
		case PODELEEIBMP.QUEST_ACTION_FORCE_EXECUTION:
			return new QuestActionForceExecution();
		case PODELEEIBMP.QUEST_ACTION_GIVE_CURRENCY:
			return new QuestActionGiveCurrency();
		case PODELEEIBMP.QUEST_ACTION_TAKE_CURRENCY:
			return new QuestActionTakeCurrency();
		case PODELEEIBMP.QUEST_ACTION_MAP_FOCUS:
			return new QuestActionMapFocus();
		case PODELEEIBMP.QUEST_ACTION_VERSION:
			return new QuestActionCurrentVersion();
		case PODELEEIBMP.QUEST_ACTION_CLEAR_STACK:
			return new QuestActionClearStack();
		case PODELEEIBMP.QUEST_ACTION_FB:
			return new QuestActionFacebookAPICall();
		case PODELEEIBMP.QUEST_ACTION_FB_INDICATOR:
			return new QuestActionSetFBIndicator();
		case PODELEEIBMP.QUEST_ACTION_DELIVER:
			return new QuestActionDeliver();
		case PODELEEIBMP.QUEST_ACTION_ATTACH_FILE:
			return new QuestActionAttachFile();
		case PODELEEIBMP.QUEST_ACTION_SET_PARAMETER:
			return new QuestActionSetParameter();
		case PODELEEIBMP.QUEST_ACTION_FOREACH:
			return new QuestActionForeach();
		case PODELEEIBMP.QUEST_ACTION_RECOUNT:
			return new QuestActionRecount();
		case PODELEEIBMP.QUEST_ACTION_SHOW_AD:
			return new QuestActionShowAd();
		case PODELEEIBMP.QUEST_ACTION_SET_ENERGY:
			return new QuestActionSetEnergy();
		case PODELEEIBMP.QUEST_ACTION_OPEN_URL:
			return new QuestActionOpenUrl();
		case PODELEEIBMP.QUEST_ACTION_RESET_PERKS:
			return new QuestActionResetPerks();
		case PODELEEIBMP.QUEST_ACTION_UPGRADES_CLEANUP:
			return new QuestActionUpgradesCleanup();
		case PODELEEIBMP.QUEST_ACTION_SESSION_SETTINGS:
			return new QuestActionSessionSettings();
		case PODELEEIBMP.QUEST_ACTION_RESET_DUEL_TIMER:
			return new QuestActionResetDuelTimer();
		case PODELEEIBMP.QUEST_ACTION_SEND_STRANGER_STATS:
			return new QuestActionSendStrangerStats();
		case PODELEEIBMP.QUEST_ACTION_SEND_DISCOUNT_STATS:
			return new QuestActionSendDiscountStats();
		case PODELEEIBMP.QUEST_ACTION_SET_CURRENT_ZONE:
			return new QuestActionSetCurrentZone();
		case PODELEEIBMP.QUEST_ACTION_SET_LANGUAGE:
			return new QuestActionSetLanguage();
		case PODELEEIBMP.QUEST_ACTION_BUY_ITEM:
			return new QuestActionBuyItem();
		case PODELEEIBMP.QUEST_ACTION_SHOW_STARTER_PACK_TIMER:
			return new QuestActionShowStarterPackTimer();
		case PODELEEIBMP.QUEST_ACTION_HIDE_STARTER_PACK_TIMER:
			return new QuestActionHideStarterPackTimer();
		case PODELEEIBMP.QUEST_ACTION_SHOW_VIDEO:
			return new QuestActionShowVideo();
		case PODELEEIBMP.QUEST_ACTION_UPDATE_SCREEN:
			return new QuestActionUpdateScreen();
		case PODELEEIBMP.QUEST_ACTION_DENOMINATION:
			return new QuestActionDenomination();
		case PODELEEIBMP.QUEST_ACTION_TAPJOY_CALL:
			return new QuestActionTapjoyActionCall();
		case PODELEEIBMP.QUEST_ACTION_ACTIVATE_TIMER:
			return new QuestActionActivateTimer();
		case PODELEEIBMP.QUEST_ACTION_END_TIMER:
			return new QuestActionEndTimer();
		case PODELEEIBMP.QUEST_ACTION_SHOW_MAP_BUTTON:
			return new QuestActionShowMapButton();
		case PODELEEIBMP.QUEST_ACTION_HIDE_MAP_BUTTON:
			return new QuestActionHideMapButton();
		case PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE:
			return new QuestActionEclipseMode();
		case PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE_TUTORIAL:
			return new QuestActionEclipseModeTutorial();
		case PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE_SWITCH_BACK_TUTORIAL:
			return new QuestActionEclipseModeSwitchBackTutorial();
		case PODELEEIBMP.QUEST_ACTION_ECLIPSE_MODE_REPLAY_TUTORIAL:
			return new QuestActionEclipseModeReplayTutorial();
		case PODELEEIBMP.QUEST_ACTION_TOGGLE_GROUP:
			return new QuestActionToggleGroup();
		case PODELEEIBMP.QUEST_ACTION_REMOVE_PACK:
			return new QuestActionRemovePack();
		case PODELEEIBMP.QUEST_ACTION_RESTART_APPLICATION:
			return new QuestActionRestartApplication();
		case PODELEEIBMP.QUEST_ACTION_SET_MAP_MASK:
			return new QuestActionMapMask();
		case PODELEEIBMP.QUEST_ACTION_RESUME_QUEST:
			return new QuestActionResumeQuests();
		case PODELEEIBMP.QUEST_ACTION_TOGGLE_BATTLE:
			return new QuestActionToggleBattle();
		case PODELEEIBMP.QUEST_ACTION_SHOW_FORGE_TUTORIAL:
			return new QuestActionShowForgeTutorial();
		case PODELEEIBMP.QUEST_ACTION_RESET_CRASH_FLAG:
			return new QuestActionResetCrashFlag();
		case PODELEEIBMP.QUEST_ACTION_SET_LOW_GRAPHICS:
			return new QuestActionSetLowGraphics();
		case PODELEEIBMP.QUEST_ACTION_RESET_ENCHANTMENTS:
			return new QuestActionResetEnchantments();
		case PODELEEIBMP.QUEST_ACTION_GIVE_PERK:
			return new QuestActionGivePerk();
		case PODELEEIBMP.QUEST_ACTION_GIVE_FREE_RECIPE:
			return new QuestActionGiveFreeRecipe();
		case PODELEEIBMP.QUEST_ACTION_OPEN_FORGE:
			return new QuestActionOpenForge();
		case PODELEEIBMP.QUEST_ACTION_CHANGE_PLAYER_AVATAR:
			return new QuestActionChangePlayerAvatar();
		case PODELEEIBMP.QUEST_ACTION_SHOW_SET_TUTORIAL:
			return new QuestActionShowSetTutorial();
		case PODELEEIBMP.QUEST_ACTION_SHOW_CREDITS:
			return new QuestActionShowCredits();
		case PODELEEIBMP.QUEST_ACTION_GIVE_ACHIEVEMENT:
			return new QuestActionGiveAchievement();
		case PODELEEIBMP.QUEST_ACTION_SHOW_RAID_TOGGLE_BTN:
			return new QuestActionShowRaidToggleBtn();
		case PODELEEIBMP.QUEST_ACTION_SHOW_RAID_TUTORIAL:
			return new QuestActionShowRaidTutorial();
		case PODELEEIBMP.QUEST_ACTION_OPEN_LEAGUE_DIALOG:
			return new QuestActionOpenLeagueDialog();
		case PODELEEIBMP.QUEST_ACTION_SHOW_RAID_FIGHT_TUTORIAL:
			return new QuestActionShowRaidFightTutorial();
		case PODELEEIBMP.QUEST_ACTION_SHOW_RAID_LEAGUES_TUTORIAL:
			return new QuestActionShowRaidLeaguesTutorial();
		case PODELEEIBMP.QUEST_ACTION_SET_STORY_TUTORIAL_STEP:
			return new QuestActionSetStoryTutorialStep();
		case PODELEEIBMP.QUEST_ACTION_SET_RAID_INFO_TUTORIAL_STEP:
			return new QuestActionSetRaidInfoTutorialStep();
		case PODELEEIBMP.QUEST_ACTION_SWITCH_TO_RAIDS:
			return new QuestActionSwitchToRaidsMap();
		case PODELEEIBMP.QUEST_ACTION_UNZIP:
			return new QuestActionUnzip();
		case PODELEEIBMP.QUEST_ACTION_BUY_PACK:
			return new QuestActionBuyPack();
		case PODELEEIBMP.QUEST_ACTION_MENU_BTN_FLASHING:
			return new QuestActionMenuBtnFlashing();
		case PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_MOVE:
			return new QuestActionStoryTutorialMove();
		case PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_PUNCHBAG:
			return new QuestActionStoryTutorialPunchbag();
		case PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_BUY_ITEM:
			return new QuestActionStoryTutorialBuyItem();
		case PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_CLICK_FIGHT:
			return new QuestActionStoryTutorialClickFight();
		case PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_LEARN_PERK:
			return new QuestActionStoryTutorialLearnPerk();
		case PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_DOUBLE_SWEEP:
			return new QuestActionStoryTutorialDoubleSweep();
		case PODELEEIBMP.QUEST_ACTION_STORY_TUTORIAL_SHOW_BLOCK:
			return new QuestActionStoryTutorialShowBlock();
		case PODELEEIBMP.QUEST_ACTION_IF:
			return new QuestActionIf();
		case PODELEEIBMP.QUEST_ACTION_RUN:
			return new QuestActionRun();
		case PODELEEIBMP.QUEST_ACTION_CHANGE_DOJO_LOCATION:
			return new QuestActionChangeDojoLocation();
		case PODELEEIBMP.QUEST_ACTION_UPDATE_ECLIPSE_BATTLES:
			return new QuestActionUpdateEclipseBattles();
		default:
			LLLOJBFMONN.Error(string.Format("{0} {1}", "QuestAction.getClassActionByType - type: ", LFLGCDNKNJI));
			return new QuestAction();
		}
	}

	public void OGIJONMKABB()
	{
		if (JKIPOGOLAAI != KHLLOOHAMLC.LOCK_NONE)
		{
			Module.ELEBLBJKDBI().DIDFMBMPEAF(false);
		}
		if (LogRules.ELEBLBJKDBI().PIAKPGMPGMN())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("QuestAction ");
			stringBuilder.Append(EFJMDEMAGIM);
			stringBuilder.Append(" completed");
			LLLOJBFMONN.INNGABABJPC(stringBuilder.ToString());
		}
		QuestsManager.get_Instance().CurrentActionName = string.Empty;
		CallEvent(1, PAJDEKLLFNJ);
	}

	public virtual void Parse(XmlNode EPKLCPOEELO)
	{
		EFJMDEMAGIM = EPKLCPOEELO.Name;
		JKIPOGOLAAI = LKMGEKCOFMF(XmlUtils.ParseString(EPKLCPOEELO.Attributes["Lock"], string.Empty));
		DPBKBKDCIOI = XmlUtils.ParseString(EPKLCPOEELO.Attributes["Sound"], string.Empty);
	}

	public virtual void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		CallEvent(0, PAJDEKLLFNJ);
		if (LogRules.ELEBLBJKDBI().PIAKPGMPGMN())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("QuestAction ");
			stringBuilder.Append(EFJMDEMAGIM);
			stringBuilder.Append(" started");
			LLLOJBFMONN.INNGABABJPC(stringBuilder.ToString());
		}
		QuestsManager.get_Instance().CurrentActionName = EFJMDEMAGIM;
		if (JKIPOGOLAAI != KHLLOOHAMLC.LOCK_NONE)
		{
			Module.ELEBLBJKDBI().DIDFMBMPEAF(true, JKIPOGOLAAI == KHLLOOHAMLC.LOCK_VISIBLE);
		}
		if (!DPBKBKDCIOI.Equals(string.Empty))
		{
			IFKCCDAIADF();
		}
		PAJDEKLLFNJ = GFIHPBCEEOB;
	}

	public virtual void GKFMJKAAJCA()
	{
	}

	public virtual void Render()
	{
	}

	public virtual void PJGEOIKPGFH()
	{
		QuestStage mLLKDGBEGJI = ListSF.ELEBLBJKDBI().PBGCEEBDBGG(ONGHPGEIJEN);
		if (mLLKDGBEGJI != null)
		{
			mLLKDGBEGJI.MFGLIALECAM();
		}
	}

	public virtual void NLJLHHNPCAO(XmlNode EPKLCPOEELO, QuestActionsSequence AFENHJFICNN, Action<object> ODDEOFKLIAG)
	{
		if (EPKLCPOEELO != null)
		{
			foreach (XmlNode childNode in EPKLCPOEELO.ChildNodes)
			{
				string name = childNode.Name;
				QuestAction mBAAKHELFKL = GetClassActionByName(name);
				mBAAKHELFKL.ONGHPGEIJEN = ONGHPGEIJEN;
				mBAAKHELFKL.Parse(childNode);
				AFENHJFICNN.NLJLHHNPCAO(mBAAKHELFKL);
			}
		}
		AFENHJFICNN.AddEventListener(1, ODDEOFKLIAG);
	}

	public virtual void APKBANHAEGN(XmlNode EPKLCPOEELO, QuestActionsSequence AFENHJFICNN, Action<object> ODDEOFKLIAG)
	{
		NLJLHHNPCAO(EPKLCPOEELO, AFENHJFICNN, ODDEOFKLIAG);
		AFENHJFICNN.AddEventListener(0, OnRunSuccessAndErrorAction);
	}

	public void OnRunSuccessAndErrorAction(object data)
	{
		if (JKIPOGOLAAI != KHLLOOHAMLC.LOCK_NONE)
		{
			Module.ELEBLBJKDBI().DIDFMBMPEAF(false);
		}
	}

	public virtual void EPFCAILHDII(QuestStage DOKAIKMLLDK)
	{
		this.DOKAIKMLLDK = DOKAIKMLLDK;
	}

	public QuestStage NOFNJFOCIMK()
	{
		return DOKAIKMLLDK;
	}

	public KHLLOOHAMLC LKMGEKCOFMF()
	{
		return JKIPOGOLAAI;
	}

	private void IFKCCDAIADF()
	{
		Sound.IFKCCDAIADF(DPBKBKDCIOI);
	}

	private KHLLOOHAMLC LKMGEKCOFMF(string value)
	{
		if (value.Equals("Silent"))
		{
			return KHLLOOHAMLC.LOCK_SILENT;
		}
		if (value.Equals("Visible"))
		{
			return KHLLOOHAMLC.LOCK_VISIBLE;
		}
		return KHLLOOHAMLC.LOCK_NONE;
	}
}

// Compatibility actions ported from the newer quest vocabulary used by the
// plaintext 2.41.x gamedata.
public class QuestActionIf : QuestAction
{
	private readonly List<QuestCondition> _conditions = new List<QuestCondition>();
	private readonly QuestActionsSequence _then = new QuestActionsSequence();
	private readonly QuestActionsSequence _else = new QuestActionsSequence();
	private QuestActionsSequence _running;

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		ParseConditions(node["Conditions"], _conditions);
		NLJLHHNPCAO(node["Then"], _then, OnBranchComplete);
		NLJLHHNPCAO(node["Else"], _else, OnBranchComplete);
	}

	private static void ParseConditions(XmlNode container, List<QuestCondition> output)
	{
		if (container == null)
		{
			return;
		}
		foreach (XmlNode node in container.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			QuestCondition condition = new QuestCondition();
			condition.Parse(node);
			if (condition.LFLGCDNKNJI == QuestCondition.NFFNINLIPJJ.QUEST_CONDITION_OPERATOR)
			{
				ParseConditions(node, condition.conditions);
			}
			output.Add(condition);
		}
	}

	public override void DEJMHFMLKIC(QuestParameters parameters)
	{
		base.DEJMHFMLKIC(parameters);
		bool matches = true;
		foreach (QuestCondition condition in _conditions)
		{
			if (!condition.Compare(parameters, null))
			{
				matches = false;
				break;
			}
		}
		_running = matches ? _then : _else;
		_running.JJIHOMLLAOL = 0;
		_running.FHPKJMMLIEG();
		if (_running.AFENHJFICNN.Count == 0)
		{
			OGIJONMKABB();
			return;
		}
		_running.DEJMHFMLKIC(parameters);
	}

	private void OnBranchComplete(object data)
	{
		OGIJONMKABB();
	}

	public override void GKFMJKAAJCA()
	{
		_then.FHPKJMMLIEG();
		_else.FHPKJMMLIEG();
		_running = null;
	}
}

public class QuestActionRun : QuestAction
{
	private string _name = string.Empty;
	private QuestStage _runningQuest;

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		_name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters parameters)
	{
		base.DEJMHFMLKIC(parameters);
		_runningQuest = ListSF.ELEBLBJKDBI().PBGCEEBDBGG(_name);
		if (_runningQuest == null)
		{
			Debug.LogWarning("[DevXml] Run action could not find quest: " + _name);
			OGIJONMKABB();
			return;
		}
		_runningQuest.AddEventListener(1, OnQuestComplete);
		_runningQuest.MHHNIPBJNAD(parameters, false);
	}

	private void OnQuestComplete(object data)
	{
		if (_runningQuest != null)
		{
			_runningQuest.RemoveEventListener(1, OnQuestComplete);
			_runningQuest = null;
		}
		OGIJONMKABB();
	}
}

public class QuestActionChangeDojoLocation : QuestAction
{
	private string _name = "dojo";

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		_name = node.Attributes["Name"].CIPOICEEIBK("dojo");
	}

	public override void DEJMHFMLKIC(QuestParameters parameters)
	{
		base.DEJMHFMLKIC(parameters);
		ConditionExtension.CompareResult result = new ConditionExtension.CompareResult();
		QuestCondition condition = new QuestCondition();
		condition.LIMHBJBEEIA(parameters);
		condition.MCPIOGALBMK(_name, result);
		GameUtils.NIPABEEAMHJ = result.ToString();
		OGIJONMKABB();
	}
}

public class QuestActionUpdateEclipseBattles : QuestAction
{
	public override void DEJMHFMLKIC(QuestParameters parameters)
	{
		base.DEJMHFMLKIC(parameters);
		Roster roster = ListSF.CCDKHLAMKKO();
		ListSF listSF = ListSF.ELEBLBJKDBI();
		if (roster == null || listSF == null)
		{
			OGIJONMKABB();
			return;
		}
		bool eclipseMode = roster.JPMPIDFGCJL();
		MapScene current = Scene<MapScene>.get_Current();
		Battle selectedBattle = GetSelectedBattle(current);
		Battle selectedReplacement = null;
		List<Battle> changedBattles = new List<Battle>();
		foreach (Battle normalBattle in listSF.MMCHMBIKIEP())
		{
			string eclipseBattleName = GetEclipseBattleName(normalBattle);
			if (string.IsNullOrEmpty(eclipseBattleName))
			{
				continue;
			}
			Zone zone = normalBattle.LKDFFCADHNO();
			Battle eclipseBattle = FindBattle(zone, eclipseBattleName);
			if (eclipseBattle == null)
			{
				continue;
			}
			// A mode switch may only exchange an already unlocked pair. If an old
			// save retained a hidden flag while the Eclipse counterpart was never
			// introduced, restore the normal entry instead of leaving no button.
			if (normalBattle.NNPNEABKHPP() == null)
			{
				continue;
			}
			if (eclipseBattle.NNPNEABKHPP() == null)
			{
				// The newer UpdateEclipseBattles action also introduces the replay
				// counterpart for every battle that has already been unlocked.  The
				// recovered stub only toggled pre-existing roster entries, so a normal
				// playthrough never acquired any Eclipse tournament/challenge entries.
				roster.KJIMPNEGNAN(eclipseBattle, true, true, false, !eclipseMode, 0);
				eclipseBattle.DCHJDPCEODD = true;
				if (eclipseBattle.NNPNEABKHPP() == null)
				{
					SetBattleHidden(normalBattle, false, changedBattles);
					continue;
				}
				changedBattles.Add(eclipseBattle);
			}
			// Base and intermission entries share an Eclipse replacement.  Normal
			// progression removes the base roster entry before it adds the
			// intermission one, but old saves can contain both.  In that case only
			// the current intermission entry may drive the shared replacement.
			if (HasActiveIntermissionSource(normalBattle, zone, eclipseBattleName))
			{
				continue;
			}
			// Run on fight return, mode switches and session initialization so
			// completed Eclipse segments (including old saves) remain replayable.
			BattleReplayable replayable = eclipseBattle as BattleReplayable;
			if (replayable != null && replayable.TryStartNextReplay())
			{
				changedBattles.Add(eclipseBattle);
				if (eclipseMode && eclipseBattle == selectedBattle)
				{
					selectedReplacement = eclipseBattle;
				}
			}
			SetBattleHidden(normalBattle, eclipseMode, changedBattles);
			SetBattleHidden(eclipseBattle, !eclipseMode, changedBattles);
			if (normalBattle == selectedBattle && eclipseMode)
			{
				selectedReplacement = eclipseBattle;
			}
			else if (eclipseBattle == selectedBattle && !eclipseMode)
			{
				selectedReplacement = normalBattle;
			}
		}
		if (changedBattles.Count != 0)
		{
			listSF.EJANJEEGOOE();
			if (current != null)
			{
				foreach (Battle battle in changedBattles)
				{
					current.UpdateBattleButtonHidden(battle);
				}
				// The replacement button used to remain disabled until the player
				// visited another zone.  Reselecting the paired battle rebuilds the
				// preview and reapplies the current zone's input state immediately.
				if (selectedReplacement != null)
				{
					current.SelectBattle(selectedReplacement, 0f);
				}
				else
				{
					current.UpdateCurrentZone();
				}
			}
		}
		OGIJONMKABB();
	}

	private static string GetEclipseBattleName(Battle battle)
	{
		XmlNode node = battle.MMLPEMNIFBD().IOJIGDNFCFL();
		if (node == null || node.Attributes == null)
		{
			return string.Empty;
		}
		XmlAttribute attribute = node.Attributes["EclipseToggleName"];
		return (attribute == null) ? string.Empty : attribute.Value;
	}

	private static Battle FindBattle(Zone zone, string name)
	{
		if (zone == null)
		{
			return null;
		}
		foreach (Battle battle in zone.LGIIBNJFADA)
		{
			if (battle.get_Name() == name)
			{
				return battle;
			}
		}
		return null;
	}

	private static Battle GetSelectedBattle(MapScene mapScene)
	{
		if (mapScene == null)
		{
			return null;
		}
		ZoneScrollItem currentZone = mapScene.GetCurrentZone();
		return (currentZone == null) ? null : currentZone.get_LastBattle();
	}

	private static bool HasActiveIntermissionSource(Battle battle, Zone zone, string eclipseBattleName)
	{
		if (battle.get_Name().EndsWith("_INTERMISSION", StringComparison.Ordinal))
		{
			return false;
		}
		foreach (Battle candidate in zone.LGIIBNJFADA)
		{
			if (candidate == battle || candidate.NNPNEABKHPP() == null || !candidate.get_Name().EndsWith("_INTERMISSION", StringComparison.Ordinal))
			{
				continue;
			}
			if (GetEclipseBattleName(candidate) == eclipseBattleName)
			{
				return true;
			}
		}
		return false;
	}

	private static void SetBattleHidden(Battle battle, bool hidden, List<Battle> changedBattles)
	{
		RosterBattle rosterBattle = battle.NNPNEABKHPP();
		if (rosterBattle == null || rosterBattle.KAPIELMDIIK() == hidden)
		{
			return;
		}
		rosterBattle.HCEOCBOFIGC(hidden);
		changedBattles.Add(battle);
	}
}
