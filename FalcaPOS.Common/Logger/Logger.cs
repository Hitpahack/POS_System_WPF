using Serilog;
using System;

namespace FalcaPOS.Common.Logger
{
    public class Logger
    {

        public void LogError(string msg, Exception exception = null)
        {
            try
            {
                Log.Error(exception, msg);
            }
            catch (Exception)
            {
            }
        }

        public void LogInformation(string msg)
        {
            try
            {
                Log.Information(msg);
            }
            catch (Exception)
            {
            }
        }

        public void LogWarning(string msg, Exception exception = null)
        {
            try
            {
                Log.Warning(exception, msg);
            }
            catch (Exception)
            {
            }
        }

        public void LogObject(string msg, object obj)
        {
            try
            {
                Log.Information("{@msg}--{@obj}", msg, obj);
            }
            catch (Exception)
            {
            }
        }
    }
}
