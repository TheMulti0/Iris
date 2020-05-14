***"The automatic Telegram update notifier bot"***

# Iris

Iris is a Telegram bot that polls updates from configured sources and sends them to configured chat ids.

Iris is built with .NET Core 3.1 (the libraries are targetted to .NET Standard 2.1), deployed with Docker.

> [This is a working sample, configured to post updates from the candidates of the Israeli 'Yamina' party](https://t.me/YaminaUpdates)

Currently supported update sources:
 - [x] Twitter
 - [x] Facebook

The system has 3 services, each running on its own container:
 - `Iris`-  Manages the bot and gets updates from all of the sources.
 - `twitterscraper` - Delivers tweets.
 - `facebookscraper` - Delivers posts.
