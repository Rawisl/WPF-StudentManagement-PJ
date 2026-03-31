using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WPF_StudentManagement_Project.Services;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Generic;

namespace WPF_StudentManagement_Project.ViewModels
{
    public class LopHocModel
    {
        public int STT { get; set; }
        public string MaLop { get; set; } = string.Empty;
        public string Khoi { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
        public string XoaIcon { get; set; } = "X";
    }

    public class MonHocModel
    {
        public int STT { get; set; }
        public string MaMonHoc { get; set; } = string.Empty;
        public string TenMon { get; set; } = string.Empty;
        public string XoaIcon { get; set; } = "X";
    }

    public partial class ThayDoiQDViewModel : ObservableObject
    {
        private bool _isLoaded = false;

        [ObservableProperty]
        private bool _hasUnsavedChanges = false;

        // CÁC BIẾN NULLABLE ĐỂ CHO PHÉP XÓA RỖNG
        [ObservableProperty] private int? _minAge;
        [ObservableProperty] private int? _maxAge;
        [ObservableProperty] private int? _maxClassSize;
        [ObservableProperty] private double? _passingGrade;

        [ObservableProperty]
        private ObservableCollection<LopHocModel> _danhSachLop = new ObservableCollection<LopHocModel>();

        [ObservableProperty]
        private ObservableCollection<MonHocModel> _danhSachMon = new ObservableCollection<MonHocModel>();

        public ThayDoiQDViewModel()
        {
            QuyDinhService.LoadTuDatabase();
            LoadDataFromService();

            this.PropertyChanged += (s, e) =>
            {
                if (_isLoaded && (e.PropertyName == nameof(MinAge) || e.PropertyName == nameof(MaxAge) ||
                                  e.PropertyName == nameof(MaxClassSize) || e.PropertyName == nameof(PassingGrade)))
                {
                    HasUnsavedChanges = true;
                }
            };

            try
            {
                DataTable dtLop = DatabaseHelper.ExecuteQuery("SELECT TenLop, Khoi FROM LOP");
                int sttLop = 1;
                foreach (DataRow row in dtLop.Rows)
                {
                    DanhSachLop.Add(new LopHocModel
                    {
                        STT = sttLop++,
                        TenLop = row["TenLop"].ToString() ?? "",
                        Khoi = row["Khoi"].ToString() ?? ""
                    });
                }
            }
            catch { /* Bỏ qua nếu DB chưa có dữ liệu */ }
        }

        private void LoadDataFromService()
        {
            _isLoaded = false;

            MinAge = QuyDinhService.minAge;
            MaxAge = QuyDinhService.maxAge;
            MaxClassSize = QuyDinhService.maxClassSize;
            PassingGrade = QuyDinhService.passingGrade;

            _isLoaded = true;
            HasUnsavedChanges = false;
        }

        // --- LOGIC TĂNG/GIẢM  ---
        [RelayCommand]
        private void TangTuoiMin()
        {
            MinAge ??= 0; MaxAge ??= 0;
            if (MinAge < MaxAge && MinAge < 100) MinAge++;
        }

        [RelayCommand]
        private void GiamTuoiMin()
        {
            MinAge ??= 0;
            if (MinAge > 0) MinAge--;
        }

        [RelayCommand]
        private void TangTuoiMax()
        {
            MaxAge ??= 0;
            if (MaxAge < 100) MaxAge++;
        }

        [RelayCommand]
        private void GiamTuoiMax()
        {
            MaxAge ??= 0; MinAge ??= 0;
            if (MaxAge > MinAge && MaxAge > 0) MaxAge--;
        }

        [RelayCommand]
        private void TangSiSo()
        {
            MaxClassSize ??= 0;
            MaxClassSize++;
        }

        [RelayCommand]
        private void GiamSiSo()
        {
            MaxClassSize ??= 0;
            if (MaxClassSize > 0) MaxClassSize--;
        }

        [RelayCommand]
        private void TangDiem()
        {
            PassingGrade ??= 0.0;
            if (PassingGrade < 10.0) PassingGrade = System.Math.Round((double)PassingGrade.Value + 0.1, 1);
        }

        [RelayCommand]
        private void GiamDiem()
        {
            PassingGrade ??= 0.0;
            if (PassingGrade > 0.0) PassingGrade = System.Math.Round((double)PassingGrade.Value - 0.1, 1);
        }

        // --- LOGIC THÊM LỚP/MÔN ---
        [RelayCommand]
        private void ThemLop()
        {
            DanhSachLop.Add(new LopHocModel { STT = DanhSachLop.Count + 1, Khoi = "", TenLop = "" });
        }

        [RelayCommand]
        private void ThemMon()
        {
            DanhSachMon.Add(new MonHocModel { STT = DanhSachMon.Count + 1, TenMon = "" });
        }

        // --- LƯU DỮ LIỆU ---
        [RelayCommand]
        private void LuuCaiDat()
        {
            if (MinAge == null || MaxAge == null || MaxClassSize == null || PassingGrade == null)
            {
                NotificationHelper.ShowWarning("Vui lòng nhập đầy đủ các quy định, không được để trống ô nào!");
                return;
            }

            if (MinAge < 0 || MinAge > MaxAge)
            {
                NotificationHelper.ShowWarning("Tuổi tối thiểu phải >= 0 và KHÔNG ĐƯỢC LỚN HƠN Tuổi tối đa!");
                return;
            }
            if (MaxAge > 100)
            {
                NotificationHelper.ShowWarning("Tuổi tối đa không được vượt quá 100!");
                return;
            }
            if (MaxClassSize < 0)
            {
                NotificationHelper.ShowWarning("Sĩ số tối đa không được nhỏ hơn 0!");
                return;
            }
            if (PassingGrade < 0.0 || PassingGrade > 10.0)
            {
                NotificationHelper.ShowWarning("Điểm đạt môn phải nằm trong khoảng từ 0.0 đến 10.0!");
                return;
            }

            try
            {
                List<string> updateQueries = new List<string>
                {
                    $"UPDATE THAMSO SET GiaTri = {MinAge} WHERE MaThamSo = 'MinAge'",
                    $"UPDATE THAMSO SET GiaTri = {MaxAge} WHERE MaThamSo = 'MaxAge'",
                    $"UPDATE THAMSO SET GiaTri = {MaxClassSize} WHERE MaThamSo = 'MaxClassSize'",
                    $"UPDATE THAMSO SET GiaTri = {PassingGrade.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)} WHERE MaThamSo = 'PassingGrade'"
                };

                foreach (var query in updateQueries)
                {
                    DatabaseHelper.ExecuteNonQuery(query);
                }

                QuyDinhService.minAge = MinAge.Value;
                QuyDinhService.maxAge = MaxAge.Value;
                QuyDinhService.maxClassSize = MaxClassSize.Value;
                QuyDinhService.passingGrade = PassingGrade.Value;

                HasUnsavedChanges = false;
                NotificationHelper.ShowSuccess("Đã lưu các quy định tham số thành công!");
            }
            catch (System.Exception ex)
            {
                NotificationHelper.ShowError("Có lỗi xảy ra khi lưu: " + ex.Message);
            }
        }
    }
}