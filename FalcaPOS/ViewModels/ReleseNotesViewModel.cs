using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FalcaPOS.Shell.ViewModels
{
    public class ReleseNotesViewModel : FlyoutBaseViewModel
    {
        private string _title = "Hello from flyout";

        private IEventAggregator _eventAggregator;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<VersionInfo> _versionHistory;
        public ObservableCollection<VersionInfo> VersionHistory
        {
            get { return _versionHistory; }
            set { SetProperty(ref _versionHistory, value); }
        }

        private VersionInfo _Version;

        public VersionInfo Version
        {
            get { return _Version; }
            set { SetProperty(ref _Version, value); }
        }

        public ReleseNotesViewModel(IEventAggregator eventAggregator)
        {
            Header = "Release Notes";
            Position = MahApps.Metro.Controls.Position.Left;
            Width = 400;
            Height = GridLength.Auto;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ShowReleseNote>().Subscribe((isopen) => { IsOpen = isopen; });

            LoadVersion();

        }


        public void LoadVersion()
        {

            var _versionInfo = new ParseJSON().ParseJson<List<VersionInfo>>("VersionInfo");
            if (_versionInfo != null)
            {
                _versionInfo.Where(x => x.Comments.Contains("@@"))
                    .Select(x =>
                    {
                        x.Comments = x.Comments.Replace("@@", Environment.NewLine);
                        return x;
                    })
                        .ToList();
                _versionInfo.Reverse();
                VersionHistory = new ObservableCollection<VersionInfo>(_versionInfo);
                Version = _versionInfo.FirstOrDefault();
            }
        }

    }


    public class VersionInfo
    {
        public string No { get; set; }
        public string Comments { get; set; }
        public string OnDate { get; set; }
    }
}
