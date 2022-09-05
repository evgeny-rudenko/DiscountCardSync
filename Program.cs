using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiscountCardSync
{
    internal class Program
    {

        /// <summary>
        /// Ловим ошибки при выполнении программы. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            //Console.WriteLine(e.ExceptionObject.ToString());
            // Console.WriteLine("Press Enter to continue");
            // Console.ReadLine();

            // Instantiate the class
            var logger = new SimpleLogger(); // Will create a fresh new log file if it doesn't exist.
                                             // To log Trace message
                                             //logger.Trace("--> Trace in message here...");
                                             // To log Info message
                                             //logger.Info("Anything to info here...");
                                             // To log Debug message
                                             //logger.Debug("Something to debug...");
                                             // To log Warning message
                                             //logger.Warning("Anything to put as a warning log...");
                                             // To log Error message
                                             //logger.Error("Error message...");
                                             // To log Fatal error message


            Exception ee = (Exception)e.ExceptionObject;
            logger.Fatal(e.ExceptionObject.ToString());


            //ravenClient.Capture(new SharpRaven.Data.SentryEvent(ee));

            Environment.Exit(1);
        }


        static void Main(string[] args)
        {
            // на ошибках не падаем, а логируем и выходим
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            var logger = new SimpleLogger();
            logger.Info("Запуск синхронизации");

            MysqlCards mysqlCards = new MysqlCards();
            F3TailCardMembers f3TailCardMembers = new F3TailCardMembers();

            logger.Info("Получаем данные из MSSQL");
            List<string> f3TailPhones = new List<string>();
            logger.Info("Получаем данные из MySQL");
            List<string> mysqlPhones = new List<string>();

            /// список телефонов из внешней базы MYSQL
            foreach (MysqlCardMembers msqlm in mysqlCards.mysqlCards)
            {
                mysqlPhones.Add("7"+ msqlm.phone);
            }

           


            foreach (MysqlCardMembers msqlm in mysqlCards.mysqlCards)
            {
                string phone ="7"+ msqlm.phone;
                if (!f3TailCardMembers.phoneNumbers.Contains(phone))
                {
                    logger.Info("Добавили телефон " + msqlm.surname +" "+ msqlm.name +" "+ msqlm.patronymic + " "+ phone);
                    f3TailCardMembers.InsertNewCard(msqlm.surname, msqlm.name, msqlm.patronymic, phone);
                }
            }

            f3TailCardMembers.LinkCards();

            logger.Info("Синхронизация завершена");
        }

        static string NormalizePhone (string phone)
        {
            string result="";

            // regexp (^\+\d{1,2})?((\(\d{3}\))|(\-?\d{3}\-)|(\d{3}))((\d{3}\-\d{4})|(\d{3}\-\d\d\-\d\d)| (\d{ 7})| (\d{ 3}\-\d\-\d{ 3}))  
            string phoneNumber =phone;
            string pattern = @"(^\+\d{1,2})?((\(\d{3}\))|(\-?\d{3}\-)|(\d{3}))((\d{3}\-\d{4})|(\d{3}\-\d\d\-\d\d)| (\d{ 7})| (\d{ 3}\-\d\-\d{ 3}))  ";
            string target = "";
            Regex regex = new Regex(pattern);
            result = regex.Replace(phoneNumber, target);
            Console.WriteLine(result);  // 18762341298
            return result;
        }
    }
}
