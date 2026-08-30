using System;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCleanupFixture.Session
{
    public static class Trace
    {
        public static readonly List<string> Events = new List<string>();
        public static bool RunQuests;
    }
    public class GiveLogin { public void PGAJKMOPDIJ() { Trace.Events.Add("rewards"); } }
    public class LedgerManager { }
    public static class LLLOJBFMONN { public static void INNGABABJPC(string message) { } }
    public struct FightIDS { public static FightIDS Empty() { return new FightIDS(); } }
    public static class QuestEvent { public enum PMDPDMFLCIJ { QUEST_EVENT_LOGIN_END, QUEST_EVENT_SESSION } }
    public class QuestParameters
    {
        public object ActiveQuest;
        public FightIDS JLGLBLDPAAF;
        public string HEIADONEACH = "active";
        public object LBGOMJFFEPP() { return ActiveQuest; }
    }
    public class ListSF
    {
        private static readonly ListSF instance = new ListSF();
        public readonly QuestParameters Quest = new QuestParameters();
        public static ListSF ELEBLBJKDBI() { return instance; }
        public static ListSF CCDKHLAMKKO() { return instance; }
        public void MAOPKFNKHOI() { Trace.Events.Add("first-session"); }
        public void BIHELGAGPGO() { Trace.Events.Add("local-update"); }
        public QuestParameters BNMLDPNCMLB() { return Quest; }
        public bool FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ evt) { Trace.Events.Add(evt.ToString()); return Trace.RunQuests; }
        public void MHHNIPBJNAD() { Trace.Events.Add("quest-run"); }
    }
}

namespace AssemblyCleanupFixture.Backend
{
    public interface JNEBPDNJFJG
    {
        void VerifyPurchaseAction(JLDHCFFAIPK product, string platform, Action<bool, string, object> callback);
        void ConfirmVerificationAction(JLDHCFFAIPK product, string platform, Action<bool, string, object> callback);
    }
    public class JLDHCFFAIPK { }
    public class ServerProviderBase
    {
        public static ServerProviderBase BPCBBHAKFDM;
        public static ServerProviderBase get_Instance() { return null; }
        public static T Init<T>() where T : new() { return new T(); }
        protected virtual void Init() { }
        protected virtual string NFKOPHMCLFF() { return null; }
        protected virtual IEnumerator TimeSyncRoutine(Action<long> done, Action<string> error) { yield break; }
        public IEnumerator Pending;
        public void StartCoroutine(IEnumerator routine) { Pending = routine; }
        public void Drain() { while (Pending.MoveNext()) { } }
        public IEnumerator Clock(Action<long> done, Action<string> error) { return TimeSyncRoutine(done, error); }
    }
}

namespace AssemblyCleanupFixture
{
    public static class Tests
    {
        private static int checks;
        private static void Check(bool value, string message)
        {
            if (!value) throw new Exception(message);
            checks++;
        }
        public static string Run()
        {
            var session = Session.NetworkController.ELEBLBJKDBI();
            Check(ReferenceEquals(session, Session.NetworkController.BPCBBHAKFDM), "Session singleton changed");
            Check(session.LBDHOLEICEG != null && session.KDILDKDNIID != null, "Local reward/ledger helpers missing");
            int completions = 0;
            session.OnLoginComplete += state => {
                Check(state == null, "Login fabricated remote data");
                Session.Trace.Events.Add("complete"); completions++;
            };
            for (int i = 0; i < 4; i++)
            {
                bool active = i % 2 == 1;
                Session.Trace.RunQuests = i >= 2;
                var quest = Session.ListSF.ELEBLBJKDBI().Quest;
                quest.ActiveQuest = active ? new object() : null;
                quest.HEIADONEACH = "active";
                Session.Trace.Events.Clear();
                session.IFFDOFMDABC();
                string run = Session.Trace.RunQuests ? ",quest-run" : "";
                string expected = (i == 0 ? "first-session," : "") +
                    "local-update,rewards,QUEST_EVENT_LOGIN_END" + run + ",complete,QUEST_EVENT_SESSION" + run;
                Check(string.Join(",", Session.Trace.Events) == expected, "Local session ordering changed");
                Check(quest.HEIADONEACH == (active ? "active" : ""), "Active quest state was cleared incorrectly");
            }
            Check(completions == 4, "Login completion count changed");
            var server = Backend.ServerProvider.get_Instance();
            var product = new Backend.JLDHCFFAIPK();
            object expectedState = null;
            int responses = 0;
            Action<bool, string, object> callback = (success, error, state) => {
                Check(!success && error == "offline build", "Request did not report unavailable");
                Check(ReferenceEquals(state, expectedState), "Request lost caller state"); responses++;
            };
            Action[] requests = {
                () => server.VerifyPurchaseAction(product, "Android", callback),
                () => server.ConfirmVerificationAction(product, "iOS", callback),
                () => server.SendGiveLogin(callback),
                () => server.CheckLedger("https://unused.invalid", callback),
                () => server.ConfirmLedger("https://unused.invalid", callback, "ids")
            };
            for (int i = 0; i < requests.Length; i++)
            {
                expectedState = i < 2 ? product : null;
                requests[i]();
                Check(responses == i, "Callback bypassed coroutine dispatch");
                server.Drain();
                Check(responses == i + 1, "Callback did not run exactly once");
            }
            server.VerifyPurchaseAction(null, null, null); server.Drain();
            server.ConfirmVerificationAction(null, null, null); server.Drain();
            server.SendGiveLogin(null); server.Drain();
            server.CheckLedger(null, null); server.Drain();
            server.ConfirmLedger(null, null, null); server.Drain();
            int downloads = 0;
            server.DownloadFile("https://unused.invalid", (bytes, error, url) => {
                Check(bytes.Length == 0 && error == "offline build" && url == "https://unused.invalid", "Download facade changed"); downloads++;
            });
            Check(downloads == 1, "Download completion count changed");
            server.DownloadFile(null, null);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), received = 0;
            int clockCalls = 0;
            var clock = server.Clock(time => { received = time; clockCalls++; }, error => { throw new Exception(error); });
            Check(!clock.MoveNext(), "Local clock started asynchronous work");
            Check(clockCalls == 1 && received >= now && received <= now + 2, "Local clock no longer uses UTC");
            Check(!server.Clock(null, null).MoveNext(), "Null clock callback should complete safely");
            return $"PASS: {checks} session/backend assertions (actual source; Unity scheduling and game dependencies simulated).";
        }
    }
}
