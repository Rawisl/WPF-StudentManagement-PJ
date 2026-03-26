using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WPF_StudentManagement_Project.Services;
using Microsoft.Data.SqlClient;

namespace WPF_StudentManagement_Project.ViewModels
{
    public partial class ThayDoiQDViewModel : ObservableObject
    {
        // 1. Cờ đánh dấu có thay đổi
        private bool _isLoaded = false; // Cờ này để chặn OnChanged chạy lúc vừa load dữ liệu

        [ObservableProperty]
        private bool _hasUnsavedChanges = false;

        [ObservableProperty] private int _minAge;
        [ObservableProperty] private int _maxAge;
        [ObservableProperty] private int _maxClassSize;
        [ObservableProperty] private double _passingGrade;

        public ThayDoiQDViewModel()
        {
            LoadDataFromService();
        }

        private void LoadDataFromService()
        {
            _isLoaded = false; // Đang load, đừng tính là "User sửa"

            MinAge = QuyDinhService.minAge;
            MaxAge = QuyDinhService.maxAge;
            MaxClassSize = QuyDinhService.maxClassSize;
            PassingGrade = QuyDinhService.passingGrade;

            _isLoaded = true; // Load xong rồi, giờ user động vào mới tính
            HasUnsavedChanges = false;
        }

        // Tự động chạy khi gõ số mới. Chỉ bật HasUnsavedChanges khi đã load xong
        partial void OnMinAgeChanged(int value) => MarkDirty();
        partial void OnMaxAgeChanged(int value) => MarkDirty();
        partial void OnMaxClassSizeChanged(int value) => MarkDirty();
        partial void OnPassingGradeChanged(double value) => MarkDirty();

        private void MarkDirty()
        {
            if (_isLoaded) HasUnsavedChanges = true;
        }

        [RelayCommand]
        private void LuuCaiDat()
        {
            // Thay vì Parse từ TextBox thì check trực tiếp các thuộc tính đã Binding
            // Vì Binding tự ép kiểu nên MinAge, MaxAge chắc chắn là số 

            // Kiểm tra Logic có hợp lý không
            if (MinAge < 0 || MaxAge > 100 || MinAge > MaxAge)
            {
                MessageBox.Show("Quy định Tuổi không hợp lệ!\n(Từ 0 đến 100, Tuổi Min phải <= Tuổi Max)", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MaxClassSize < 0)
            {
                MessageBox.Show("Sĩ số tối đa không được nhỏ hơn 0!", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (PassingGrade < 0.0 || PassingGrade > 10.0)
            {
                MessageBox.Show("Điểm đạt phải nằm trong khoảng từ 0.0 đến 10.0!", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            try
            {
                string query = "UPDATE THAMSO SET GiaTri = @GiaTri WHERE MaThamSo = @MaThamSo";
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@GiaTri", MinAge), new SqlParameter("@MaThamSo", "MinAge") });
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@GiaTri", MaxAge), new SqlParameter("@MaThamSo", "MaxAge") });
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@GiaTri", MaxClassSize), new SqlParameter("@MaThamSo", "MaxClassSize") });
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@GiaTri", PassingGrade), new SqlParameter("@MaThamSo", "PassingGrade") });

                // Lưu xong thì cập nhật lại RAM
                QuyDinhService.minAge = MinAge;
                QuyDinhService.maxAge = MaxAge;
                QuyDinhService.maxClassSize = MaxClassSize;
                QuyDinhService.passingGrade = PassingGrade;

                HasUnsavedChanges = false;
                MessageBox.Show("Lưu quy định thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi lưu: " + ex.Message);
            }
        }
    }
}