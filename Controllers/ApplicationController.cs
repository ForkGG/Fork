using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Model.ApplicationModel;
using ProjectAvery.Logic.Persistence;
using ProjectAvery.Notification;
using ProjectAvery.Notification.Notifications;
using ProjectAveryCommon.Model.Application;

namespace ProjectAvery.Controllers
{
    /// <summary>
    /// Controller for requests that affect the whole application
    /// i.e.: change app settings, create/delete server, etc.
    /// </summary>
    public class ApplicationController : AbstractRestController
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationCenter _notificationCenter;

        public ApplicationController(ILogger<ApplicationController> logger, ApplicationDbContext context, INotificationCenter notificationCenter) : base(logger)
        {
            _context = context;
            _notificationCenter = notificationCenter;
        }

        [HttpGet("state")]
        public State State()
        {
            LogRequest();
            //TODO create caller object with permissions from header token in each request
            return _context.GenerateStateObject();
        }

        [Obsolete]
        [HttpPost("updateState")]
        public void ChangeApplicationState(ApplicationStatus status)
        {
            //_notificationCenter.SendApplicationStateChangedNotification(ApplicationStatus.STOPPED, status);
        }
    }
}