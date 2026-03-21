using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_StudentManagement_Project.Views
{
    /// <summary>
    /// Interaction logic for LapDanhSachUC.xaml
    /// </summary>
    /// 
    public class HocSinh
    {
        public int STT { get; set; }
        public string MaHS { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string NgaySinh { get; set; }
    }

    public partial class LapDanhSachUC : UserControl
    {
        public ObservableCollection<HocSinh> DanhSachLop { get; set; }
        public LapDanhSachUC()
        {
            InitializeComponent();

            DanhSachLop = new ObservableCollection<HocSinh>
            {
                new HocSinh { STT = 1, MaHS = "HS001", HoTen = "Nguyễn Văn A", GioiTinh = "Nam", NgaySinh = "10/01/2008" },
                new HocSinh { STT = 2, MaHS = "HS002", HoTen = "Trần Thị B", GioiTinh = "Nữ", NgaySinh = "15/02/2008" },
                new HocSinh { STT = 3, MaHS = "HS003", HoTen = "Lê Hoàng C", GioiTinh = "Nam", NgaySinh = "20/03/2008" },
                new HocSinh { STT = 4, MaHS = "HS004", HoTen = "Phạm Thị D", GioiTinh = "Nữ", NgaySinh = "25/04/2008" },
                new HocSinh { STT = 5, MaHS = "HS005", HoTen = "Hoàng Văn E", GioiTinh = "Nam", NgaySinh = "30/05/2008" },
                new HocSinh { STT = 6, MaHS = "HS006", HoTen = "Vũ Thị F", GioiTinh = "Nữ", NgaySinh = "05/06/2008" },
                new HocSinh { STT = 7, MaHS = "HS007", HoTen = "Đặng Thái G", GioiTinh = "Nam", NgaySinh = "12/07/2008" },
                new HocSinh { STT = 8, MaHS = "HS008", HoTen = "Bùi Thu H", GioiTinh = "Nữ", NgaySinh = "18/08/2008" },
                new HocSinh { STT = 9, MaHS = "HS009", HoTen = "Đỗ Tấn I", GioiTinh = "Nam", NgaySinh = "22/09/2008" },
                new HocSinh { STT = 10, MaHS = "HS010", HoTen = "Hồ Ngọc K", GioiTinh = "Nữ", NgaySinh = "28/10/2008" }
            };

            dtgDanhSach.ItemsSource = DanhSachLop;
        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            ThemHocSinhWnd popup = new ThemHocSinhWnd();
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                popup.Owner = parentWindow;
                popup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            popup.ShowDialog();
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var hocSinh = button.DataContext as HocSinh;
            if (hocSinh != null)
            {
                DanhSachLop.Remove(hocSinh);
                txtSiSo.Text = "Sĩ số: " + DanhSachLop.Count;
            }
        }
    }
}
