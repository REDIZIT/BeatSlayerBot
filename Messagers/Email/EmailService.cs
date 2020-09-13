using BeatSlayerServer.Models.Configuration;
using BeatSlayerServer.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace BeatSlayerServer.Utils.Email
{
    public class EmailService
    {
        public bool IsServiceAlive => client != null;
        public bool IsServiceEnabled => settings.Email.IsEnabled;

        public SmtpClient client;


        private readonly ServerSettings settings;
        private readonly ILogger<EmailService> logger;

        public EmailService(SettingsWrapper wrapper, ILogger<EmailService> logger)
        {
            settings = wrapper.settings;
            this.logger = logger;

            Build();
        }
        public void Build()
        {
            string fromEmail = "redizitgamestudios@gmail.com";
            string fromPassword = "GR0155YXZ9";

            //client = new SmtpClient("smtp.gmail.com", 587)
            //{
            //    Credentials = new NetworkCredential(fromEmail, fromPassword),
            //    EnableSsl = true
            //};

            MailMessage mail = new MailMessage();
            //mail.From = new MailAddress("iv24032004@yandex.ru"); // Адрес отправителя
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(new MailAddress("iv24032004@gmail.com")); // Адрес получателя
            mail.Subject = "Заголовок";
            mail.Body = "Письмо.....";

            client = new SmtpClient();
            //client.Host = "smtp.yandex.ru";
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            //client.Credentials = new NetworkCredential("iv24032004@yandex.ru", "GR0155yxz0"); // Ваши логин и пароль
            client.Credentials = new NetworkCredential(fromEmail, fromPassword); // Ваши логин и пароль
            client.Send(mail);
        }
        public void Kill()
        {
            client.Dispose();
            client = null;
        }

        private void SendMapTemplate(string email, string mapper, string trackname, string moderator, string comment, string status)
        {
            MailMessage msg = new MailMessage(settings.Email.Login,  email);
            msg.Subject = $"Your map was {status}!";


            msg.Body = File.ReadAllText(settings.Email.Template_Map);
            msg.IsBodyHtml = true;

            msg.Body = msg.Body.Replace("%mapper%", mapper);
            msg.Body = msg.Body.Replace("%trackname%", trackname);
            msg.Body = msg.Body.Replace("%moderator%", moderator);
            msg.Body = msg.Body.Replace("%comment%", comment);
            msg.Body = msg.Body.Replace("%status%", status);

            client.Send(msg);
        }
        private void SendCodeTemplate(string email, string nick, string code, string action)
        {
            MailMessage msg = new MailMessage(settings.Email.Login, email);
            msg.Subject = "Beat Slayer " + action;


            msg.Body = File.ReadAllText(settings.Email.Template_Code);
            msg.IsBodyHtml = true;

            msg.Body = msg.Body.Replace("%nick%", nick);
            msg.Body = msg.Body.Replace("%action%", action);
            msg.Body = msg.Body.Replace("%code%", code);

            client.Send(msg);
        }



        public void SendRestorePasswordCode(string nick, string email, string code)
        {
            Console.WriteLine("Send code to " + email);
            SendCodeTemplate(email, nick, code, "password restore");
        }
        public void SendEmailChangeCode(string nick, string email, string code)
        {
            Console.WriteLine("Send email change code to " + email + " " + code);
            SendCodeTemplate(email, nick, code, "email changing");
        }




        public void SendApprove(string nick, string email, string trackname, string moderator, string reason)
        {
            Console.WriteLine("Send email approve " + trackname);
            SendMapTemplate(email, nick, trackname, moderator, reason, "approved");
        }
        public void SendReject(string nick, string email, string trackname, string moderator, string reason)
        {
            Console.WriteLine("Send email reject " + trackname);
            SendMapTemplate(email, nick, trackname, moderator, reason, "rejected");
        }
        public void SendUpload(string nick, string email, string trackname)
        {
            Console.WriteLine("Send email upload (DISABLED) " + trackname);
        }
    }
}
