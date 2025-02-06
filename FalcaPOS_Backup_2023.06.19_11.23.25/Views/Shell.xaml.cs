using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Entites.User;
using MahApps.Metro.Controls;
using Prism.Events;
using System;
using System.Diagnostics;
using System.Windows;

namespace FalcaPOS.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {

        private IEventAggregator _eventAggregator;

        public Shell(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            //lblwelcome.Visibility = Visibility.Collapsed;

            //lblsignout.Visibility = Visibility.Collapsed;

            //_eventAggregator.GetEvent<Common.Events.Login>().Subscribe((IsloginedIn) =>
            //{
            //    lblwelcome.Visibility = lblsignout.Visibility = IsloginedIn ? Visibility.Visible : Visibility.Collapsed;
            //});
            _eventAggregator.GetEvent<LoggedInUserEvent>().Subscribe(UserLoggedIn);


        }

        private void UserLoggedIn(object obj)
        {
            if (obj is User _user)
            {
                //lblwelcome.Content = $"Welcome {_user.Name}  {(AppConstants.USER_ROLES.Contains("admin")|| AppConstants.USER_ROLES.Contains("auditor") || AppConstants.USER_ROLES.Contains("finance") || AppConstants.USER_ROLES.Contains("falcadirector")|| AppConstants.USER_ROLES.Contains("purchasemanager")|| AppConstants.USER_ROLES.Contains("territorymanager")|| AppConstants.USER_ROLES.Contains("regionalmanager") ? "" : ("[ " + AppConstants.LoggedInStoreInfo.Name + " ]"))}";
            }
        }

        private void Flyout_ClosingFinished(object sender, RoutedEventArgs e)
        {
            // RaiseEvent(this, EventArgs.Empty);
        }

        private void LauchFalcaSite(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(AppConstants.FALCA_SITE_URL);
            }
            catch (Exception)
            {
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    _eventAggregator.GetEvent<ShowReleseNote>().Publish(true);
        //}
    }
}
