# Discord server research for DE and Eclipse

This one-shot command-line tool reads the message history visible to a Discord bot, stores it in a local SQLite database, and uses the OpenAI Responses API to write a source-linked goals brief. It does not run continuously, post to Discord, or use a normal user account.

## Set up the bot

1. In the [Discord Developer Portal](https://discord.com/developers/applications), create an application and add a bot. Keep its token private.
2. Enable **Message Content Intent** in the bot settings. Without it, Discord may return empty message text even when the bot can see message metadata.
3. Install the bot in the server with the `bot` scope and **View Channels** and **Read Message History** permissions. Grant access only to the channels you want analyzed. A server admin can use the portal's installation link; this tool needs no write permissions.
4. In Discord, enable Developer Mode and copy the server ID. Optionally copy individual channel IDs to limit the export.

Use a bot account. Do not use a user token or an alternate user account as a scraper.

## Export

Run from the repository root in PowerShell:

```powershell
$env:DISCORD_BOT_TOKEN = Read-Host 'Discord bot token'
python Tools/DiscordResearch/discord_research.py export --guild <server-id>
```

For a small first pass, use `--channel <channel-id>` (repeatable). Use `--max-pages 10` to cap the older-history pages fetched per channel in one run; new messages are always caught up without a cap so there are no gaps. Run the same command again to resume older history and pick up new messages. `--no-threads` skips thread discovery. By default, the tool reads regular and announcement channels, active threads, and archived public threads or forum posts that the bot can access. Private archived threads are not enumerated. Message text, textual embeds, and poll questions/options are saved. Attachment **names** are recorded; files and images are not downloaded or interpreted.

The archive is saved to `Tools/DiscordResearch/data/discord.sqlite3`. The `data/` directory is Git-ignored because server messages and AI output may contain private information. Keep it local and share only with people authorized to read those channels. The exporter logs inaccessible channels and continues.

## Analyze

Check the amount of source text before sending it to an AI model:

```powershell
python Tools/DiscordResearch/discord_research.py analyze --dry-run
```

Then choose an OpenAI model available to your API account and generate the report:

```powershell
$env:OPENAI_API_KEY = Read-Host 'OpenAI API key'
python Tools/DiscordResearch/discord_research.py analyze --model gpt-6-astra
```

The report is written to `Tools/DiscordResearch/data/de-goals.md`. Analysis sends message text, author display names, timestamps, and source links to the OpenAI API. It uses `store: false`, processes bounded chunks, caches completed chunk summaries in the local database, and separates suggestions from decisions in the prompt. Re-running uses cached results where the input and model match. For a massive server, review `--dry-run` first: API usage grows with the number of chunks and condensation passes. The report checks that each cited message ID exists in the local archive, but the claims still need human review against the linked messages.

Bot-authored messages are stored but excluded from analysis by default to avoid automated chatter. Add `--include-bots` to `analyze` if bots posted relevant plans or polls.

Discord API docs: [message history](https://docs.discord.com/developers/resources/message#get-channel-messages), [threads](https://docs.discord.com/developers/resources/channel#list-public-archived-threads), [rate limits](https://docs.discord.com/developers/topics/rate-limits). OpenAI Docs: [text generation with the Responses API](https://developers.openai.com/api/docs/guides/text).
