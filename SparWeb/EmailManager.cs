using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SparWeb
{
	public partial class EmailManager
	{
		public enum EmailTypes
		{
			EmailConfirmationTemplate = 1,
			SparRequestInitialTrainerToTrainerTemplate = 2,
			SparRequestInitialFighterToTrainerTemplate = 3,
			SparRequestInitialTrainerToFighterTemplate = 4,
			SparRequestInitialFighterToFighterTemplate = 5,
			SparRequestFirstTimeResponseTrainerToTrainerTemplate = 6,
			SparRequestFirstTimeResponseFighterToTrainerTemplate = 7,
			SparRequestFirstTimeResponseTrainerToFighterTemplate = 8,
			SparRequestFirstTimeResponseFighterToFighterTemplate = 9,
			SparRequestNegotiationTrainerToTrainerTemplate = 10,
			SparRequestNegotiationFighterToTrainerTemplate = 11,
			SparRequestNegotiationTrainerToFighterTemplate = 12,
			SparRequestNegotiationFighterToFighterTemplate = 13,
			SparRequestConfirmedTrainerToTrainerTemplate = 14,
			SparRequestConfirmedFighterToTrainerTemplate = 15,
			SparRequestConfirmedTrainerToFighterTemplate = 16,
			SparRequestConfirmedFighterToFighterTemplate = 17,
			SparRequestCancelledTrainerToTrainerTemplate = 18,
			SparRequestCancelledFighterToTrainerTemplate = 19,
			SparRequestCancelledTrainerToFighterTemplate = 20,
			SparRequestCancelledFighterToFighterTemplate = 21,
			ContactFormTemplate = 22,
			NewFighterMemberNotificationTemplate = 23,
			NewTrainerMemberNotificationTemplate = 24,
			PasswordResetTemplate = 25,
			SparRequestNotificationTemplate = 26
		}

		public static void SendEmail(EmailTypes emailType, string fromEmail, string toEmail, string subject, Dictionary<string, string> placeholders)
		{
			string emailLayout = "";
			using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Data/EmailTemplates/_LayoutTemplate.html")))
			{
				emailLayout = sr.ReadToEnd();
			}

			string emailContent = "";
			using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath(String.Format("~/App_Data/EmailTemplates/{0}.html", emailType.ToString()))))
			{
				emailContent = sr.ReadToEnd();
			}

			foreach (var key in placeholders.Keys)
			{
				emailContent = emailContent.Replace(key, placeholders[key]);
			}

			string emailBody = emailLayout.Replace("[TEMPLATE_CONTENT]", emailContent);
			SendEmail(fromEmail, toEmail, subject, emailBody);
		}

		public static void SendEmail(string fromEmail, string toEmail, string subject, string body)
		{
			WebMail.SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];
			WebMail.UserName = ConfigurationManager.AppSettings["SmtpUserName"];
			WebMail.Password = ConfigurationManager.AppSettings["SmtpPassword"];
			WebMail.SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
			WebMail.From = fromEmail;

			WebMail.Send(toEmail, subject, body);
		}
	}
}