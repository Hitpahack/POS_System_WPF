using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace FalcaPOS.Views
{
    /// <summary>
    /// Interaction logic for ReleseNotesWhenUpdate.xaml
    /// </summary>
    public partial class ReleseNotesWhenUpdate : MetroWindow
    {
        public delegate void OnReleseNoteClosed(Object sender, EventArgs e);

        public event OnReleseNoteClosed IsOk;

        protected virtual void OnIsOk()
        {
            if (IsOk != null)
            {
                IsOk(this, new EventArgs());
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnIsOk();
        }
        public ReleseNotesWhenUpdate()
        {
            InitializeComponent();
        }


    }
}
