using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCardSync
{

    internal class MysqlCardMembers
    {
        public   int id { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; } 
        public string surname { get; set; }
        public DateTime birthdate { get; set; }
        public string sex { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string preferential { get; set; }
        public string subscribe { get; set; }

    }
}
