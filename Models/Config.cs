using System;

namespace OmniusAccruals.Models
{
    public class Config
    {
        public string ProgrammName { get; } = "Accruals";

        public string Version { get; } = "1.2";

        public DateTime DateRelease { get;  } = new DateTime(2021,08,10);

        public string Programmist { get; } = "Poplavskiy Aleksandr";

        public string HOST { get; set; } = "127.0.0.1";

        public string DB { get; set; } = "database";

        public string TRUSTED_CONNECTION { get; set; } = "Yes";

        public string USER { get; set; } = String.Format(@"{0}\\{1}", Environment.UserDomainName, Environment.UserName);

        public string PASSWORD { get; set; } = @"password";

        public string SMTP_HOST { get; set; } = "127.0.0.1";

        public int SMTP_PORT { get; set; } = 25;

        public string SMTP_USER { get; set; }
        
        public string SMTP_PASSWORD { get; set; }

        public string RECEIVER_MAIL { get; set; } = "example@sakh.dvec.ru";

        public bool EXPORT_UTF8 { get; set; } = true;

    }
}
