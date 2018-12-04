using System;
using System.Collections.Generic;
using System.Text;

namespace EmailingSystem.Common.Common
{
    public class Constant
    {
        public const string LoggingFormat = "{0} - {1}";
        public const string AppSettingsFile = "appsettings.json";
    }

    public class EmailingSystemTopic
    {
        public const string ProcessStatus = "EmailingSystem.ProcessStatus";
        public const string ProcessPrepareEmail = "EmailingSystem.ProcessPrepareEmail";
        public const string SendEmail = "EmailingSystem.SendEmail";
        public const string Logging = "EmailingSystem.Logging";
    }

    public class DBCollection
    {
        public const string UserCollection = "UserCollection";
        public const string SendEmailCollection = "SendEmailCollection";
    }

    public class LogType
    {
        public const string Error = "Exception";
        public const string Information = "Information";
    }
}
