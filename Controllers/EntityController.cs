﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAvery.Notification;
using ProjectAveryCommon.Model.Entity.Enums.Console;
using ProjectAveryCommon.Model.Entity.Transient.Console;
using ProjectAveryCommon.Model.Notifications.EntityNotifications;

namespace ProjectAvery.Controllers
{
    /// <summary>
    /// Controller for requests that affect a single entity
    /// i.e: start/stop server, change server settings,...
    /// </summary>
    public class EntityController : AbstractRestController
    {
        private readonly INotificationCenter _notificationCenter;
        
        public EntityController(ILogger<EntityController> logger, INotificationCenter notificationCenter) : base(logger)
        {
            _notificationCenter = notificationCenter;
        }

        [Consumes("text/plain")]
        [HttpPost("consoleIn")]
        public async Task<StatusCodeResult> ConsoleIn([FromBody] string message)
        {
            if (string.IsNullOrEmpty(message) || message == "/")
            {
                return BadRequest();
            }
            ConsoleAddNotification notification = new ConsoleAddNotification();
            notification.NewConsoleMessage = new ConsoleMessage(message, ConsoleMessageType.UserInput);
            await _notificationCenter.BroadcastNotification(notification);
            return Ok();
        }
    }
}