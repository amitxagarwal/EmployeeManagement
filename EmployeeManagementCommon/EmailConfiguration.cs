using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementCommon
{
	public class EmailConfiguration
	{
		public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
		public string From { get; set; }

		public string ImapServer { get; set; }
		public int ImapPort { get; set; }
		public string ImapUsername { get; set; }
		public string ImapPassword { get; set; }
	}
}
