using MahApps.Metro.Controls;
using Prism.Mvvm;

namespace FalcaPOS.Common.ViewModel
{
    public class FlyoutBaseViewModel : BindableBase
    {
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
        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private Position position;
        public Position Position
        {
            get { return this.position; }
            set { SetProperty(ref this.position, value); }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set { _height = value; }

        }
    }
}
