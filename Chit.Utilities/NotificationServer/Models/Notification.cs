using System;
using System.Collections.Generic;

namespace Chit.Utilities;
public class Notification
{
    public Guid Id { get; set; }
    public string TemplateId { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsEmail { get; set; }
    public bool IsSMS { get; set; }
    public bool IsPush { get; set; }
    public bool IsWeb { get; set; }
    public bool IsMobile { get; set; }
}

public class NotificationMessage {
    public string Title { get; set; }
    public string Body { get; set; }
}

public class Notifications {
    /// <summary>
    /// Represents a collection of notifications.
    /// </summary>
    public class NotificationsCollection
    {
        /// <summary>
        /// Gets or sets the list of notifications.
        /// </summary>
        public List<Notification> Items { get; set; }
    }
}
