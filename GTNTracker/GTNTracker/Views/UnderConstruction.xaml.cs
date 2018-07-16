
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderConstruction : ContentPage
    {
        public UnderConstruction()
        {
            InitializeComponent();
            mainImage.Source = ImageSource.FromResource("GTNTracker.Images.UnderConstruct.jpg");
        }
    }
}