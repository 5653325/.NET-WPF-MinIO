using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MinIO
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static NLog.Logger NewNLog;
        public static long MinimumPartSize = 5 * 1024L * 1024L;//单次上传文件请求最大5MB

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            NewNLog = NLog.LogManager.GetLogger("MinIOLoger");
        }
    }
}
