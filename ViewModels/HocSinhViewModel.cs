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
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private string _hoTen = string.Empty;

        [ObservableProperty] private bool _isNam = true;
        [ObservableProperty] private bool _isNu;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private string _diaChi = string.Empty;

        [ObservableProperty] private string _email = string.Empty;

        // Thông báo lỗi nếu sai tuổi quy định
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private string _tuoiErrorMessage = string.Empty;

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
                NotificationHelper.ShowError($"Lỗi tải danh sách lớp:\n{ex.Message}");
            }
        }

        // Logic kiểm tra tuổi mỗi khi thay đổi ngày sinh
        partial void OnNgaySinhChanged(DateTime value)
        {
            int age = DateTime.Now.Year - value.Year;
            if (DateTime.Now.DayOfYear < value.DayOfYear) age--;

            if (age < Services.QuyDinhService.minAge || age > Services.QuyDinhService.maxAge)
            {
                TuoiErrorMessage = $"Lỗi: Tuổi học sinh ({age} tuổi) không hợp lệ.\nQuy định từ {Services.QuyDinhService.minAge} - {Services.QuyDinhService.maxAge} tuổi.";
            }
            else
            {
                TuoiErrorMessage = string.Empty;
            }
        }

        // Kiểm tra điều kiện để kích hoạt nút Lưu
        private bool CanLuu()
        {
            // Bắt buộc nhập Họ Tên và Địa Chỉ, và không có lỗi tuổi
            return string.IsNullOrEmpty(TuoiErrorMessage) &&
                   !string.IsNullOrWhiteSpace(HoTen) &&
                   !string.IsNullOrWhiteSpace(DiaChi);
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
                //MaLop = LopDuocChon!.MaLop
            };

            try
            {
                if (hsMoi.Them())
                {
                    NotificationHelper.ShowSuccess($"Tiếp nhận học sinh thành công!\nMã HS: {maHSMoi}\nHọ Tên: {HoTen}");
                    Huy();
                }
                else
                {
                    NotificationHelper.ShowWarning("Không thể lưu học sinh vào hệ thống. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi CSDL:\n{ex.Message}");
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

            // Xóa luôn câu báo lỗi đỏ nếu có
            TuoiErrorMessage = string.Empty;
        }
    }
}