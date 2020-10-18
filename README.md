![Continuous Integration](https://github.com/TheMulti0/Iris/workflows/Continuous%20Integration/badge.svg)
# Iris

This is Iris - a scalable social media update manager.

The producers use MongoDB to save each configured user's last update time, that is used to make sure no update will be sent twice.

Supported producers sources:
 - [x] YouTube
 - [x] Twitter
 - [x] Facebook
 - [ ] Instagram
 - [ ] Interviews
 
Supported Consumers sources:
 - [x] Telegram
 - [ ] Web dashboard

All of the components use Apache Kafka for communication (The `/updates` topic for all updates, `/configs` topic for configurations).

[This is a deployed working example, Iris is configured to send tweets and posts of the Israeli 'Yamina' party candidates.](https://t.me/YaminaUpdates)
