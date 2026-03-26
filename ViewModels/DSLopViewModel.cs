using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WPF_StudentManagement_Project.Services;
using System.Windows.Media.Effects;

namespace WPF_StudentManagement_Project.ViewModels
{
    public partial class HocSinhItem : ObservableObject
    {
        [ObservableProperty] private int _sTT;
        [ObservableProperty] private string _maHS = string.Empty;
        [ObservableProperty] private string _hoTen = string.Empty;
        [ObservableProperty] private string _gioiTinh = string.Empty;
        [ObservableProperty] private string _ngaySinh = string.Empty;
    }

    public partial class DSLopViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<HocSinhItem> _danhSachLop;

        public string SiSoText => $"Sĩ số: {DanhSachLop?.Count ?? 0} / {QuyDinhService.maxClassSize}";

        public DSLopViewModel()
        {
            // 1. Khởi tạo danh sách rỗng
            DanhSachLop = new ObservableCollection<HocSinhItem>();

            // 2. Load dữ liệu THẬT từ CSDL của Long
            LoadDataFromDatabase();

            // Mỗi khi danh sách thay đổi (Thêm/Xóa)
            DanhSachLop.CollectionChanged += (s, e) => {
                OnPropertyChanged(nameof(SiSoText)); // Cập nhật chữ Sĩ số
                AddStudentCommand.NotifyCanExecuteChanged(); // Đánh giá lại xem nút Thêm có được bật không
            };
        }

        // Hàm hỗ trợ load dữ liệu
        private void LoadDataFromDatabase()
        {
            DanhSachLop.Clear();

            try
            {
                // Gọi hàm LayDanhSach() từ file HocSinh.cs
                var listHS = Services.HocSinh.LayDanhSach();

                int stt = 1;
                foreach (var hs in listHS)
                {
                    // Chuyển đổi từ Model của Long sang ViewModel (Item) để hiển thị lên màn hình
                    DanhSachLop.Add(new HocSinhItem
                    {
                        STT = stt++,
                        MaHS = hs.MaHocSinh,
                        HoTen = hs.HoTen,
                        GioiTinh = hs.GioiTinh,
                        NgaySinh = hs.NgaySinh.ToString("dd/MM/yyyy")
                    });
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL khi tải danh sách:\n{ex.Message}", "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Điều kiện: Sĩ số hiện tại phải nhỏ hơn maxSiSo
        private bool CanAddStudent()
        {
            return DanhSachLop != null && DanhSachLop.Count < QuyDinhService.maxClassSize;
        }

        // Gắn điều kiện vào nút
        [RelayCommand(CanExecute = nameof(CanAddStudent))]
        private void AddStudent()
        {
            Application.Current.MainWindow.Effect = new BlurEffect { Radius = 8 };
            Views.ThemHocSinhWnd popup = new Views.ThemHocSinhWnd();
            popup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            popup.ShowDialog();
            Application.Current.MainWindow.Effect = null;
        }

        //Lệnh Sửa học sinh
        [RelayCommand]
        private void EditStudent(HocSinhItem hs)
        {
            if (hs != null)
            {
                Application.Current.MainWindow.Effect = new BlurEffect { Radius = 8 };
                Views.SuaHocSinhWnd popup = new Views.SuaHocSinhWnd(hs);

                popup.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                popup.ShowDialog();
                Application.Current.MainWindow.Effect = null;
            }
        }

        //Lệnh Xóa học sinh
        [RelayCommand]
        private void XoaStudent(HocSinhItem hs)
        {
            if (hs != null)
            {
                //Hiện hộp thoại hỏi
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa học sinh '{hs.HoTen}' khỏi hệ thống không?\nHành động này không thể hoàn tác!",
                                             "Xác nhận xóa",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                // Nếu người dùng bấm Yes
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Gọi hàm Xóa từ file HocSinh.cs
                        bool isDeleted = Services.HocSinh.Xoa(hs.MaHS);

                        if (isDeleted)
                        {
                            // Xóa trên giao diện (ObservableCollection sẽ tự update UI)
                            DanhSachLop.Remove(hs);
                            MessageBox.Show("Xóa học sinh thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Xóa thất bại! Không tìm thấy mã học sinh trong CSDL.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // Bắt lỗi nếu học sinh đang có điểm số (vướng khóa ngoại)
                        MessageBox.Show($"Không thể xóa do lỗi CSDL:\n{ex.Message}", "Lỗi nghiêm trọng", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}