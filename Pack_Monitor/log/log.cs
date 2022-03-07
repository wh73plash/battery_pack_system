using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace LGHBAcsEngine {
    /// <summary>
    /// 저장파일은 Log_200099191.log가 됩니다.
    /// </summary>
    public static class TraceManager {
        static bool _useSaveLog = true;
        static public string webRoot = @"C:\ACS\Pack_Monitor\Logs";

        /// <summary>
        /// 로그파일 저장여부 플래그
        /// </summary>
        public static bool UseSaveLog {
            get => _useSaveLog;
            set => _useSaveLog = value;
        }

        static int _traceMaxDay = 30;

        /// <summary>
        /// 보존할 최대일수
        /// </summary>
        public static int TraceMaxDay {
            get => _traceMaxDay;
            set {
                _traceMaxDay = value;
                if (_traceMaxDay <= 0) _traceMaxDay = 1;
            }
        }

        static string _logFolder = "Trace";

        /// <summary>
        /// 저장할 로그 폴더
        /// </summary>
        public static string LogFolder {
            get => _logFolder;
            set => _logFolder = value;
        }

        private static object writerLock = new object( );

        public static void AddLog(string log) {
            log = log.Replace("\n", " - ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log);
            if (!UseSaveLog) return;

            try {
                string folder = Path.Combine(webRoot, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string fileName = folder + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                DateTime dtm = DateTime.Now;
                string formatDateTime = string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00} [{6:000}] ",
                    dtm.Year, dtm.Month, dtm.Day, dtm.Hour, dtm.Minute, dtm.Second, dtm.Millisecond);
                log = formatDateTime + log;

                lock (writerLock) {
                    var sw = File.AppendText(fileName);
                    sw.WriteLine(log);
                    sw.Close( );
                }

                CheckRemoveFiles(folder);
            } catch (Exception ex) {

            }
        }

        public static void AddLog(string log, string strpref) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log);
            if (!UseSaveLog) return;

            try {
                //가저올 파일 이름
                //string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFolder);
                //string folder = Path.Combine(webRoot, LogFolder, DateTime.Now.ToString("yyyyMMdd"));
                string folder = Path.Combine(webRoot, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string fileName = folder + "\\" + strpref + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                DateTime dtm = DateTime.Now;
                string formatDateTime = string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00} [{6:000}] ",
                    dtm.Year, dtm.Month, dtm.Day, dtm.Hour, dtm.Minute, dtm.Second, dtm.Millisecond);
                log = formatDateTime + log;

                var sw = File.AppendText(fileName);
                sw.WriteLine(log);
                sw.Close( );

                CheckRemoveFiles(folder);
            } catch (Exception ex) {

            }
        }

        static void CheckRemoveFiles(string folder) {
            int maxHour = TraceMaxDay * 24; //

            string[] files = Directory.GetFiles(folder);
            for (int i = 0; i < files.Length; i++) {
                DateTime tm = File.GetLastAccessTime(files[i]);
                TimeSpan span = DateTime.Now - tm;
                if (span.TotalHours >= maxHour) {
                    File.Delete(files[i]);
                }
            }
        }
    }
}
