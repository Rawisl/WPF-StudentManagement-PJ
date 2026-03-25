using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_StudentManagement_Project.Views;

namespace WPF_StudentManagement_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new TrangChuUC();
        }

        private void Btn_TrangChu_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TrangChuUC();
        }

        private void Btn_TiepNhanHS_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TiepNhanUC();
        }

        private void Btn_LapDSL_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LapDanhSachUC();

        }

        private void Btn_NhapDiemMon_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new NhapDiemUC();

        }

        private void Btn_BaoCao_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new BaoCaoUC();

        }

        private void Btn_ThayDoiQD_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ThayDoiQDUC();
        }

        private void Btn_CaiDat_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CaiDatUC();

        }
    }
}