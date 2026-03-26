using System;
using System.Linq;
using System.Windows;
using WPF_StudentManagement_Project.ViewModels;

namespace WPF_StudentManagement_Project.Views
{
    public partial class SuaHocSinhWnd : Window
    {
        private HocSinhItem _hsItemDangSua; // Dữ liệu đang hiển thị trên Grid

        public SuaHocSinhWnd(HocSinhItem hs)
        {
            InitializeComponent();
            _hsItemDangSua = hs;

            // Đổ dữ liệu từ lưới vào các ô TextBox thông qua Binding
            this.DataContext = _hsItemDangSua;
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            // Lấy toàn bộ thông tin gốc của học sinh từ CSDL 
            var danhSachDB = Services.HocSinh.LayDanhSach();
            var hsGoc = danhSachDB.FirstOrDefault(x => x.MaHocSinh == _hsItemDangSua.MaHS);

            if (hsGoc != null)
            {
                // Cập nhật các trường vừa bị thay đổi trên form
                hsGoc.HoTen = txtHoTen.Text;
                hsGoc.GioiTinh = cbGioiTinh.Text;

                // Xử lý an toàn cho Ngày Sinh
                DateTime parsedDate;
                if (DateTime.TryParseExact(txtNgaySinh.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    hsGoc.NgaySinh = parsedDate;
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập ngày sinh đúng định dạng dd/MM/yyyy!", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi hàm Sua() của Long để lưu vào Database
                if (hsGoc.Sua())
                {
                    // Báo cáo thành công và cập nhật lại dữ liệu hiển thị trên Grid
                    _hsItemDangSua.HoTen = hsGoc.HoTen;
                    _hsItemDangSua.GioiTinh = hsGoc.GioiTinh;
                    _hsItemDangSua.NgaySinh = hsGoc.NgaySinh.ToString("dd/MM/yyyy");

                    MessageBox.Show("Cập nhật thông tin học sinh thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // Đóng Popup
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi lưu xuống CSDL!", "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}