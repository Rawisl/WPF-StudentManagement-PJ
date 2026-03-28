using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_StudentManagement_Project.Services;
using WPF_StudentManagement_Project.Views;

namespace WPF_StudentManagement_Project.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        // Tạo sẵn một đối tượng để dùng chung (singleton trong nội bộ MainVM)
        public TrangChuViewModel TrangChuVM { get; } = new TrangChuViewModel();
        public HocSinhViewModel HocSinhVM { get; } = new HocSinhViewModel();
        public DSLopViewModel DSLopVM { get; } = new DSLopViewModel();
        public TraCuuViewModel TraCuuVM { get; } = new TraCuuViewModel();
        public NhapDiemViewModel NhapDiemVM { get; } = new NhapDiemViewModel();
        public BaoCaoViewModel BaoCaoVM { get; } = new BaoCaoViewModel();
        public ThayDoiQDViewModel ThayDoiQDVM { get; } = new ThayDoiQDViewModel();
        public CaiDatViewModel CaiDatVM { get; } = new CaiDatViewModel();


        // Biến lưu trang hiện tại đang hiển thị trên ContentControl
        [ObservableProperty]
        private object _currentView;

        public MainViewModel()
        {
            CurrentView = TrangChuVM; // Trang mặc định
        }

        [RelayCommand]
        private void Navigate(object destinationViewModel)
        {
            // 1. KIỂM TRA CHỐT CHẶN
            // Nếu trang hiện tại ĐANG LÀ trang Cài Đặt (BM6) VÀ Cờ Dirty đang bật
            if (CurrentView is ThayDoiQDViewModel thayDoiQDVM && thayDoiQDVM.HasUnsavedChanges)
            {
                //Bật Cảnh báo
                bool isChacChanDi = NotificationHelper.ShowConfirm("Bạn có thay đổi chưa lưu! Xác nhận rời đi và mất dữ liệu?");

                if (!isChacChanDi) return; // Nếu chọn Hủy thì ở lại

                ThayDoiQDVM.HasUnsavedChanges = false;

            }

            // 2. NẾU AN TOÀN -> THỰC HIỆN CHUYỂN TRANG
            CurrentView = destinationViewModel;
        }
    }

}