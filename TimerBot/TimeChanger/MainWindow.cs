using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TimeChanger
{
    public partial class MainWindow : Form
    {
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

        public static int intervalDays = 7;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void backBtn_Click(object sender, EventArgs e)
        {
            backBtn.Enabled = false;
            var orignalText = backBtn.Text;
            backBtn.Text = @"Working...";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://worldclockapi.com/api/json/utc/now"));
            var response = await client.SendAsync(request);
            var responseObj = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            DateTime date = DateTime.SpecifyKind(DateTime.FromFileTimeUtc((long)responseObj.currentFileTime), DateTimeKind.Utc).AddDays(-1 * intervalDays);
            var st = new SYSTEMTIME
            {
                wYear = (short)date.Year,
                wMonth = (short)date.Month,
                wDay = (short)date.Day,
                wHour = (short)date.Hour, //== 0 ? (short)12 : (short)date.Hour,
                wMinute = (short)date.Minute,
                wSecond = (short)date.Second,
                wDayOfWeek = (short)date.DayOfWeek
            };

            SetSystemTime(ref st); // invoke this method.

            backBtn.Enabled = true;
            backBtn.Text = orignalText;
            intervalDays += 7;
        }

        private async void forwardBtn_Click(object sender, EventArgs e)
        {

            resetBtn.Enabled = false;
            var orignalText = resetBtn.Text;
            resetBtn.Text = @"Working...";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://worldclockapi.com/api/json/utc/now"));
            var response = await client.SendAsync(request);
            var responseObj = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            DateTime date = DateTime.SpecifyKind(DateTime.FromFileTimeUtc((long)responseObj.currentFileTime), DateTimeKind.Utc);
            //var date = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, timeZoneInfo);
            var st = new SYSTEMTIME
            {
                wYear = (short)date.Year,
                wMonth = (short)date.Month,
                wDay = (short)date.Day,
                wHour = (short)date.Hour, //== 0 ? (short)12 : (short)date.Hour,
                wMinute = (short)date.Minute,
                wSecond = (short)date.Second,
                wDayOfWeek = (short)date.DayOfWeek
            };

            SetSystemTime(ref st); // invoke this method.
            intervalDays = 7;
            resetBtn.Enabled = true;
            resetBtn.Text = orignalText;
        }
    }
}
