![Continuous Integration](https://github.com/TheMulti0/Iris/workflows/Continuous%20Integration/badge.svg)
# Iris

This is Iris - a scalable social media update manager.

The project consists of 4 components:
 - `ConfigProducer` (Currently not deployed) - Produces configuration to all components.
 - `twitterproducer` - Produces configured user's tweets as Updates.
 - `facebookproducer` - Produces configured user's posts as Updates.
 - `youtubeproducer` - Produces configured user's videos as Updates.
 - `TelegramConsumer` - Consumes updates and sends to configured Telegram chatid.

The producers use MongoDB to save each configured user's last update time, that is used to make sure no update will be sent twice.

All of the components use Apache Kafka for communication (The `/updates` topic for all updates, `/configs` topic for configurations).

[This is a deployed working example, Iris is configured to send tweets and posts of the Israeli 'Yamina' party candidates.](https://t.me/YaminaUpdates)
