using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_StudentManagement_Project.Views;

namespace WPF_StudentManagement_Project.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        // Biến lưu trang hiện tại đang hiển thị trên ContentControl
        [ObservableProperty]
        private object _currentView;

        [RelayCommand]
        private void Navigate(object destinationViewModel)
        {
            // 1. KIỂM TRA CHỐT CHẶN
            // Nếu trang hiện tại ĐANG LÀ trang Cài Đặt (BM6) VÀ Cờ Dirty đang bật
            if (CurrentView is ThayDoiQDViewModel caiDatVM && caiDatVM.HasUnsavedChanges)
            {
                // Bật Cảnh báo
                MessageBoxResult result = MessageBox.Show(
                    "Bạn có thay đổi chưa lưu! Bạn có chắc chắn muốn rời đi và MẤT dữ liệu vừa nhập không?",
                    "Cảnh báo chưa lưu",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Nếu người dùng chọn No (Không muốn rời đi)
                if (result == MessageBoxResult.No)
                {
                    return; // Hủy lệnh chuyển trang, ở lại BM6
                }

                // Nếu chọn Yes (Chấp nhận mất dữ liệu để rời đi)
                // Ép cờ về false để lần sau quay lại nó không bị kẹt
                caiDatVM.HasUnsavedChanges = false;
            }

            // 2. NẾU AN TOÀN -> THỰC HIỆN CHUYỂN TRANG
            CurrentView = destinationViewModel;
        }
    }
}