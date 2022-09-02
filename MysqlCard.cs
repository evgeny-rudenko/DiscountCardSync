using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;
using System.IO;

namespace DiscountCardSync
{
    internal class MysqlCards
    {
        public List<MysqlCardMembers> mysqlCards;
        
        public  MysqlCards ()
        {
            mysqlCards = new List<MysqlCardMembers> ();
            string MysqlConnectionString = File.ReadAllText("mysql.txt"); 
            using (MySqlConnection cnn = new MySqlConnection(MysqlConnectionString))
            {
             
                mysqlCards = cnn.Query<MysqlCardMembers>("select * from discount").ToList ();

            }

            
        }



    }
}
