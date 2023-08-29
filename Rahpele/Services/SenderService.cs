using Rahpele.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace Rahpele.Services
{
    public class SenderService : ISenderService
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            MailAddress to = new MailAddress(email);
            MailAddress from = new MailAddress("grouplancing@hotmail.com");
            MailMessage emailMess = new MailMessage(from, to);
            emailMess.Subject = subject;
            emailMess.Body = message;
            emailMess.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp-mail.outlook.com";
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("grouplancing@hotmail.com", "053b*u#5LX!K");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            try
            {
                smtp.Send(emailMess);
                return true;
            }
            catch (SmtpException ex)
            {
                return false;
            }
        }


        public bool SendSMS(string toNumber, string userName, string verificationCode, int patternStatus)
        {
            if (toNumber != null)
            {
                string user = userName.Replace(" ", "_");
                string apiKey = "v0XM89Z6bx_xls61OT0tscu6-6Om_02x2OEdoI4J7zk=";
                string fromNumber = "0983000505";
                string patternid = "";
                string url = "";

                if (patternStatus == 1) // ریجستر
                {
                    patternid = "75r5fo7l7u";
                    url = $"http://ippanel.com:8080/?apikey={apiKey}&pid={patternid}&fnum={fromNumber}&tnum={toNumber}&p1=name&p2=verification-code&v1={user}&v2={verificationCode}";
                }
                else if (patternStatus == 2) // فراموشی رمز عبور
                {
                    patternid = "cj0jdm47kltfy8f";
                    url = $"http://ippanel.com:8080/?apikey={apiKey}&pid={patternid}&fnum={fromNumber}&tnum={toNumber}&p1=code&v1={verificationCode}";
                }
                else
                {
                    // Invalid patternStatus, return false
                    return false;
                }

                HttpClient httpClient = new HttpClient();
                var httpResponse = httpClient.GetAsync(url).Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    // SMS sent successfully
                    return true;
                }
            }

            // SMS sending failed
            return false;
        }


        public async Task<bool> SendSMSAsync(string toNumber, string userName, string verificationCode, int patternStatus)
        {
            if (toNumber == null)
            {
                return false;
            }

            string user = userName.Replace(" ", "_");
            string apiKey = "v0XM89Z6bx_xls61OT0tscu6-6Om_02x2OEdoI4J7zk=";
            string fromNumber = "0983000505";
            string patternid = "";
            string url = "";

            if (patternStatus == 1) // ریجستر
            {
                patternid = "75r5fo7l7u";
                url = $"http://ippanel.com:8080/?apikey={apiKey}&pid={patternid}&fnum={fromNumber}&tnum={toNumber}&p1=name&p2=verification-code&v1={user}&v2={verificationCode}";
            }
            else if (patternStatus == 2) // فراموشی رمز عبور
            {
                patternid = "cj0jdm47kltfy8f";
                url = $"http://ippanel.com:8080/?apikey={apiKey}&pid={patternid}&fnum={fromNumber}&tnum={toNumber}&p1=code&v1={verificationCode}";
            }
            else
            {
                return false;
            }

            using (HttpClient httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(url);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
