using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{

    enum DailyType
    {
        CreateNotification
    }
    internal class DailyBot
    {
        private static DailyBot dailybot;
        private static ITelegramBotClient bot = new TelegramBotClient("5778874219:AAHiLbt4BjyXVh15BX6QZAzIrUmJ9-SiPdY");
        private static ReplyKeyboardMarkup rkm;

        private string regexPattern = @"[0-9]{2}.[0-9]{2}.[1-9][0-9]{3} (([0-1][0-9])|(2[0-3])):[0-5][0-9]";
        private List<DailyNotification> notificationList = new List<DailyNotification>();

        private ReceiverOptions options = new ReceiverOptions
        {
            AllowedUpdates = { }
        };

        private DailyBot()
        {
            
        }

        public static DailyBot GetInstance()
        {
            if(dailybot == null)
            {
                dailybot = new DailyBot();
                KeyboardButton[] keyboards = new KeyboardButton[]
                {
                    new KeyboardButton(DailyType.CreateNotification.ToString())
                };
                rkm = new ReplyKeyboardMarkup(keyboards);
            }
            return dailybot;
        }


        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cstoken)
        {
            if(update.Type==UpdateType.Message)
            {
                var message = update.Message;
                if(message.Text.ToUpperInvariant()=="/START")
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Выберите команду", null,null,null,null,null,null,null,rkm, cstoken);
                }

                if(message.Text.Equals(DailyType.CreateNotification.ToString(),StringComparison.InvariantCultureIgnoreCase))
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Отправьте дату в формате 01.01.1971 00:00");
                }

                if(Regex.IsMatch(message.Text, regexPattern))
                {
                    string[] values = message.Text.Split(' ');
                    string[] dmy = values[0].Split('.');
                    string[] minsec=values[1].Split(':');

                    int[] intDmy =
                    {
                        int.Parse(dmy[0]),
                        int.Parse(dmy[1]),
                        int.Parse(dmy[2])
                    };
                    int[] intms =
                    {
                        int.Parse(minsec[0]),
                        int.Parse(minsec[1])
                    };

                    DateTime date=new DateTime(intDmy[2],intDmy[1],intDmy[0], intms[0], intms[1],0);
                    Console.WriteLine(date.ToShortDateString());
                    DailyNotification notifiction = new DailyNotification(date, message.Chat.Id);
                    notificationList.Add(notifiction);
                }
            }
        }

        private async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cstoken)
        {
            Console.WriteLine(exception.Message);
        }

        private async Task CheckNotificationsAsync()
        {
            TimeSpan interval = new TimeSpan(0, 1, 0);
            TimeSpan comparison = new TimeSpan(0, 16, 0);
            while(true)
            {
                foreach (var item in notificationList)
                {
                    if(DateTime.Now.Subtract(item.Date)<comparison)
                    {
                        bot.SendTextMessageAsync(item.ChatId, $"Осталось {DateTime.Now.Subtract(item.Date).Minutes} минут до чего-то");
                        notificationList.Remove(item);
                    }
                }


                Thread.Sleep(interval);
            }
        }

        public void Start()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, options, token);
            CheckNotificationsAsync();
        }
    }
}
