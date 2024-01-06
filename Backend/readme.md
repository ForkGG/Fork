# Fork

:construction::warning::construction:Important: This repository is not production ready. Please
use [Fork-legacy](https://github.com/ForkGG/Fork-legacy) for now. Updates about this
on [Discord](https://discord.fork.gg):construction::warning::construction:

This is the successor of Fork-legacy and aims to be a general purpose Minecraft Server Manager GUI, that allows
individuals to host Minecraft servers with very little knowledge.

---

## Key features

- Platform independent (Runs on Windows, Mac and Linux)
- Clean and seamless UI
- Free and Open Source (no hidden costs and everyone can contribute code)
- Fast server setup (Most of the stuff is less than 5 clicks away)
- Support for a wide range of different servers (Vanilla, Paper, Waterfall, ...)
- Integrated Discord-Bot to control your server from Discord
- Automatic server start, stop and restart
- In-App Code Editor for editing you server configuration files
- and many many more...

---

# Stuff for nerds

## App structure

This repository is only the backend part of the Fork App, which is the core application. There is also a frontend
implemented in Blazor Webassambly found in the [ForkFrontend repository](https://github.com/ForkGG/ForkFrontend) and a
common library hosted found in the [ForkCommon repository](https://github.com/ForkGG/ForkCommon).

As both the backend and frontend are written in C# and specifically in .NET 6 we can use the common library as a shared
space for both of them and that's why the objects, that are transported between them are there.

## Communications protocol

Fork is based on two communication layers, the first one is a REST-API that is used to send commands from the frontend (
or any other service) to the backend. The second one is a WebSocket that is hosted by the backend, where the frontend (
or any other service) can subscribe to notifications, like server status changes, etc.

### REST-API

Running this application in development mode will host a swagger documentation on `http://localhost:35565/swagger`, so
you can explore the API yourself and get more infos about the endpoints. Notice, that all requests must have
the `Authorization`-Header set with the token as value. More Infos about authorization later on.
This documentation will also be provided on the Fork website at a later stage.

### Here is a list of all REST-API Endpoints:

#### App Endpoint

All endoints start with `/v1/application`
| Method | Endpoint | Required Privilege | Description |
|---|---|---|---|
| GET | /state | any | Get the current state of the application
as [State object](https://github.com/ForkGG/ForkCommon/blob/main/Model/Application/State.cs) |
| GET | /privileges | any | Get a list of privileges associated to the provided token |
| | | | |

#### Entity Endpoint

All endoints start with `/v1/entity`
| Method | Endpoint | Required Privilege | Description |
|---|---|---|---|
| POST | /createserver | CreateEntityPrivilege | Creates a new server based on the
provided [CreateServerPayload](https://github.com/ForkGG/ForkCommon/blob/main/Model/Payloads/Entity/CreateServerPayload.cs) |
| POST | /{EntityId}/start | WriteConsoleTabPrivilege | Starts the specified entity |
| POST | /{EntityId}/stop | WriteConsoleTabPrivilege | Stops the specified entity |
| POST | /{EntityId}/restart | WriteConsoleTabPrivilege | Restarts the specified entity by executing stop and start |
| POST | /{EntityId}/consolein | WriteConsoleTabPrivilege | Write a line to the console of the specified entity. The
body is interpreted as plain text input |
| GET | /{EntityId}/console | ReadConsoleConsoleTabPrivilege | Get a list of all console messages for an entity (Max.
1000 messages) |
| | | | |

### WebSocket

The WebSocket is used for sending updates to all clients simultaneously, which propagates the state of the app to all
clients to keep everything up to date. A typical frontend should send commands through the API and subscribe to this
WebSocket to update the UI according to the state of the app.

To subscribe to the WebSocket you need to connect to the WebSocket Server and send the following subscribe message:  
`{token}`
Read more about this and the Token in the Authorization part of this readme

At the current state your WebSocket connection is then getting all notifications that are emitted if you have the
privilege to receive them. At a later stage there will be a more advanced mechanism, where the UI can register to
certain notifications, which helps in network performance.

### Here is a list of all Notifications that are sent through the WebSocket:

Each Notification is sent as JSON blob and is represented by a C# class in this table

#### Application Notifications

| Notification | Required Privilege | Description |
|--------------|--------------------|-------------|
| []()         |                    |             |
| []()         |                    |             |
| []()         |                    |             |
| []()         |                    |             |
| []()         |                    |             |

#### Entity Notifications

Each notification has an EntityId from the related entity attached

| Notification                                                                                                                                                                         | Required Privilege                | Description                                                                         |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------|-------------------------------------------------------------------------------------|
| [ConsoleAddNotification](https://github.com/ForkGG/ForkCommon/blob/main/Model/Notifications/EntityNotifications/ConsoleAddNotification.cs)                                           | ReadConsoleConsoleTabPrivilege    | Fired after a line is added to the console by either a user or the Minecraft server |
| [EntityStatusChangedNotification](https://github.com/ForkGG/ForkCommon/blob/main/Model/Notifications/EntityNotifications/EntityStatusChangedNotification.cs)                         | IReadEntityPrivilege              | Fired after the status of an entity changes (Started, Stopped, ...)                 |
| [UpdatePlayerNotification](https://github.com/ForkGG/ForkCommon/blob/main/Model/Notifications/EntityNotifications/PlayerNotifications/UpdatePlayerNotification.cs)                   | ReadPlayerlistConsoleTabPrivilege | Fired after a player on the playerlist is updated (Added or Changed)                |
| [UpdateBanlistPlayerNotification](https://github.com/ForkGG/ForkCommon/blob/main/Model/Notifications/EntityNotifications/PlayerNotifications/UpdateBanlistPlayerNotification.cs)     | ReadBanlistConsoleTabPrivilege    | Fired after a player is added, removed or updated on the banlist                    |
| [UpdateWhitelistPlayerNotification](https://github.com/ForkGG/ForkCommon/blob/main/Model/Notifications/EntityNotifications/PlayerNotifications/UpdateWhitelistPlayerNotification.cs) | ReadWhitelistConsoleTabPrivilege  | Fired after a player is added, removed or updated on the whitelist                  |
| []()                                                                                                                                                                                 |                                   |                                                                                     |

---

## Authorization

Because the communication of Fork is open to all connections and any 3rd party app as well as multiple users with
different privileges can join at the same time. For this reason there is a token based authorization process at the core
of Fork.

### The Basics

In this part we will cover the basics of how the authorization system works

#### 1. Privilege structure

Privileges are based on a tree like structure that decides what a token is able to do and what information it can
access.     
The full tree:

```
Admin
├── Application
│   ├── Create Entity
│   ├── Delete Entity
│   ├── Rename Entity
│   └── Update Application (Stops all servers)
├── AppSettings
│   ├── Read AppSettings
│   │   ├── Read General
│   │   ├── Read Discord Bot
│   │   ├── Read Tokens (Only token settings not the acutal tokens)
│   │   └── Read Advanced
│   └── Write AppSettings
│       ├── Write General
│       ├── Write Discord Bot
│       ├── Write Tokens (This can be used to gain admin!)
│       └── Write Advanced
└── Entity (One Node for each entity)
    ├── Read Entity
    │   ├── Read ConsoleTab
    │   │   ├── Read Console
    │   │   ├── Read Playerlist
    │   │   ├── Read Banlist
    │   │   └── Read Whitelist
    │   ├── Read SettingsTab
    │   │   ├── Read Settings
    │   │   ├── Read server.properties
    │   │   └── Read version specific files
    │   ├── Read WorldsTab
    │   ├── Read PluginsTab (only if available)
    │   └── Read ModsTab (only if available)
    └── Write Entity
        ├── Write ConsoleTab
        ├── Write SettingsTab
        │   ├── Write Settings
        │   ├── Write server.properties
        │   └── Write version specific files
        ├── Write WorldsTab
        ├── Write PluginsTab (only if available)
        └── Write ModsTab (only if available)
```

Each command in the API and each Notification require a certain node in this tree. Granting access to a node of this
graph will allow the token access to all the sub-nodes connected to that node.

Examples:  
Granting access to the `Read Discord Bot` node will only allow him to see the settings not to write them  
Granting access to the `Read AppSettings` node will also only allow to read, but all of the Settings on that page  
Granting access to the `AppSettings` node will allow read and write access to all AppSettings  
Granting access to the `Read AppSettings` node and the `Write Discord Bot` node will allow the token to read all
settings, but only modify the Discord Bot settings

#### 2. The Token

The token is a key part of how the authorization works and is used to authenticate against the application. The token
should be kept secret from all non authorized people, this means that the token should never be shared and all
communication with the app should be done over https.

There are some basic principles for tokens:

- Tokens can be created, changed and deleted at any time by anyone with the according privilege
- Connections without a token will ALWAYS result in a `401 Unauthorized` response

### How it works

#### 1. Getting a token

As localhost:  
There is a local token created while installing Fork, which automatically has `Admin` priviledges. This token is stored
in the installation path of Fork, so everyone with access to that directory has access to that token!

As remote:
The preferred approach is to make an input field for the user to input a token before connecting to the app. Be carefull
when storing the token somewhere!

#### 2. Communication

For local applications you can just read the local `Admin` token and the settings of Fork to get the ports for API and
WebSocket. To connect to Fork you use `localhost:port`.

For remote applications this is a bit more complex, as you need a way to find IP-Adress and Ports for a connection. For
this reason you can use the public Fork API, where each token is registered with the according IP, API-Port and
WebSocket-Port.  
:construction: This service is not built yet :construction:

---

## Useful Resources

[Fork Website](https://fork.gg)<br>
[Frontend repository](https://github.com/ForkGG/ForkFrontend)<br>
[Common library](https://github.com/ForkGG/ForkCommon)<br>
[API](#)  
