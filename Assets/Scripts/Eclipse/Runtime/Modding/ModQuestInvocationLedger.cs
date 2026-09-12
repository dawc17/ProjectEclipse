using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Eclipse.Modding
{
    // One native quest run. Lazily creates metadata only when a durable action uses it.
    public sealed class ModQuestInvocationLedger
    {
        private readonly XmlElement quest;
        private readonly bool resume;
        private XmlElement run;

        public ModQuestInvocationLedger(XmlElement quest, bool resume)
        {
            this.quest = quest ?? throw new ArgumentNullException(nameof(quest));
            this.resume = resume;
        }

        public string Operation(int actionIndex)
        {
            if (actionIndex < 0) throw new ArgumentOutOfRangeException(nameof(actionIndex));
            EnsureRun();
            return run.GetAttribute("Id") + "/" + actionIndex.ToString(CultureInfo.InvariantCulture);
        }

        public bool BelongsTo(XmlNode ancestor)
        {
            for (XmlNode node = quest; node != null; node = node.ParentNode)
                if (ReferenceEquals(node, ancestor)) return true;
            return false;
        }

        public bool IsCompleted(int actionIndex)
        {
            Operation(actionIndex);
            return Find(actionIndex)?.Name == "Completed";
        }

        public void CloseRun()
        {
            EnsureRun();
            // Validate every receipt before closing, including actions other than
            // the caller's last action. A pending draw must remain resumable.
            Find(-1);
            if (run["Pending"] != null) throw new InvalidOperationException("Cannot finish a quest with an unclaimed lottery reward.");
            quest.SetAttribute("State", "completed");
        }

        public void BindClaim(int actionIndex, string claimId)
        {
            Operation(actionIndex);
            if (!Guid.TryParseExact(claimId, "N", out _)) throw new ArgumentException("A claim GUID is required.", nameof(claimId));
            var existing = Find(actionIndex);
            if (existing != null)
            {
                if (existing.GetAttribute("Claim") != claimId) throw new InvalidOperationException("Quest action already owns another claim.");
                return;
            }
            var pending = quest.OwnerDocument.CreateElement("Pending");
            pending.SetAttribute("Action", actionIndex.ToString(CultureInfo.InvariantCulture));
            pending.SetAttribute("Claim", claimId);
            run.AppendChild(pending);
        }

        public void Complete(int actionIndex, string claimId)
        {
            Operation(actionIndex);
            if (!Guid.TryParseExact(claimId, "N", out _)) throw new ArgumentException("A claim GUID is required.", nameof(claimId));
            var existing = Find(actionIndex);
            if (existing != null)
            {
                if (existing.GetAttribute("Claim") != claimId)
                    throw new InvalidOperationException("Quest action already completed with another claim.");
                if (existing.Name == "Completed") return;
            }
            var receipt = quest.OwnerDocument.CreateElement("Completed");
            receipt.SetAttribute("Action", actionIndex.ToString(CultureInfo.InvariantCulture));
            receipt.SetAttribute("Claim", claimId);
            if (existing != null) run.ReplaceChild(receipt, existing);
            else run.AppendChild(receipt);
        }

        private XmlElement Find(int actionIndex)
        {
            string key = actionIndex.ToString(CultureInfo.InvariantCulture);
            XmlElement found = null;
            foreach (XmlNode child in run.ChildNodes)
            {
                if (!(child is XmlElement item)) continue;
                if ((item.Name != "Completed" && item.Name != "Pending") || !int.TryParse(item.GetAttribute("Action"), NumberStyles.None,
                    CultureInfo.InvariantCulture, out _) || !Guid.TryParseExact(item.GetAttribute("Claim"), "N", out _))
                    throw new InvalidDataException("Invalid quest invocation receipt.");
                if (item.GetAttribute("Action") != key) continue;
                if (found != null) throw new InvalidDataException("Duplicate quest invocation receipt.");
                found = item;
            }
            return found;
        }

        private void EnsureRun()
        {
            if (run != null)
            {
                if (!ReferenceEquals(run.ParentNode, quest)) throw new InvalidOperationException("Quest invocation belongs to an expired run.");
                return;
            }
            var existing = quest["EclipseInvocations"];
            if (existing != null && (existing.GetAttribute("Format") != "1" ||
                !Guid.TryParseExact(existing.GetAttribute("Id"), "N", out _)))
                throw new InvalidDataException("Unsupported quest invocation ledger.");
            if (resume && existing != null) { run = existing; return; }
            if (existing?["Pending"] != null)
                throw new InvalidOperationException("Finish the pending quest claim before starting another run.");
            run = quest.OwnerDocument.CreateElement("EclipseInvocations");
            run.SetAttribute("Format", "1");
            run.SetAttribute("Id", Guid.NewGuid().ToString("N"));
            if (existing != null) quest.ReplaceChild(run, existing);
            else quest.AppendChild(run);
        }
    }
}
