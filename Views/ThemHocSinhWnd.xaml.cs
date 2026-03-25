using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_StudentManagement_Project.Views
{
    /// <summary>
    /// Interaction logic for ThemHocSinhWnd.xaml
    /// </summary>
    public partial class ThemHocSinhWnd : Window
    {
        public ThemHocSinhWnd()
        {
            InitializeComponent();

            var ds = new List<object>
            {
                new { HoTen="Phạm Thị D", MaHS="HS004", GioiTinh="Nữ", NgaySinh="15/08/2008" },
                new { HoTen="Hoàng Văn E", MaHS="HS005", GioiTinh="Nam", NgaySinh="22/11/2008" },
                new { HoTen="Vũ Thị F", MaHS="HS006", GioiTinh="Nữ", NgaySinh="05/01/2009" },
                new { HoTen="Trần Văn G", MaHS="HS007", GioiTinh="Nam", NgaySinh="12/03/2008" },
                new { HoTen="Lê Thị H", MaHS="HS008", GioiTinh="Nữ", NgaySinh="29/12/2008" }
            };

            ThemHSLB.ItemsSource = ds;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
