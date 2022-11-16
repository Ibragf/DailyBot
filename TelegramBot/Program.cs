using System;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Telegram.Bot;

namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Бот запущен");
            DailyBot bot = DailyBot.GetInstance();
            bot.Start();
            Console.ReadLine();        
        }
    }
}