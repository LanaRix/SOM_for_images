using ImagesRecovery3.ViewModel;
using System.Windows.Controls;

namespace ImagesRecovery3.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для RandomPage.xaml
    /// </summary>
    public partial class RandomPage : Page
    {
        public RandomPage()
        {
            InitializeComponent();
            DataContext = new RandomPageVM();
        }
    }
}
