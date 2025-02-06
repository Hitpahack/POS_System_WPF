using FalcaPOS.Common.Events;
using MahApps.Metro.Controls;
using Prism.Events;
using Prism.Mvvm;

namespace FalcaPOS.Indent.ViewModels
{
    public class IndentStatusFlyoutViewModel : BindableBase
    {

        private IEventAggregator _eventAggregator;
        public IndentStatusFlyoutViewModel(IEventAggregator eventAggregator)
        {
            Width = 1200;
            Height = 200;
            Position = Position.Bottom;

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<IndentStatusFlyoutEvent>().Subscribe((x) =>
            {
                isOpen = !isOpen;
            });
        }

        private string header;
        public string Header
        {
            get { return this.header; }
            set { SetProperty(ref this.header, value); }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return this.isOpen; }
            set { SetProperty(ref this.isOpen, value); }
        }

        private Position position;
        public Position Position
        {
            get { return this.position; }
            set { SetProperty(ref this.position, value); }

        }


        private int _width;


        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }

        }
        private int _height;

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }

        }

    }
}
