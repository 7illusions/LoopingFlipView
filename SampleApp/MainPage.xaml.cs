using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<SampleEntity> Items
        { get; set; }

        public MainPage()
        {
            Items = GetSampleData();
            DataContext = this;

            this.InitializeComponent();
        }

        private ObservableCollection<SampleEntity> GetSampleData()
        {
            return new ObservableCollection<SampleEntity>
            {
                new SampleEntity { Title = "Image courtesy of jscreationzs at FreeDigitalPhotos.net", ImageUrl = "ms-appx:///Assets/ID-10025978.jpg"},
                new SampleEntity { Title = "Image courtesy of scottchan at FreeDigitalPhotos.net", ImageUrl = "ms-appx:///Assets/ID-10031660.jpg" },
                new SampleEntity { Title = "Image courtesy of  Sira Anamwong at FreeDigitalPhotos.net", ImageUrl = "ms-appx:///Assets/ID-100181724.jpg" },
                new SampleEntity { Title = "Image courtesy of fotographic1980 at FreeDigitalPhotos.net", ImageUrl = "ms-appx:///Assets/ID-100319049.jpg"},
                new SampleEntity { Title = "Image courtesy of fotographic1980 at FreeDigitalPhotos.net", ImageUrl = "ms-appx:///Assets/ID-100140364.jpg"},
            };
        }              

        private void EnableTimer_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (EnableTimer.IsOn)
            {
                LoopingFlipView.Interval = 5;
            }
            else
            {
                LoopingFlipView.Interval = 0;
            }
        }

        private void EnableContext_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (EnableContext.IsOn)
            {
                LoopingFlipView.ContextIndicatorVisiblity = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                LoopingFlipView.ContextIndicatorVisiblity = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}
