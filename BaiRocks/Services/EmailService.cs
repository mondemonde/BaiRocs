using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Services
{
   public class EmailService
    {
        public bool SendEmail(string strSubject, StringBuilder strBody, bool obMailFormat, String strTo, String strFrom, String strCC = "")
        {
            bool result = false;
            MailMessage mailMsg = new MailMessage();
            try
            {
                mailMsg.From = new MailAddress(strFrom);
                mailMsg.To.Add(strTo);
                if (!String.IsNullOrEmpty(strCC))
                    mailMsg.CC.Add(strCC);
                mailMsg.Subject = strSubject;
                mailMsg.Body = strBody.ToString();
                mailMsg.IsBodyHtml = obMailFormat;
                mailMsg.Priority = MailPriority.High;
                

                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]);
                smtp.Credentials = CredentialCache.DefaultNetworkCredentials;


                #region---TEST ONLY: Compiler will  automatically erase this in RELEASE mode and it will not run if Global.GlobalTestMode is not set to TestMode.Simulation
#if OVERRIDE || OFFLINE
            //_HACK EMAIL DUMP --overide emial part2 put to local directory//////////////////////

            smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            var emailPickupDirectory = @"D:\Dump\Emails";//HostingEnvironment.MapPath("~/EmailPickup");
            if (!Directory.Exists(emailPickupDirectory))
            {
                Directory.CreateDirectory(emailPickupDirectory);
            }
            smtp.PickupDirectoryLocation = emailPickupDirectory;

#endif
                #endregion //////////////END TEST


                smtp.Send(mailMsg);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mailMsg = null;
            }
            return result;
        }

    }
}
