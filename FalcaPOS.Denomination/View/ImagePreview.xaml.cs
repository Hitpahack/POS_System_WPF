using FalcaPOS.Common.Events;
using Prism.Events;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Media.Imaging;

namespace FalcaPOS.Denomination.View
{
    /// <summary>
    /// Interaction logic for ImagePreview.xaml
    /// </summary>
    public partial class ImagePreview : UserControl
    {
        private IEventAggregator _eventAggregator;

        public ImagePreview(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator= eventAggregator;
            _eventAggregator.GetEvent<PreviewEvent>().Subscribe((path) =>
            {
                if (String.IsNullOrEmpty(path)) return;
                LoadSampleImage(this.imagepreviewUI, path);
            });

        }

        public static void LoadSampleImage(RadImageEditorUI imageEditor, string path)
        {
            try
            {
                var images = new System.Windows.Media.Imaging.BitmapImage( new Uri(path));
                imageEditor.Image = new Telerik.Windows.Media.Imaging.RadBitmap(images);
                imageEditor.ApplyTemplate();
               
            }
            catch (Exception _ex)
            {

            }
        }
        
    }
}
