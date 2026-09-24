import contextlib
import io
import tempfile
import unittest
from argparse import Namespace
from pathlib import Path
from unittest import mock

import discord_research as research


class FakeDiscord:
    def __init__(self, message_ids):
        self.message_ids = message_ids

    def get(self, path, **params):
        before = params.get("before")
        ids = [value for value in self.message_ids if before is None or value < before]
        return [{"id": str(value), "timestamp": "2026-09-24T00:00:00Z",
                 "content": f"Goal {value}", "author": {"id": "42", "username": "Seby"},
                 "message_reference": None, "attachments": []} for value in ids[:100]]


class ExportTests(unittest.TestCase):
    def test_message_text_includes_embeds_and_polls(self):
        message = {"content": "DE plan", "embeds": [
            {"title": "Roadmap", "fields": [{"name": "Feature", "value": "New bosses"}]}],
            "poll": {"question": {"text": "Which mode?"}, "answers": [
                {"poll_media": {"text": "Eclipse"}}]}}
        text = research.message_text(message)
        self.assertIn("DE plan", text)
        self.assertIn("New bosses", text)
        self.assertIn("Which mode? | options: Eclipse", text)

    def test_large_channel_resumes_and_collects_new_messages(self):
        with tempfile.TemporaryDirectory() as temp:
            db = research.open_db(Path(temp) / "messages.sqlite3")
            channel = {"id": "9", "name": "ideas", "type": 0}
            db.execute("INSERT INTO channels(id, guild_id, name, kind) VALUES (9, 8, 'ideas', 0)")
            api = FakeDiscord(list(range(205, 0, -1)))
            for expected in (100, 200, 205):
                research.export_channel(api, db, 8, channel, max_pages=1)
                self.assertEqual(db.execute("SELECT COUNT(*) FROM messages").fetchone()[0], expected)
            self.assertEqual(db.execute("SELECT complete FROM channels").fetchone()[0], 1)
            api.message_ids = list(range(410, 0, -1))
            research.export_channel(api, db, 8, channel, max_pages=1)
            self.assertEqual(db.execute("SELECT COUNT(*) FROM messages").fetchone()[0], 410)
            self.assertEqual(db.execute("SELECT complete FROM channels").fetchone()[0], 1)
            chunks = research.build_chunks(db, 8, max_chars=4000)
            self.assertTrue(chunks)
            self.assertIn("https://discord.com/channels/8/9/410", chunks[-1])
            db.close()

            with contextlib.redirect_stdout(io.StringIO()) as output:
                research.analyze(Namespace(db=Path(temp) / "messages.sqlite3", guild="8",
                                           chunk_chars=4000, include_bots=False,
                                           dry_run=True, model=None,
                                           out=Path(temp) / "report.md"))
            self.assertIn("410 stored messages", output.getvalue())
            self.assertFalse((Path(temp) / "report.md").exists())

            args = Namespace(db=Path(temp) / "messages.sqlite3", guild="8",
                             chunk_chars=24000, include_bots=False, dry_run=False,
                             model="test-model", out=Path(temp) / "report.md")
            source = "https://discord.com/channels/8/9/410"
            fake_response = {"output": [{"type": "message", "content": [
                {"type": "output_text", "text": f"Goal [source]({source})"}]}]}
            with mock.patch.dict("os.environ", {"OPENAI_API_KEY": "test-key"}), \
                 mock.patch.object(research, "request_json", return_value=fake_response) as request:
                with contextlib.redirect_stdout(io.StringIO()):
                    research.analyze(args)
                    first_run_requests = request.call_count
                    research.analyze(args)
                self.assertEqual(request.call_count, first_run_requests)
            self.assertIn(source, args.out.read_text(encoding="utf-8"))


if __name__ == "__main__":
    unittest.main()
