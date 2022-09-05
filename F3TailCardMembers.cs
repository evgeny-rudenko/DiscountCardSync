using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.IO;

namespace DiscountCardSync
{
    class F3tailCardMember
    {
        public string id_discount2_member_global { get; set; } // тут GUID
        public string lastname { get; set; }
        public string firstname { get; set; }  
        public string middlename { get; set; } 
        public string phone { get; set; }  

    }


    
    internal class F3TailCardMembers
    {
        public List<F3tailCardMember> f3TailCardMembers { get; set; }
        public List<string> emptyCards { get; set; }

        public List<string> phoneNumbers = new List<string>();
        private string f3TailConnectionString = File.ReadAllText("mssql.txt"); 
               

        public F3TailCardMembers ()
        {
            f3TailCardMembers = new List<F3tailCardMember> ();
            getEmptyCards();
            getPhoneNumbers();
        }
        
        /// <summary>
        /// Список пустых карт без накоплений м привязки к физ лицу 
        /// каждый раз выбираются 1000 случайных 
        /// </summary>
        public void getEmptyCards ()
        {
            string sqlEmptyCards = @"select top 1000
convert(nvarchar(36),ID_DISCOUNT2_CARD_GLOBAL) as ID_DISCOUNT2_CARD_GLOBAL 
from DISCOUNT2_CARD dc 
where isnull ( ACCUMULATE_SUM ,0) =0
and ID_DISCOUNT2_MEMBER_GLOBAL is NULL 
order by newid ()";

            emptyCards = new List<string>();
            using (SqlConnection cnn = new SqlConnection (f3TailConnectionString))
            {
                emptyCards = cnn.Query<string>(sqlEmptyCards).ToList();
            }

        
        }
        
        /// <summary>
        /// Получаем список номеров телефонов зарегистрированных в F3Tail
        /// в mysql нет даты изменения / заполнения
        /// </summary>
        public void getPhoneNumbers ()
        {
            string sqlPhoneNumbers = "select PHONE from DISCOUNT2_MEMBER dm";
            using (SqlConnection cnn = new SqlConnection (f3TailConnectionString))
            {
                phoneNumbers = cnn.Query<string>(sqlPhoneNumbers).ToList();
            }
        }


        /// <summary>
        /// Заводим в базу незарегистрированные номера телефонов
        /// </summary>
        /// <param name="lastname">Фамилия</param>
        /// <param name="firstname">Имя</param>
        /// <param name="middlename">Отчество</param>
        /// <param name="phone">Телефон</param>
        public void InsertNewCard  (string lastname,string firstname , string middlename, string phone )
        {
            string sqlInsertPhone = @"INSERT INTO [dbo].[DISCOUNT2_MEMBER]
           ([ID_DISCOUNT2_MEMBER_GLOBAL]
           ,[LASTNAME]
           ,[FIRSTNAME]
           ,[MIDDLENAME]
           ,[DATE_MODIFIED]
           ,[PHONE]
          )
     VALUES
           (newid ()
           ,@lastname
           ,@firstname
           ,@middlename
           ,getdate()
           ,@phone
           )";
            using (SqlConnection cnn = new SqlConnection(f3TailConnectionString))
            {
                var result = cnn.Execute(sqlInsertPhone, new
                {
                    lastname,
                    firstname,
                    middlename,
                    phone                    
                });
            }
        }



        /// <summary>
        /// Привязка учатников, которых мы загрузили, но еще не привязали
        /// </summary>
        public void LinkCards ()
        {
            
            string sqlNotLinkedMembers = @"SELECT convert(nvarchar(36),ID_DISCOUNT2_MEMBER_GLOBAL) as ID_DISCOUNT2_MEMBER_GLOBAL FROM DISCOUNT2_MEMBER 
where DISCOUNT2_MEMBER.ID_DISCOUNT2_MEMBER_GLOBAL  not in 
(
select ID_DISCOUNT2_MEMBER_GLOBAL  from DISCOUNT2_CARD dc where ID_DISCOUNT2_MEMBER_GLOBAL  is not NULL 
)";
            List<string> notLinkedMembers = new List<string>();
            
            // список участников, которых мы еще не привязали 
            using (SqlConnection cnn = new SqlConnection(f3TailConnectionString))
            {


                int listIndex = 0;

                notLinkedMembers = cnn.Query<string>(sqlNotLinkedMembers).ToList();


                foreach (string member_id in notLinkedMembers)
                {
                    listIndex++;
                    string card_id = emptyCards[listIndex];
                    string updateQuery = @"update DISCOUNT2_CARD 
SET ID_DISCOUNT2_MEMBER_GLOBAL = @member_id , DATE_MODIFIED = GETDATE() 
WHERE ID_DISCOUNT2_CARD_GLOBAL = @card_id
";
                    Console.WriteLine($"Карта {card_id} участник скидок {member_id} ");
                    var result = cnn.Execute(updateQuery, new
                    {
                        card_id,
                        member_id
                                                
                    });
                }
            }

            




        }

       



    }
}




/*
 
INSERT INTO [dbo].[DISCOUNT2_MEMBER]
           ([ID_DISCOUNT2_MEMBER_GLOBAL]
           ,[LASTNAME]
           ,[FIRSTNAME]
           ,[MIDDLENAME]
           ,[GENDER]
           ,[BIRTHDAY]
           ,[ADDRESS]
           ,[DATE_DELETED]
           ,[DATE_MODIFIED]
           ,[BIRTHDAY_WITHOUT_YEAR]
           ,[SNILS]
           ,[PHONE]
           ,[EMAIL])
     VALUES
           (<ID_DISCOUNT2_MEMBER_GLOBAL, uniqueidentifier,>
           ,<LASTNAME, varchar(40),>
           ,<FIRSTNAME, varchar(40),>
           ,<MIDDLENAME, varchar(40),>
           ,<GENDER, varchar(1),>
           ,<BIRTHDAY, datetime,>
           ,<ADDRESS, varchar(1024),>
           ,<DATE_DELETED, datetime,>
           ,<DATE_MODIFIED, datetime,>
           ,<BIRTHDAY_WITHOUT_YEAR, datetime,>
           ,<SNILS, varchar(14),>
           ,<PHONE, nvarchar(28),>
           ,<EMAIL, nvarchar(60),>)

 */