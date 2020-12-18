![Continuous Integration](https://github.com/TheMulti0/Iris/workflows/Continuous%20Integration/badge.svg)
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

#### TelegramBot

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "System": "Information",
            "Microsoft": "Information"
        }
    },
    "UpdatesConsumer": {
        "ConnectionString": "amqp://guest:guest@rabbitmq:5672//",
        "Destination": "updates"
    },
    "Telegram": {
        "AccessToken": "telegram-access-token",
        "Users": [
            {
                "UserNames": [
                    "example-user",
                    "example-user2"
                ],
                "DisplayName": "Example user",
                "ChatIds": [
                    "@ExampleChatId",
                    -111111111
                ]
            }
        ],
        "FilterRules": [
            {
                "UserNames": [
                    "example-user2"
                ],
                "ChatIds": [
                    "@ExampleChatId"        
                ],
                "SkipReposts": true,
                "HideMessagePrefix": true,
                "DisableMedia": true
            }
        ]
    },
    "Sentry": {
        "Dsn": "sentry-dsn",
        "MinimumBreadcrumbLevel": "Information",
        "MinimumEventLevel": "Warning"
    }
}
```

#### Producers

This is a base configuration example for all update producers in the system:

> Note: if the `MongoDB` node will not be found then an in-memory database will be used

> Note 3: in `producer`, the field `store_sent_updates` enables store of sent urls for configured period, useful for websites in which the timestamp on the post is relative and not absolute 

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        }
    },
    "MongoDb": {
        "ConnectionString": "mongodb://mongodb:27017",
        "DatabaseName": "ProducerDB",
        "SentUpdatesExpiration": "48:00:00"
    },
    "UpdatesPublisher": {
        "ConnectionString": "amqp://guest:guest@rabbitmq:5672//",
        "Destination": "amq.topic"
    },
    "UpdatesProvider": {
        "Name": "MyProducer"
    },
    "Poller": {
        "Interval": "00:00:10",
        "WatchedUserIds": [
            "example-user",
            "example-user2"
        ],
        "StoreSentUpdates": false
    },
    "VideoExtractor": {
        "FormatRequest": "best",
        "UserName": "optional username",
        "Password": "optional password"
    },
    "Sentry": {
        "Dsn": "sentry-dsn",
        "MinimumBreadcrumbLevel": "Information",
        "MinimumEventLevel": "Warning"
    }
}
```

##### TwitterProducer

TwitterProducer has additional fields in the `UpdatesProvider` node:
```json
"UpdatesProvider": {
    "Name": "Twitter",
    "ConsumerKey": "consumerkey",
    "ConsumerSecret": "consumersecret",
    "AccessToken": "accesstoken",
    "AccessTokenSecret": "d5w3LWl8eef52xSGJfj61OJeF606ddJxdVSMTpzknKfZ6"
},
```
