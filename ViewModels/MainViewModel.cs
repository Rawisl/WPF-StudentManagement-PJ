using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_StudentManagement_Project.ViewModels
{
    public partial class ThayDoiQDViewModel : ObservableObject
    {
        // 1. Cờ đánh dấu có thay đổi chưa lưu
        public bool HasUnsavedChanges { get; set; } = false;

        [ObservableProperty]
        private int _tuoiMin;

        // 2. Hàm này TỰ ĐỘNG CHẠY mỗi khi người dùng gõ số mới vào ô Tuổi Min
        partial void OnTuoiMinChanged(int value)
        {
            HasUnsavedChanges = true;
        }

        // Tương tự cho TuoiMax, SiSoMax...
        [ObservableProperty]
        private int _tuoiMax;
        partial void OnTuoiMaxChanged(int value) => HasUnsavedChanges = true;

        [RelayCommand]
        private void LuuCaiDat()
        {
            // Code gọi Database lưu xuống DB của Long...

            // 3. Lưu xong thì "rửa sạch" cờ
            HasUnsavedChanges = false;
            MessageBox.Show("Lưu thành công!");
        }
    }

}