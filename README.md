<img src="https://i.imgur.com/Sb7FfrL.png" width="175pt"></img>

![Build and push images](https://github.com/TheMulti0/Iris/workflows/Build%20and%20push%20images/badge.svg)

# Iris

This is Iris - a scalable social media update manager.

The producers use MongoDB to save each configured user's last update time, that is used to make sure no update will be sent twice.

Supported producers sources:
 - [x] Twitter
 - [x] Facebook
 - [x] RSS Feeds (only text & audio items are currently supported)
 - [ ] YouTube
 - [ ] Instagram
 - [ ] Soundcloud
 - [ ] Interviews
 
Supported Consumers sources:
 - [x] Telegram
 - [ ] Web dashboard

All of the components use RabbitMQ for communication.

[This is a deployed working example, Iris is configured to send tweets and posts of the Israeli 'Yamina' party candidates.](https://t.me/YaminaUpdates)

The code of Iris belongs to TheMulti0, and use of it should be done only with his confirmation.

### Architecture

<img src="https://i.imgur.com/Wtp5MgL.png"></img>

### Deploying

Iris has a `docker-compose.yml` file you can easily use to deploy all of its micro-services:

```
> git clone https://github.com/TheMulti0/Iris.git
> cd Iris
> docker-compose up -d
```

### Configuring

All components are configured using JSON files; `appsettings.json` by default, and `appsettings.Development.json` (`Development` is set by the `ENVIRONMENT` environment variable).

> In production, the configuration file of each service is stored at `/app/appsettings.json` (inside container).
