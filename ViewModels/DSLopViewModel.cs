using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WPF_StudentManagement_Project.Services;

namespace WPF_StudentManagement_Project.ViewModels
{
    public partial class HocSinhItem : ObservableObject
    {
        [ObservableProperty] private int _sTT;
        [ObservableProperty] private string _maHS = string.Empty;
        [ObservableProperty] private string _hoTen = string.Empty;
        [ObservableProperty] private string _gioiTinh = string.Empty;
        [ObservableProperty] private string _ngaySinh = string.Empty;
    }

    public partial class DSLopViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<HocSinhItem> _danhSachLop;

        public string SiSoText => $"Sĩ số: {DanhSachLop?.Count ?? 0} / {QuyDinhService.maxSiSo}";

        public DSLopViewModel()
        {
            DanhSachLop = new ObservableCollection<HocSinhItem>
            {
                new HocSinhItem { STT = 1, MaHS = "HS001", HoTen = "Nguyễn Văn A", GioiTinh = "Nam", NgaySinh = "10/01/2008" }
            };

            // Mỗi khi danh sách thay đổi (Thêm/Xóa)
            DanhSachLop.CollectionChanged += (s, e) => {
                OnPropertyChanged(nameof(SiSoText)); // Cập nhật chữ Sĩ số
                AddStudentCommand.NotifyCanExecuteChanged(); // Đánh giá lại xem nút Thêm có được bật không
            };
        }

        // Điều kiện: Sĩ số hiện tại phải nhỏ hơn maxSiSo
        private bool CanAddStudent()
        {
            return DanhSachLop != null && DanhSachLop.Count < QuyDinhService.maxSiSo;
        }

        // Gắn điều kiện vào nút
        [RelayCommand(CanExecute = nameof(CanAddStudent))]
        private void AddStudent()
        {
            Views.ThemHocSinhWnd popup = new Views.ThemHocSinhWnd();
            popup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            popup.ShowDialog();
        }

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