using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Model.ApplicationModel;
using ProjectAvery.Notification;
using ProjectAvery.Notification.Notifications;

namespace ProjectAvery.Controllers
{
    /// <summary>
    /// Controller for requests that affect the whole application
    /// i.e.: change app settings, create/delete server, etc.
    /// </summary>
    public class ApplicationController : AbstractRestController
    {
        private readonly INotificationCenter _notificationCenter;

        public ApplicationController(ILogger<ApplicationController> logger, INotificationCenter notificationCenter) : base(logger)
        {
            _notificationCenter = notificationCenter;
        }

        [HttpGet("ping")]
        public string Ping()
        {
            _logger.LogDebug("Received Ping from "+Request.HttpContext.Connection.RemoteIpAddress);
            return "Pong";
        }

        [HttpPost("updateState")]
        public void ChangeApplicationState(ApplicationState state)
        {
            _notificationCenter.SendApplicationStateChangedNotification(ApplicationState.STOPPED, state);
        }
    }
}