# Project Avery

:construction::warning::construction:Important: This repository is not production ready. Please use [Fork](https://github.com/ForkGG/fork) for now. Updates about this on [Discord](https://discord.fork.gg):construction::warning::construction:

Project Avery is the successor of Fork and aims to be a general purpose Minecraft Server Manager GUI, that allows individuals to host Minecraft servers with very little knowledge.

Also it might be worth to mention, that `Avery` is only a working title and most likely not the name of the final product.

---

## Key features

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

## Communications protocol

Avery is based on two communication layers, the first one is a REST-API that is used to send Commands from the Frontend (or any other service) to the Backend. The second one is a WebSocket that is hosted by the backend, where the Frontend (or any other service) can subscribe to notifications, like server status changes, etc.

### REST-API

Running this application will host a swagger documentation, so you can explore the API yourself and get more infos about the endpoints. Notice, that all requests must have the `Authorization`-Header set with the token as value. More Infos about authorization later on.

#### Here is a list of all REST-API Endpoints:

##### App Endpoint

All endoints start with `/v1/application`
| Method | Endpoint | Description |
|---|---|---|
| GET | /state | Get the current state of the application |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |

##### Entity Endpoint

All endoints start with `/v1/entity`
| Method | Endpoint | Description |
|---|---|---|
| PUT | /{EntityId}/start | Starts the specified entity |
| PUT | /{EntityId}/stop | Stops the specified entity |
| PUT | /{EntityId}/restart | Restarts the specified entity by executing stop and start |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |
|  |  |  |



### WebSocket

The WebSocket is used for sending updates to all clients simultaneously, which propagates the state of the app to all clients to keep everything up to date. A typical frontend should send commands through the API and subscribe to this WebSocket to update the UI according to the state of the app.

To subscribe to the WebSocket you need to connect to the WebSocket Server and send the following subscribe message:  
`subscribe|{token}`  
Read more about this and the Token in the Authorization part of this readme

#### Here is a list of all Notifications that are sent through the WebSocket:

Each Notification is sent as JSON blob and is represented by a C# class in this table

##### Application Notifications

| Notification | Parameter Options | Description |
|---|---|---|
| [StateChangeNotification](Logic/Model/NotificationModel/ApplicationNotificationModel/StateChangeNotification.cs) |  | Fired when the application changes state (finished startup, starting shutdown etc.) |
| []() |  |  |
| []() |  |  |
| []() |  |  |
| []() |  |  |
| []() |  |  |

##### Entity Notifications

| Notification | Parameter Options | Description |
|---|---|---|
| []() |  |  |
| []() |  |  |
| []() |  |  |
| []() |  |  |
| []() |  |  |
| []() |  |  |
| []() |  |  |

---

## Authorization

Because the communication of Avery is open to all connections and any 3rd party app can join the party as well as multiple users with different privileges that join at the same time. For this reason there is a token based authorization process at the core of Avery.

### The Basics

In this part we will cover the basics of how the authorization system works

#### 1. Privilege structure

Privileges are based on a tree like structure that decides what a token is able to do and what information it can access.     
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

Each command in the API and each Notification require a certain node in this tree. Granting access to a node of this graph will allow the token access to all the sub-nodes connected to that node.  

Examples:  
Granting access to the `Read Discord Bot` node will only allow him to see the settings not to write them  
Granting access to the `Read AppSettings` node will also only allow to read, but all of the Settings on that page  
Granting access to the `AppSettings` node will allow read and write access to all AppSettings  
Granting access to the `Read AppSettings` node and the `Write Discord Bot` node will allow the token to read all settings, but only modify the Discord Bot settings  

#### 2. The token

The token is a key part of how the authorization works and is used to authenticate against the application. The token should be kept secret from all non authorized people, this means that the token should never be shared and all communication with the app should be done over https.  

There are some basic priciples for tokens:
 - Tokens can be created, changed and deleted at any time by anyone with the according privilege
 - Connections without a token will ALWAYS result in a `401 Unauthorized` response

### How it works

#### 1. Getting a token

As localhost:  
There is a local token created while installing Avery, that automatically has `Admin` priviledges. This token is stored in the installation path of Avery, so everyone with access to that directory has access to that token!  

As remote:
The preferred approach is to make an input field for the user to input a token before connecting to the app. Be carefull when storing the token somewhere!


#### 2. Communication

For local applications you can just read the local `Admin` token and the settings of Avery to get the ports for API and WebSocket. To connect to Avery you use `localhost:port`.  

For remote applications this is a bit more complex, as you need a way to find IP-Adress and Ports for a connection. For this reason you can use the public Avery API, where each token is registered with the according IP, API-Port and WebSocket-Port.  
:construction: TODO CKE more infos about that service :construction:

---

## Useful Resources :construction:

Website  
Frontend  
API  
