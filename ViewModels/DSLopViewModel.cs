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
            DanhSachLop = new ObservableCollection<HocSinhItem>();
            LoadDataFromDatabase();

            DanhSachLop.CollectionChanged += (s, e) => {
                OnPropertyChanged(nameof(SiSoText));
                AddStudentCommand.NotifyCanExecuteChanged();
            };
        }

        private void LoadDataFromDatabase()
        {
            DanhSachLop.Clear();
            try
            {
                var listHS = Services.HocSinh.LayDanhSach();
                int stt = 1;
                foreach (var hs in listHS)
                {
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
                NotificationHelper.ShowError($"Lỗi kết nối CSDL:\n{ex.Message}");
            }
        }

        private bool CanAddStudent()
        {
            return DanhSachLop != null && DanhSachLop.Count < QuyDinhService.maxClassSize;
        }

        [RelayCommand(CanExecute = nameof(CanAddStudent))]
        private void AddStudent()
        {
            Application.Current.MainWindow.Effect = new BlurEffect { Radius = 8 };
            Views.ThemHocSinhWnd popup = new Views.ThemHocSinhWnd();
            popup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            popup.ShowDialog();
            Application.Current.MainWindow.Effect = null;
        }

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

        [RelayCommand]
        private void XoaStudent(HocSinhItem hs)
        {
            if (hs != null)
            {
                bool isChonOK = NotificationHelper.ShowConfirm($"Bạn có chắc chắn muốn xóa học sinh '{hs.HoTen}' khỏi hệ thống không?\nHành động này không thể hoàn tác!");

                if (isChonOK)
                {
                    try
                    {
                        bool isDeleted = Services.HocSinh.Xoa(hs.MaHS);
                        if (isDeleted)
                        {
                            DanhSachLop.Remove(hs);
                            NotificationHelper.ShowSuccess("Xóa học sinh thành công!");
                        }
                        else
                        {
                            NotificationHelper.ShowWarning("Xóa thất bại! Không tìm thấy mã học sinh.");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        NotificationHelper.ShowError($"Không thể xóa do lỗi CSDL:\n{ex.Message}");
                    }
                }
            }
        }
    }
}