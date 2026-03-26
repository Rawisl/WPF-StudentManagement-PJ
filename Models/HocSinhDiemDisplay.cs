using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace WPF_StudentManagement_Project.Models
{
    public partial class HocSinhDiemDisplay : ObservableObject
    {
        [ObservableProperty] private int _stt;
        [ObservableProperty] private string _hoTen;

        // Phép thuật: Cứ mỗi lần Điểm 15p hoặc 1 tiết thay đổi, báo cho giao diện biết Điểm TB cũng thay đổi theo!
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DiemTB))]
        private double? _diem15Phut;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DiemTB))]
        private double? _diem1Tiet;

        // Điểm TB chỉ cho phép Đọc (get), tự động tính toán
        public double? DiemTB
        {
            get
            {
                if (Diem15Phut.HasValue && Diem1Tiet.HasValue)
                    return (Diem15Phut.Value + Diem1Tiet.Value * 2) / 3;
                return null;
            }
        }
    }
}