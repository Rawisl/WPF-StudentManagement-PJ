using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace WPF_StudentManagement_Project.ViewModels
{
    public partial class HocSinhViewModel : ObservableObject
    {
        [ObservableProperty] private string _hoTen;
        [ObservableProperty] private bool _isNam = true;
        [ObservableProperty] private bool _isNu;
        [ObservableProperty] private DateTime _ngaySinh = DateTime.Now;
        [ObservableProperty] private string _diaChi;
        [ObservableProperty] private string _email;

        [RelayCommand]
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
