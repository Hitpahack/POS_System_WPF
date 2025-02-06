using Prism.Mvvm;
using System;

namespace FalcaPOS.Entites.Notification
{
    public class POSNotification : BindableBase
    {
        string[] colors = new string[] { "#dca995", "#9584ea", "#d49dab", "#5a308b", "#c6394a" };

        private string _color;
        private string _primaryText;
        private string _secondaryText;
        private string _timetext;
        private DateTime _receivedTime;

        public string Color
        {
            get { return colors[new Random().Next(colors.Length)]; }
            set { SetProperty(ref _color, value); }
        }

        public string PrimaryText
        {
            get { return _primaryText; }
            set { SetProperty(ref _primaryText, value); }
        }


        public string SecondaryText
        {
            get { return _secondaryText; }
            set { SetProperty(ref _secondaryText, value); }
        }

        public string Timetext
        {
            get { return _timetext; }
            set { SetProperty(ref _timetext, value); }
        }


        public DateTime ReceivedTime
        {
            get { return _receivedTime; }
            set { SetProperty(ref _receivedTime, value); }
        }


        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

    }
}
