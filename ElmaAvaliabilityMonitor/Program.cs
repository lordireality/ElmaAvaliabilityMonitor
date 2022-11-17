using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Mail;

namespace ElmaAvaliabilityMonitor
{
    //Прога была написана на коленках в 3 часа ночи, так как я устал отлавливать падения системы, для дальнейшего его исправления
    //сорре за плохой код...
    //может быть этот кусок .... кому то пригодится в жизни...

    
    class Program
    {
        static void Main(string[] args)
        {
            var mon = new Monitor();
            while (true)
            {
                string log = mon.GetLog();
                File.AppendText("ElmaLog.txt");
                Console.WriteLine(log);
                Thread.Sleep(15000);
            }
        }


    }
    /// <summary>
    /// Класс самого мониторинга
    /// </summary>
    public class Monitor
    {
        /// <summary>
        /// Получить лог
        /// </summary>
        /// <returns></returns>
        public string GetLog()
        {

            string type = "Log";
            var message = Request();
            switch (message)
            {
                case "Response ok":type = "Log"; break;
                default: type = "Error"; CreateMessage(message); break;
            }
            return string.Format("[{0}][{1}]{2}", type, DateTime.Now.ToString(), message);
        }
        /// <summary>
        /// Создает оповещалку на Email
        /// </summary>
        /// <param name="errCode">текст детализации ошибки</param>
        public void CreateMessage(string errCode)
        {
            
            MailAddress from = new MailAddress("example@gmail.com", "name");
            MailAddress to = new MailAddress("example@gmail.com");
            MailMessage m = new MailMessage(from, to);
            m.Subject = "АЛЯРМ! ОШИБКА НА СТРАНИЦЕ";
            m.Body = "АЛЯРМ! СТРАНИЦА С ВЕБ ПОРТАЛОМ ELMA НЕ ДОСТУПНА!\n"+ errCode;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("example@gmail.com", "mypassword");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }

        /// <summary>
        /// Формирует реквест
        /// </summary>
        /// <returns></returns>
        public string Request()
        {
            var url = "https://127.0.0.1:80"; //elma adress
            using (var webClient = new WebClient())
            {
                try
                {
                    byte[] data;
                    data = webClient.DownloadData(url);
                    return "Response ok";

                }

                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var resp = ex.Response as HttpWebResponse;
                        if (resp != null)
                        {
                            return "Alarm! Page status code - " + resp.StatusCode.ToString();
                        }
                    }
                }
                return "Alarm! Null returned after request!";
            }
        }
    }
}
