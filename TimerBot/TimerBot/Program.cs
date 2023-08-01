using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using AutoIt;
using CsvHelper;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TimerBot
{
    class Program
    {
        public static BotInput config;

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        static void Main(string[] args)
        {
            config = ReadConfig();
            if (config != null)
            {
                //AutoItX.WinMinimizeAll();

                if (config.KillSelf)
                {
                    DoStuff();
                    Thread.Sleep(1000);
                }
                else
                {
                    var timer = new Timer
                    {
                        Interval = 1000 * /*60 **/ config.Interval,
                        Enabled = true
                    };

                    timer.Elapsed += OnTimedEvent;
                    timer.Start();
                    var key = Console.ReadLine();
                    while (key != "q")
                    {
                        key = Console.ReadLine();
                    }
                }
            }
        }

        public static BotInput ReadConfig()
        {
            try
            {
                var fileBytes = File.ReadAllBytes("config.csv");
                using (var memoryStream = new MemoryStream(fileBytes))
                using (var fileStream = new StreamReader(memoryStream))
                using (var csvReader = new CsvReader(fileStream))
                {
                    csvReader.Configuration.Delimiter = ",";
                    csvReader.Configuration.HeaderValidated = null;
                    csvReader.Configuration.MissingFieldFound = null;
                    var configs = csvReader.GetRecords<BotInput>().ToList();

                    var readConfig = configs.First();
                    if (!readConfig.IsPast && readConfig.EndDate <= readConfig.StartDate)
                    {
                        throw new Exception("End date should be greater than start date");
                    }
                    if (!readConfig.IsPast && readConfig.EndTime <= readConfig.StartTime)
                    {
                        throw new Exception("End time should be greater than start time");
                    }
                    if (readConfig.Interval <= 0)
                    {
                        throw new Exception("Interval value should be greater than zero");
                    }
                    return readConfig;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Beep();
                Console.Beep();
                Console.Beep();
                Console.WriteLine("No or Bad config file");
                return null;
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            DoStuff();
        }

        private static void DoStuff()
        {
            Random rnd = new Random();
            var range = config.EndTime - config.StartTime;

            var randTimeSpan = new TimeSpan((long)(rnd.NextDouble() * range.Ticks));

            var randomTime = config.StartTime + randTimeSpan;

            var dateRange = config.EndDate - config.StartDate;

            var randDate = new TimeSpan((long)(rnd.NextDouble() * dateRange.Ticks));

            var randomDate = config.StartDate + randDate;

            var newDate = randomDate.Add(randomTime);

            SYSTEMTIME st = new SYSTEMTIME
            {
                wYear = (short)newDate.Year,
                wMonth = (short)newDate.Month,
                wDay = (short)newDate.Day,
                wHour = (short)newDate.Hour,
                wMinute = (short)newDate.Minute,
                wSecond = (short)newDate.Second
            };
            // must be short

            SetSystemTime(ref st); // invoke this method.

            if (config.Clicks > 0)
            {
                AutoItX.MouseClick("LEFT", (int)new AppSettingsReader().GetValue("X-Axis", typeof(int)), (int)new AppSettingsReader().GetValue("Y-Axis", typeof(int)), config.Clicks);
            }
        }
    }
}
