using System;
using System.Reflection;
using System.Windows.Media;

namespace FalcaPOS.Common.Media
{
    public class MediaPlay
    {
        //private FalcaPOS.Common.Logger.Logger _logger;
        public MediaPlay() //FalcaPOS.Common.Logger.Logger logger
        {
            //_logger = logger;
        }
        public void Play(String absolutePath)
        {
            try
            {
                MediaPlayer player = new MediaPlayer();
                player.Open(new System.Uri($"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}" + absolutePath));
                player.Play();
            }
            catch (System.Exception ex)
            {
                // _logger.LogError(ex.Message, ex);

            }
        }
    }
}
