using System.Text.Json.Serialization;

namespace ProjectAvery.Logic.Model.ApplicationModel
{
    /// <summary>
    /// The states the application can have
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApplicationState
    {
        // Application is stopped (only here to complete the list)
        STOPPED,
        // Application is starting up
        STARTUP,
        // Application is updating itself
        UPDATING,
        // Application is running and ready
        STARTED,
        // Application is shutting down
        SHUTDOWN
    }
}