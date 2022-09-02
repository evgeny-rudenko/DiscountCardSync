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
                // connection string sample
                // Server = myServerAddress; Port = 1234; Database = myDataBase; Uid = myUsername; Pwd = myPassword;
                // Efarma2MustDie
                // Server = 192.168.1.82; Port = 3307; Database = backoffice; Uid = root; Pwd = Efarma2MustDie;
                mysqlCards = cnn.Query<MysqlCardMembers>("select * from discount").ToList ();

            }

            
        }



    }
}
