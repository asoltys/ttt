using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Net.Mail;
using TransformationTimelineTool.Helpers;

namespace TransformationTimelineTool.Helpers
{
    public static class Mailer
    {
        private readonly static string _ETIHost = "smtp.ctst.email-courriel.canada.ca";
        private readonly static int _ETIHostPort = 587;
        private readonly static string _ETIServiceAccount = "TPSGC.DGDPIPacifique-CIOBPacific.PWGSC@ctst.canada.ca";
        private readonly static string _ETIServiceAccountPass = "4t8D=Y$xr+4R$z7R%f5F9a$G4M!M3q";

        public static void Send(string Recipient, string Subject, string Message)
        {
            MailAddress From = new MailAddress(_ETIServiceAccount, "Transformation Timeline Tool", Encoding.UTF8);
            MailAddress To = new MailAddress(Recipient);
            using (MailMessage Mail = new MailMessage(From, To))
            using (SmtpClient SMTP = new SmtpClient(_ETIHost, _ETIHostPort))
            {
                // MailMessage setup
                Mail.Subject = Subject;
                Mail.SubjectEncoding = Encoding.UTF8;
                Mail.Body = Message;
                Mail.BodyEncoding = Encoding.UTF8;
                Mail.IsBodyHtml = true;

                // SMTP client setup
                NetworkCredential credential = new NetworkCredential(_ETIServiceAccount, _ETIServiceAccountPass);
                SMTP.EnableSsl = true;
                SMTP.UseDefaultCredentials = false;
                SMTP.Credentials = credential;
                SMTP.Timeout = 10000;

                try
                {
                    SMTP.Send(Mail);
                }
                catch (SmtpException se)
                {
                    Utils.log(se.ToString());
                }
            }
        }
    }
}