using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace WPF_StudentManagement_Project.ViewModels
{
    // TẠO CLASS TRUNG GIAN (Giống hệt cách bạn làm với HocSinhItem)
    public class LopItem
    {
        public string MaLop { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
    }

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

        // DÙNG LopItem THAY VÌ Services.Lop ĐỂ KHÔNG BỊ LỖI BẢO MẬT (CS0053)
        [ObservableProperty]
        private ObservableCollection<LopItem> _danhSachLop = new ObservableCollection<LopItem>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private LopItem? _lopDuocChon;

        // Khởi tạo ViewModel: Tự động lôi danh sách lớp từ DB lên
        public HocSinhViewModel()
        {
            try
            {
                // Gọi hàm lấy danh sách của Long
                var list = Services.Lop.LayDanhSach();

                // Đổ dữ liệu từ class của Long sang class trung gian của Giao diện
                foreach (var lop in list)
                {
                    DanhSachLop.Add(new LopItem
                    {
                        MaLop = lop.MaLop,
                        TenLop = lop.TenLop ?? lop.MaLop // Nếu không có tên thì lấy mã làm tên
                    });
                }

                // Chọn sẵn lớp đầu tiên cho tiện
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

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LuuCommand))]
        private DateTime _ngaySinh = DateTime.Now;

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

        // Điều kiện để nút Lưu sáng lên: Không có lỗi tuổi VÀ phải chọn Lớp
        private bool CanLuu()
        {
            return string.IsNullOrEmpty(TuoiErrorMessage) && LopDuocChon != null;
        }


        [RelayCommand(CanExecute = nameof(CanLuu))]
        private void Luu()
        {
            string gioiTinh = IsNam ? "Nam" : "Nữ";

            // Tự động sinh Mã Học Sinh
            string maHSMoi = "HS" + DateTime.Now.ToString("ddHHmmss");

            // Lấy mã lớp thực tế mà người dùng chọn
            string maLopThucTe = LopDuocChon!.MaLop;

            Services.HocSinh hsMoi = new Services.HocSinh()
            {
                MaHocSinh = maHSMoi,
                HoTen = this.HoTen,
                GioiTinh = gioiTinh,
                NgaySinh = this.NgaySinh,
                DiaChi = this.DiaChi,
                Email = this.Email,
                MaLop = maLopThucTe
            };

            try
            {
                if (hsMoi.Them()) // Gọi hàm Them() của Long
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