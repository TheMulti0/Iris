![Continuous Integration](https://github.com/TheMulti0/Iris/workflows/Continuous%20Integration/badge.svg)
# Iris

This is Iris - a scalable social media update manager.

The producers use MongoDB to save each configured user's last update time, that is used to make sure no update will be sent twice.

Supported producers sources:
 - [x] Twitter
 - [x] Facebook
 - [ ] YouTube
 - [ ] Instagram
 - [ ] Soundcloud
 - [ ] Interviews
 
Supported Consumers sources:
 - [x] Telegram
 - [ ] Web dashboard

All of the components use Apache Kafka for communication (The `/updates` topic for all updates, `/configs` topic for configurations).

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
    "SubscriptionTopics": [
      "updates"
    ],
    "GroupId": "updates-consumers",
    "BrokersServers": "kafka:9092",
    "KeySerializationType": "String",
    "ValueSerializationType": "Json"
  },
  "ConfigConsumer": {
    "SubscriptionTopics": [
      "configs"
    ],
    "GroupId": "telegram-config-consumers",
    "BrokersServers": "kafka:9092",
    "KeySerializationType": "String",
    "ValueSerializationType": "String"
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

> Note: if the `mongodb` node will not be found then an in-memory database will be used

> Note 2: the `video_downloader` node is optional, it is used to pass parameters to `yt-dl`

> Note 3: in `producer`, the field `store_sent_updates` enables 24-hour period store of sent urls, useful for websites in which for the first 24-hour period the timestamp on the post is relative and not absolute 

```json
{
  "kafka": {
    "bootstrap_servers": [
      "kafka:9092"    
    ],
    "configs": {
      "topic": "configs"
    },
    "updates": {
      "topic": "updates", 
      "key": "fromproducer1"
    }
  },
  "mongodb": {
    "connection_string": "mongodb://localhost:27017/",
    "db": "ProducerDB"
  },
  "video_downloader": {
    "username": "username@tothiswebsite.com",
    "password": "safepassword"
  },
  "poller": {
    "update_interval_seconds": 60,
    "watched_users": [
      "example-user",
      "example-user2"
    ],
    "store_sent_updates": true
  }
}
```

##### `twitter-producer`

`twitter-producer` has an additional node in the configuration: 
```json
"twitter": {
  "consumer_key": "consumer-key",
  "consumer_secret": "consumer-key-secret",
  "access_token": "access-token",
  "access_token_secret": "access-token-secret",
} 
```

##### `youtube-producer`

`youtube-producer` has an additional node in the configuration: 
```json
"youtube": {
  "api_key": "youtube-api-key"
} 
```
