using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using WPF_StudentManagement_Project.Services;

namespace WPF_StudentManagement_Project.ViewModels
{
    public partial class HocSinhViewModel : ObservableObject
    {
        [ObservableProperty] private string _hoTen = string.Empty;
        [ObservableProperty] private bool _isNam = true;
        [ObservableProperty] private bool _isNu;
        [ObservableProperty] private string _diaChi = string.Empty;
        [ObservableProperty] private string _email = string.Empty;

        // Báo lỗi màu đỏ
        [ObservableProperty]
        private string _tuoiErrorMessage = string.Empty;

        // Khi NgaySinh thay đổi, tự động tính lại tuổi và cập nhật trạng thái nút Lưu
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private DateTime _ngaySinh = DateTime.Now;

        // Tự động chạy mỗi khi _ngaySinh thay đổi
        partial void OnNgaySinhChanged(DateTime value)
        {
            // Tính tuổi chính xác
            int age = DateTime.Now.Year - value.Year;
            if (DateTime.Now.DayOfYear < value.DayOfYear) age--;

            // Kiểm tra quy định
            if (age < QuyDinhService.minTuoi || age > QuyDinhService.maxTuoi)
            {
                TuoiErrorMessage = $"Lỗi: Tuổi học sinh ({age} tuổi) không hợp lệ.\nQuy định từ {QuyDinhService.minTuoi} - {QuyDinhService.maxTuoi} tuổi.";
            }
            else
            {
                TuoiErrorMessage = string.Empty; // Xóa lỗi nếu hợp lệ
            }
        }

        // Điều kiện để nút Lưu sáng lên: Không có thông báo lỗi nào
        private bool CanLuu()
        {
            return string.IsNullOrEmpty(TuoiErrorMessage);
        }

        // Gắn điều kiện CanExecute vào lệnh Lưu
        [RelayCommand(CanExecute = nameof(CanLuu))]
        private void Luu()
        {
            string gioiTinh = IsNam ? "Nam" : "Nữ";
            MessageBox.Show($"Đã lưu: {HoTen}\nGiới tính: {gioiTinh}\nSinh ngày: {NgaySinh:dd/MM/yyyy}", "Thông báo");
        }

        [RelayCommand]
        private void Huy()
        {
            HoTen = string.Empty;
            DiaChi = string.Empty;
            Email = string.Empty;
            NgaySinh = DateTime.Now;
            IsNam = true;
        }
    }
}