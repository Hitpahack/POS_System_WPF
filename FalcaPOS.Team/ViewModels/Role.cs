using Prism.Mvvm;

namespace FalcaPOS.Team.ViewModels
{
    public class Role : BindableBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _autoMationId;
        public string AutoMationId
        {
            get { return _autoMationId; }
            set { SetProperty(ref _autoMationId, value); }
        }

        private bool _isEnableRole;
        public bool IsEnableRole
        {
            get => _isEnableRole;
            set => SetProperty(ref _isEnableRole, value);
        }

    }
}
