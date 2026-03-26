using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_StudentManagement_Project.Views
{
    /// <summary>
    /// Interaction logic for NhapDiemUC.xaml
    /// </summary>
    public partial class NhapDiemUC : UserControl
    {
        public NhapDiemUC()
        {
            InitializeComponent();
        }
        private void GetListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // HÀM CHẶN GÕ CHỮ
        private void ChiChoPhepNhapSo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
