using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RmrHelper.Helpers
{
    public static class Logger
    {
        static readonly object _locker = new object();

        static string LogFilePath
        {
            get { return Path.ChangeExtension(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "RmrHelper_log"), "log"); }
        }

        static string LaunchGuid;




        public static void Clear()
        {
            try
            {
                LaunchGuid = Guid.NewGuid().ToString();
                if (File.Exists(LogFilePath))
                {
                    File.Delete(LogFilePath);
                }
            }
            catch { }
        }




        public static void Log(string message, string level = "INFO")
        {
            try
            {
                Log(new List<string> { message }, level);
            }
            catch { }
        }

        public static void Log(Exception ex, string level = "ERROR")
        {
            try
            {
                var messages = new List<string>();
                while (ex != null)
                {
                    messages.Add(ex.GetType().Name);
                    messages.Add(ex.Message);
                    messages.Add(JsonConvert.SerializeObject(ex, new JsonSerializerSettings { ReferenceLoopHandling= ReferenceLoopHandling.Ignore }));
                    messages.Add(ex.StackTrace);
                    ex = ex.InnerException;
                }
                Log(messages, level);
            }
            catch { }
        }

        public static void Log(List<string> messages, string level = "INFO")
        {
            try
            {
                WriteToLog(messages, level);
            }
            catch { }
        }




        static void WriteToLog(List<string> messages, string level = "INFO")
        {
            var now = DateTime.Now.ToString(@"yyyy-MM-dd HH\:mm\:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            var guid = Guid.NewGuid().ToString();
            lock (_locker)
            {
                foreach (var message in messages)
                {
                    File.AppendAllText(LogFilePath, $"{now}\t{LaunchGuid}\t{guid}\t{level}\t{message}{Environment.NewLine}");
                }
            }
        }
    }
}
