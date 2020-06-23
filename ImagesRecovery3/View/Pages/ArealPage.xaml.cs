using ImagesRecovery3.ViewModel;
using System.Windows.Controls;

namespace ImagesRecovery3.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для ArealPage.xaml
    /// </summary>
    public partial class ArealPage : Page
    {
        public ArealPage()
        {
            InitializeComponent();
            DataContext = new ArealPageVM();
        }
    }
}
