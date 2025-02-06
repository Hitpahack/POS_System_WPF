using FalcaPOS.Common.Events;
using FalcaPOS.Contracts;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows;

namespace FalcaPOS.Shell.ViewModels
{
    public class GSTCalculatorViewModel : FlyoutBaseViewModel
    {
        private IEventAggregator _eventAggregator;
        private INotificationService _notificationService;

        public DelegateCommand<object> GSTCheckedCommand { get; private set; }
        public DelegateCommand<object> GSTTexChangeCommand { get; private set; }

        public DelegateCommand ResetGSTCalculatorCommand { get; private set; }

        private bool _IsfiveChecked;
        private bool _IstweleveChecked;
        private bool _IseighteenChecked;
        private bool _IstwentyeightChecked;

        public bool IsfiveChecked
        {
            get { return _IsfiveChecked; }
            set { SetProperty(ref _IsfiveChecked, value); }
        }
        public bool IstweleveChecked
        {
            get { return _IstweleveChecked; }
            set { SetProperty(ref _IstweleveChecked, value); }
        }

        public bool IseighteenChecked
        {
            get { return _IseighteenChecked; }
            set { SetProperty(ref _IseighteenChecked, value); }
        }

        public bool IstwentyeightChecked
        {
            get { return _IstwentyeightChecked; }
            set { SetProperty(ref _IstwentyeightChecked, value); }
        }


        private float _includingGST;

        public float IncludingGST
        {
            get { return _includingGST; }
            set { SetProperty(ref _includingGST, value); }
        }

        private float _excludingGST;

        public float ExcludingGST
        {
            get { return _excludingGST; }
            set { SetProperty(ref _excludingGST, value); }
        }


        private float _cgst;
        public float CGST
        {
            get { return _cgst; }
            set { SetProperty(ref _cgst, value); }
        }


        private float _sgst;
        public float SGST
        {
            get { return _sgst; }
            set { SetProperty(ref _sgst, value); }
        }

        private float _igst;
        public float IGST
        {
            get { return _igst; }
            set { SetProperty(ref _igst, value); }
        }


        private float _totalGST;
        public float TotalGST
        {
            get { return _totalGST; }
            set { SetProperty(ref _totalGST, value); }
        }

        public GSTCalculatorViewModel(IEventAggregator eventAggregator, INotificationService notificationService)
        {
            Header = "GST Calculator";
            Position = MahApps.Metro.Controls.Position.Right;
            Width = 400;
            Height = GridLength.Auto;
            _eventAggregator = eventAggregator;
            _notificationService = notificationService;
            _eventAggregator.GetEvent<ShowGSTCalculator>().Subscribe((isopen) => { IsOpen = isopen; });
            GSTCheckedCommand = new DelegateCommand<object>(GSTCheckedCommandEvent);
            GSTTexChangeCommand = new DelegateCommand<object>(GSTExcludeCommandEvent);
            ResetGSTCalculatorCommand = new DelegateCommand(ResetGSTCalculator);

        }

        /// <summary>
        /// This method is used to reset all the values in the GST calculator.
        /// </summary>
        private void ResetGSTCalculator()
        {
            TotalGST = 0;
            IGST = 0;
            SGST = 0;
            CGST = 0;
            ExcludingGST = 0;
            IncludingGST = 0;
            IsfiveChecked = IseighteenChecked = IstweleveChecked = IstwentyeightChecked =  false;
        }

        private void GSTExcludeCommandEvent(object obj)
        {
            try
            {
                Enum.TryParse(obj.ToString(), out GSTType gSTType);
                float selectedSlab = IsfiveChecked ? 5 :
                                          IstweleveChecked ? 12 :
                                          IseighteenChecked ? 18 :
                                          IstwentyeightChecked ? 28 : 0;
                if (selectedSlab == 0)
                {
                    MessageBox.Show("Select GST Slab");
                    return;
                }

                selectedSlab = (float)(selectedSlab * 0.01);
                if (gSTType == GSTType.Include)
                {

                    if (IncludingGST > 0)
                    {
                        TotalGST = (float)Math.Round((IncludingGST - IncludingGST / (1 + (selectedSlab))),5,  MidpointRounding.AwayFromZero);
                        ExcludingGST = (IncludingGST - TotalGST);
                        CGST = TotalGST / 2;
                        SGST = TotalGST / 2;
                        IGST = TotalGST;
                    }

                }
                else if (gSTType == GSTType.Exclude)
                {
                    if (ExcludingGST > 0)
                    {
                        TotalGST = (float)Math.Round((ExcludingGST * (selectedSlab)), 5, MidpointRounding.AwayFromZero);
                        IncludingGST = ExcludingGST + TotalGST;
                        CGST = TotalGST / 2;
                        SGST = TotalGST / 2;
                        IGST = TotalGST;
                    }

                }
                if (IncludingGST == 0 && ExcludingGST == 0)
                    CGST = SGST = IGST = TotalGST = 0;
            }
            catch (System.FormatException)
            {
                _notificationService.ShowMessage("Invalid number", Common.NotificationType.Error);
                return;
            }
            catch (Exception)
            {
                _notificationService.ShowMessage("Some error occurred", Common.NotificationType.Error);
                return;

            }
        }

        private void GSTCheckedCommandEvent(object obj)
        {
            try
            {
                if (obj is String)
                {
                    var Selection = Convert.ToString(obj);

                    switch (Selection)
                    {
                        case "5%":
                            IstweleveChecked = false;
                            IseighteenChecked = false;
                            IstwentyeightChecked = false;
                            break;
                        case "12%":
                            IsfiveChecked = false;
                            IseighteenChecked = false;
                            IstwentyeightChecked = false;

                            break;
                        case "18%":
                            IsfiveChecked = false;
                            IstweleveChecked = false;
                            IstwentyeightChecked = false;
                            break;
                        case "28%":
                            IsfiveChecked = false;
                            IstweleveChecked = false;
                            IseighteenChecked = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public enum GSTType
        {
            Include,
            Exclude
        }

    }


}
