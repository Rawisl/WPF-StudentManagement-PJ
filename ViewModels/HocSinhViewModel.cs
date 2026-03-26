using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import mvvm lib
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace WPF_StudentManagement_Project.ViewModels
{
    //Dùng mvvm thì phải thêm 'partial' với kế thừa ObservableObject vào
    internal partial class HocSinhViewModel : ObservableObject
    {
        //Khai báo các ô nhập liệu ở đây, viết chữ thường, có dấu gạch dưới
        [ObservableProperty]
        private string _tenHocSinh;

        [ObservableProperty]
        private int _tuoi;

        //Viết logic khi bấm nút Lưu vào đây
        [RelayCommand]
        private void LuuThongTin()
        {
            //Tự viết logic gọi QuyDinhService.TuoiMin ở đây
            //MessageBox.Show($"Đang lưu học sinh: {TenHocSinh}");
        }
    }
}
