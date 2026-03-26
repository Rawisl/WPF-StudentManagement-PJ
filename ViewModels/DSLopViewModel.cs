using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace WPF_StudentManagement_Project.ViewModels
{
    // Class phụ dùng để hiển thị lên DataGrid của BM2
    public partial class HocSinhItem : ObservableObject
    {
        [ObservableProperty] private int _sTT;
        [ObservableProperty] private string _maHS;
        [ObservableProperty] private string _hoTen;
        [ObservableProperty] private string _gioiTinh;
        [ObservableProperty] private string _ngaySinh;
    }

    public partial class DSLopViewModel : ObservableObject
    {
        // Danh sách học sinh sẽ tự động cập nhật lên UI
        [ObservableProperty]
        private ObservableCollection<HocSinhItem> _danhSachLop;

        // Tự động tính Sĩ số mỗi khi có thay đổi
        public string SiSoText => $"Sĩ số: {DanhSachLop?.Count ?? 0}";

        public DSLopViewModel()
        {
            // Dữ liệu giả lập (Mock data)
            DanhSachLop = new ObservableCollection<HocSinhItem>
            {
                new HocSinhItem { STT = 1, MaHS = "HS001", HoTen = "Nguyễn Văn A", GioiTinh = "Nam", NgaySinh = "10/01/2008" },
                new HocSinhItem { STT = 2, MaHS = "HS002", HoTen = "Trần Thị B", GioiTinh = "Nữ", NgaySinh = "15/02/2008" },
                new HocSinhItem { STT = 3, MaHS = "HS003", HoTen = "Lê Hoàng C", GioiTinh = "Nam", NgaySinh = "20/03/2008" }
            };

            // Lắng nghe sự kiện thêm/xóa để cập nhật số lượng sĩ số
            DanhSachLop.CollectionChanged += (s, e) => OnPropertyChanged(nameof(SiSoText));
        }

        // Lệnh Thêm học sinh
        [RelayCommand]
        private void AddStudent()
        {
            Views.ThemHocSinhWnd popup = new Views.ThemHocSinhWnd();
            popup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            popup.ShowDialog();
        }

        // Lệnh Xóa học sinh
        [RelayCommand]
        private void XoaStudent(HocSinhItem hs)
        {
            if (hs != null)
            {
                DanhSachLop.Remove(hs);
            }
        }
    }
}