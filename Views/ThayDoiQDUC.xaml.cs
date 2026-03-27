using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_StudentManagement_Project.Services;

namespace WPF_StudentManagement_Project.Views
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

    public partial class ThayDoiQDUC : UserControl
    {
        ObservableCollection<LopHocModel> DanhSachLop = new ObservableCollection<LopHocModel>();
        ObservableCollection<MonHocModel> DanhSachMon = new ObservableCollection<MonHocModel>();

        public ThayDoiQDUC()
        {
            InitializeComponent();

            dgLopHoc.ItemsSource = DanhSachLop;
            dgMonHoc.ItemsSource = DanhSachMon;

            // 1. Nạp quy định từ Database lên biến tĩnh của QuyDinhService
            QuyDinhService.LoadTuDatabase();

            // 2. Đổ dữ liệu từ QuyDinhService ra các ô nhập
            txtTuoiMin.Text = QuyDinhService.minAge.ToString();
            txtTuoiMax.Text = QuyDinhService.maxAge.ToString();
            txtSiSoMax.Text = QuyDinhService.maxClassSize.ToString();
            txtPassScore.Text = QuyDinhService.passingGrade.ToString("0.0");

            // 3. Nạp danh sách lớp từ DB
            try
            {
                DataTable dtLop = DatabaseHelper.ExecuteQuery("SELECT TenLop, Khoi FROM LOP");
                int sttLop = 1;
                foreach (DataRow row in dtLop.Rows)
                {
                    DanhSachLop.Add(new LopHocModel
                    {
                        STT = sttLop++,
                        TenLop = row["TenLop"].ToString(),
                        Khoi = row["Khoi"].ToString()
                    });
                }
            }
            catch { /* Bỏ qua nếu DB chưa có dữ liệu */ }
        }

        // --- LOGIC RÀNG BUỘC CÁC NÚT TĂNG/GIẢM ---

        private void btnTangTuoiMin_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMin.Text, out int value) && int.TryParse(txtTuoiMax.Text, out int max))
            {
                // TuoiMin không được vượt quá TuoiMax
                if (value < max) txtTuoiMin.Text = (value + 1).ToString();
            }
        }

        private void btnGiamTuoiMin_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMin.Text, out int value))
            {
                // TuoiMin >= 0
                if (value > 0) txtTuoiMin.Text = (value - 1).ToString();
            }
        }

        private void btnTangTuoiMax_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMax.Text, out int value))
            {
                // TuoiMax <= 100
                if (value < 100) txtTuoiMax.Text = (value + 1).ToString();
            }
        }

        private void btnGiamTuoiMax_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMax.Text, out int value) && int.TryParse(txtTuoiMin.Text, out int min))
            {
                // TuoiMax không được nhỏ hơn TuoiMin
                if (value > min) txtTuoiMax.Text = (value - 1).ToString();
            }
        }

        private void btnTangSiSo_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtSiSoMax.Text, out int value))
            {
                txtSiSoMax.Text = (value + 1).ToString();
            }
        }

        private void btnGiamSiSo_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtSiSoMax.Text, out int value))
            {
                // Sĩ số >= 0
                if (value > 0) txtSiSoMax.Text = (value - 1).ToString();
            }
        }

        private void btnTangDiem_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtPassScore.Text, out double value))
            {
                // Điểm <= 10.0
                if (value < 10.0) txtPassScore.Text = Math.Round(value + 0.1, 1).ToString("0.0");
            }
        }

        private void btnGiamDiem_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtPassScore.Text, out double value))
            {
                // Điểm >= 0.0
                if (value > 0.0) txtPassScore.Text = Math.Round(value - 0.1, 1).ToString("0.0");
            }
        }

        // --- LOGIC THÊM LỚP/MÔN UI ---
        private void btnThemLop_Click(object sender, RoutedEventArgs e)
        {
            DanhSachLop.Add(new LopHocModel { STT = DanhSachLop.Count + 1, Khoi = "", TenLop = "" });
        }

        private void btnThemMon_Click(object sender, RoutedEventArgs e)
        {
            DanhSachMon.Add(new MonHocModel { STT = DanhSachMon.Count + 1, TenMon = "" });
        }

        // --- CHẶN KÝ TỰ (REGEX) ---
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            if (sender is TextBox textBox && (e.Text == "." || e.Text == ","))
            {
                if (textBox.Text.Contains(".") || textBox.Text.Contains(","))
                {
                    e.Handled = true;
                }
            }
        }

        // --- LƯU DỮ LIỆU ---
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Đề phòng user xóa trống TextBox rồi bấm Lưu
                if (string.IsNullOrWhiteSpace(txtTuoiMin.Text) || string.IsNullOrWhiteSpace(txtTuoiMax.Text) ||
                    string.IsNullOrWhiteSpace(txtSiSoMax.Text) || string.IsNullOrWhiteSpace(txtPassScore.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ các quy định!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int minAge = int.Parse(txtTuoiMin.Text);
                int maxAge = int.Parse(txtTuoiMax.Text);
                int maxClassSize = int.Parse(txtSiSoMax.Text);
                double passScore = double.Parse(txtPassScore.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                // KIỂM TRA RÀNG BUỘC GÕ TAY
                if (minAge < 0 || minAge > maxAge)
                {
                    MessageBox.Show("Tuổi tối thiểu phải >= 0 và KHÔNG ĐƯỢC LỚN HƠN Tuổi tối đa!", "Lỗi Logic", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (maxAge > 100)
                {
                    MessageBox.Show("Tuổi tối đa không được vượt quá 100!", "Lỗi Logic", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (maxClassSize < 0)
                {
                    MessageBox.Show("Sĩ số tối đa không được nhỏ hơn 0!", "Lỗi Logic", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (passScore < 0.0 || passScore > 10.0)
                {
                    MessageBox.Show("Điểm đạt môn phải nằm trong khoảng từ 0.0 đến 10.0!", "Lỗi Logic", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật Database
                List<string> updateQueries = new List<string>
                {
                    $"UPDATE THAMSO SET GiaTri = {minAge} WHERE MaThamSo = 'MinAge'",
                    $"UPDATE THAMSO SET GiaTri = {maxAge} WHERE MaThamSo = 'MaxAge'",
                    $"UPDATE THAMSO SET GiaTri = {maxClassSize} WHERE MaThamSo = 'MaxClassSize'",
                    $"UPDATE THAMSO SET GiaTri = {passScore.ToString(System.Globalization.CultureInfo.InvariantCulture)} WHERE MaThamSo = 'PassingGrade'"
                };

                foreach (var query in updateQueries)
                {
                    DatabaseHelper.ExecuteNonQuery(query);
                }

                // Cập nhật lại biến tĩnh dùng chung trong RAM (Để các form khác gọi xài luôn mà không cần query lại DB)
                QuyDinhService.minAge = minAge;
                QuyDinhService.maxAge = maxAge;
                QuyDinhService.maxClassSize = maxClassSize;
                QuyDinhService.passingGrade = passScore;

                MessageBox.Show("Đã lưu các quy định tham số thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu: " + ex.Message, "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}