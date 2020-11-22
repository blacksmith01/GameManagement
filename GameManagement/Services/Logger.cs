using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GameManagement.Services
{
    public class Logger
    {
        public class Sep // seperator
        {

        }
        static public void Initialize()
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();
            NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
        }

        static public void Debug(string message, Sep s = null, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {
            NLog.LogManager.GetLogger(caller).Debug(message);
        }
        static public void Info(string message, Sep s = null, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {
            NLog.LogManager.GetLogger(caller).Info(message);
        }
        static public void Info(string message, int arg1, Sep s = null, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {
            NLog.LogManager.GetLogger(caller).Info(message, arg1.ToString());
        }
        static public void Info(string message, int arg1, int arg2, Sep s = null, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {
            NLog.LogManager.GetLogger(caller).Info(message, arg1.ToString(), arg2.ToString());
        }
        static public void Error(string message, Sep s = null, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {
            NLog.LogManager.GetLogger(caller).Error(message);
        }
        static public void Error(Exception e, string message, Sep s = null, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {
            NLog.LogManager.GetLogger(caller).Error(e, message);
        }
    }
}
