using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


using System.Text.RegularExpressions;

namespace DiscountCardSync
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //string r = NormalizePhone("+7-909-804-05-05");
            //Console.WriteLine(r);
            
            
            
            MysqlCards mysqlCards = new MysqlCards();
            F3TailCardMembers f3TailCardMembers = new F3TailCardMembers();

            List<string> f3TailPhones = new List<string>();
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
                    f3TailCardMembers.InsertNewCard(msqlm.surname, msqlm.name, msqlm.patronymic, phone);
                }
            }

            f3TailCardMembers.LinkCards();
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
