using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using WPF_StudentManagement_Project.Services;

namespace WPF_StudentManagement_Project.ViewModels
{
    // Lớp trung gian để hiển thị danh sách lớp lên giao diện
    public class LopItem
    {
        public string MaLop { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
    }

    // ViewModel chính cho màn hình Tiếp nhận học sinh
    public partial class HocSinhViewModel : ObservableObject
    {
        [ObservableProperty] private string _hoTen = string.Empty;
        [ObservableProperty] private bool _isNam = true;
        [ObservableProperty] private bool _isNu;
        [ObservableProperty] private string _diaChi = string.Empty;
        [ObservableProperty] private string _email = string.Empty;

        // Thông báo lỗi nếu sai tuổi quy định
        [ObservableProperty] private string _tuoiErrorMessage = string.Empty;

        // Danh sách lớp nạp từ Database
        [ObservableProperty]
        private ObservableCollection<LopItem> _danhSachLop = new ObservableCollection<LopItem>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private LopItem? _lopDuocChon;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private DateTime _ngaySinh = DateTime.Now;

        // Constructor: Tự động tải danh sách lớp khi khởi tạo màn hình
        public HocSinhViewModel()
        {
            try
            {
                // Gọi hàm lấy danh sách từ Services của Long
                var list = Services.Lop.LayDanhSach();

                foreach (var lop in list)
                {
                    DanhSachLop.Add(new LopItem
                    {
                        MaLop = lop.MaLop,
                        TenLop = lop.TenLop ?? lop.MaLop
                    });
                }

                // Tự động chọn lớp đầu tiên
                if (DanhSachLop.Count > 0)
                {
                    LopDuocChon = DanhSachLop[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách lớp:\n{ex.Message}", "Lỗi DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Logic kiểm tra tuổi mỗi khi thay đổi ngày sinh
        partial void OnNgaySinhChanged(DateTime value)
        {
            int age = DateTime.Now.Year - value.Year;
            if (DateTime.Now.DayOfYear < value.DayOfYear) age--;

            if (age < Services.QuyDinhService.minTuoi || age > Services.QuyDinhService.maxTuoi)
            {
                TuoiErrorMessage = $"Lỗi: Tuổi học sinh ({age} tuổi) không hợp lệ.\nQuy định từ {Services.QuyDinhService.minTuoi} - {Services.QuyDinhService.maxTuoi} tuổi.";
            }
            else
            {
                TuoiErrorMessage = string.Empty;
            }
        }

        // Kiểm tra điều kiện để kích hoạt nút Lưu
        private bool CanLuu()
        {
            return string.IsNullOrEmpty(TuoiErrorMessage) && LopDuocChon != null;
        }

        [RelayCommand(CanExecute = nameof(CanLuu))]
        private void Luu()
        {
            string gioiTinh = IsNam ? "Nam" : "Nữ";

            // Tự động sinh mã HS 10 ký tự (ddHHmmss + HS) để tránh lỗi truncated
            string maHSMoi = "HS" + DateTime.Now.ToString("ddHHmmss");

            Services.HocSinh hsMoi = new Services.HocSinh()
            {
                MaHocSinh = maHSMoi,
                HoTen = this.HoTen,
                GioiTinh = gioiTinh,
                NgaySinh = this.NgaySinh,
                DiaChi = this.DiaChi,
                Email = this.Email,
                MaLop = LopDuocChon!.MaLop
            };

            try
            {
                if (hsMoi.Them())
                {
                    MessageBox.Show($"Tiếp nhận học sinh thành công!\nMã HS: {maHSMoi}\nHọ Tên: {HoTen}\nVào lớp: {LopDuocChon.TenLop}",
                                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    Huy();
                }
                else
                {
                    MessageBox.Show("Không thể lưu học sinh vào hệ thống. Vui lòng thử lại.",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi CSDL:\n{ex.Message}", "Lỗi nghiêm trọng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Huy()
        {
            HoTen = string.Empty;
            DiaChi = string.Empty;
            Email = string.Empty;
            NgaySinh = DateTime.Now;
            IsNam = true;
            if (DanhSachLop.Count > 0) LopDuocChon = DanhSachLop[0];
        }
    }
}