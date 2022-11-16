using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    internal class DailyNotification
    {
        public DateTime Date { get; private set; }
        public long ChatId { get; private set; }
        public DailyNotification(DateTime datetime, long chatId)
        {
            Date = datetime;
            ChatId = chatId;
        }
    }
}
