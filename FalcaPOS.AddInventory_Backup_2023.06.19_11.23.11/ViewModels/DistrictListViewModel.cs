using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class DistrictListViewModel : BindableBase
    {
        private readonly ICommonService _locationService;

        public DelegateCommand AddNewDistrictCommand { get; private set; }

        public DelegateCommand<object> EditDistrictCommand { get; private set; }

        private readonly IDialogService _dialogService;


        private readonly Logger _logger;

        public DistrictListViewModel(ICommonService locationService, Logger logger, IDialogService dialogService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));


            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            AddNewDistrictCommand = new DelegateCommand(AddDistrcit);


            EditDistrictCommand = new DelegateCommand<object>(EditDistrict);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            LoadDistrictsAsync();

        }

        private void EditDistrict(object obj)
        {
            if (obj is District _dis)
            {
                var _params = new DialogParameters()
                                {
                                        {"isedit",true },
                                        {"title",$"Edit {_dis.Name} district"},
                                        {"name",_dis.Name },
                                        {"shortname",_dis.Shortname },
                                        {"stateId",_dis.StateId },
                                        {"districtId",_dis.DistrictId }
                                };

                _dialogService.ShowDialog(_districtDialog, _params, HandleCallBack);
            }
        }

        private const string _districtDialog = "DistrictDialog";

        private void AddDistrcit()
        {
            var _params = new DialogParameters()
            {
                {"isedit",false },
                { "title","Add new district"}
            };

            _dialogService.ShowDialog(_districtDialog, _params, HandleCallBack);
        }

        private void HandleCallBack(IDialogResult res)
        {
            if (res != null && res.Result == ButtonResult.OK)
            {
                var _updateDistrict = res.Parameters.GetValue<District>("district");

                if (_updateDistrict == null) return;

                var _existDistrict = Districts.FirstOrDefault(x => x.DistrictId == _updateDistrict.DistrictId);

                if (_existDistrict != null)
                {
                    Districts.Remove(_existDistrict);
                }
                Districts.Add(_updateDistrict);
                Districts = new ObservableCollection<District>(Districts.OrderBy(x => x.Name));
            }
        }

        private ObservableCollection<District> _districts;

        public ObservableCollection<District> Districts
        {
            get { return _districts; }
            set { SetProperty(ref _districts, value); }
        }


        private async void LoadDistrictsAsync()
        {
            try
            {
                await Task.Run(async () =>
                {

                    var _result = await _locationService.GetAllDistricts();

                    if (_result != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {

                            Districts = new ObservableCollection<District>(_result.OrderBy(x => x.Name));

                        });
                    }
                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in district dialog view model", _ex);
            }
        }
    }
}
