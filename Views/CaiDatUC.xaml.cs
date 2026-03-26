using Microsoft.Data.SqlClient;
using System;
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
        public string Khoi { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
        public string XoaIcon { get; set; } = "X";
    }

    public class MonHocModel
    {
        public int STT { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public string XoaIcon { get; set; } = "X";
    }

    public partial class CaiDatUC : UserControl
    {
        ObservableCollection<LopHocModel> DanhSachLop = new ObservableCollection<LopHocModel>();
        ObservableCollection<MonHocModel> DanhSachMon = new ObservableCollection<MonHocModel>();

        public CaiDatUC()
        {
            InitializeComponent();
            dgLopHoc.ItemsSource = DanhSachLop;
            dgMonHoc.ItemsSource = DanhSachMon;

            // Gọi ThamSo.LayDanhSach() để lấy dữ liệu từ DB đổ lên TextBox khi vừa mở form
            txtTuoiMin.Text = QuyDinhService.minTuoi.ToString();
            txtTuoiMax.Text = QuyDinhService.maxTuoi.ToString();
            txtSiSoMax.Text = QuyDinhService.maxSiSo.ToString();
            txtPassScore.Text = QuyDinhService.DiemDat.ToString("0.0");

            // Nạp danh sách lớp từ DB
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

            // Làm tương tự với môn học...
        }

        // LOGIC RÀNG BUỘC CÁC NÚT TĂNG/GIẢM ---

        private void btnTangTuoiMin_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMin.Text, out int value) && int.TryParse(txtTuoiMax.Text, out int max))
            {
                if (value < max && value < 100) txtTuoiMin.Text = (value + 1).ToString();
            }
        }

        private void btnGiamTuoiMin_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMin.Text, out int value))
            {
                if (value > 0) txtTuoiMin.Text = (value - 1).ToString();
            }
        }

        private void btnTangTuoiMax_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMax.Text, out int value))
            {
                if (value < 100) txtTuoiMax.Text = (value + 1).ToString();
            }
        }

        private void btnGiamTuoiMax_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMax.Text, out int value) && int.TryParse(txtTuoiMin.Text, out int min))
            {
                if (value > min && value > 0) txtTuoiMax.Text = (value - 1).ToString();
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
                if (value > 0) txtSiSoMax.Text = (value - 1).ToString();
            }
        }

        private void btnTangDiem_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtPassScore.Text, out double value))
            {
                if (value < 10.0) txtPassScore.Text = Math.Round(value + 0.1, 1).ToString("0.0");
            }
        }

        private void btnGiamDiem_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtPassScore.Text, out double value))
            {
                if (value > 0.0) txtPassScore.Text = Math.Round(value - 0.1, 1).ToString("0.0");
            }
        }

        // LOGIC THÊM LỚP/MÔN (Tạm thời hiển thị UI) ---
        private void btnThemLop_Click(object sender, RoutedEventArgs e)
        {
            DanhSachLop.Add(new LopHocModel { STT = DanhSachLop.Count + 1, Khoi = "", TenLop = "" });
        }

        private void btnThemMon_Click(object sender, RoutedEventArgs e)
        {
            DanhSachMon.Add(new MonHocModel { STT = DanhSachMon.Count + 1, TenMon = "" });
        }

        // CHẶN KÝ TỰ CHỮ ---
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            // Ngăn người dùng gõ 2 dấu chấm (Ví dụ: 5.5.5 -> Chặn dấu chấm thứ 2)
            if (sender is TextBox textBox && (e.Text == "." || e.Text == ","))
            {
                if (textBox.Text.Contains(".") || textBox.Text.Contains(","))
                {
                    e.Handled = true;
                }
            }
        }

        // LƯU DỮ LIỆU XUỐNG RAM & DATABASE ---
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Bước 4.1: Parse và Validate dữ liệu gõ tay (Đề phòng user gõ "999")
            if (!int.TryParse(txtTuoiMin.Text, out int minTuoi) ||
                !int.TryParse(txtTuoiMax.Text, out int maxTuoi) ||
                !int.TryParse(txtSiSoMax.Text, out int siSo) ||
                !double.TryParse(txtPassScore.Text, out double diemDat))
            {
                MessageBox.Show("Vui lòng nhập số hợp lệ vào các ô quy định!", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra Logic theo tiêu chí của bạn
            if (minTuoi < 0 || maxTuoi > 100 || minTuoi > maxTuoi)
            {
                MessageBox.Show("Quy định Tuổi không hợp lệ!\n(Từ 0 đến 100, Tuổi Min phải <= Tuổi Max)", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (siSo < 0)
            {
                MessageBox.Show("Sĩ số tối đa không được nhỏ hơn 0!", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (diemDat < 0.0 || diemDat > 10.0)
            {
                MessageBox.Show("Điểm đạt phải nằm trong khoảng từ 0.0 đến 10.0!", "Lỗi logic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Lưu vào biến RAM của QuyDinhService
            QuyDinhService.minTuoi = minTuoi;
            QuyDinhService.maxTuoi = maxTuoi;
            QuyDinhService.maxSiSo = siSo;
            QuyDinhService.DiemDat = diemDat;

            //Lưu xuống Database qua DatabaseHelper
            try
            {
                // Cần thay thế các mã 'TS01', 'TS02'... bên dưới 
                // cho khớp với cột MaThamSo hoặc TenThamSo thực tế trong CSDL

                string query = "UPDATE THAMSO SET GiaTri = @GiaTri WHERE MaThamSo = @MaThamSo";

                // Cập nhật Tuổi tối thiểu
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@GiaTri", minTuoi),
                    new SqlParameter("@MaThamSo", "MinAge")
                });

                // Cập nhật Tuổi tối đa
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@GiaTri", maxTuoi),
                    new SqlParameter("@MaThamSo", "MaxAge")
                });

                // Cập nhật Sĩ số
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@GiaTri", siSo),
                    new SqlParameter("@MaThamSo", "MaxClassSize")
                });

                // Cập nhật Điểm đạt
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@GiaTri", diemDat),
                    new SqlParameter("@MaThamSo", "PassingGrade")
                });

                MessageBox.Show("Cập nhật quy định hệ thống thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật CSDL:\n{ex.Message}", "Lỗi nghiêm trọng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}