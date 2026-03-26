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

    public partial class ThayDoiQDUC : UserControl
    {
        ObservableCollection<LopHocModel> DanhSachLop = new ObservableCollection<LopHocModel>();
        ObservableCollection<MonHocModel> DanhSachMon = new ObservableCollection<MonHocModel>();

        public ThayDoiQDUC()
        {
            InitializeComponent();

            dgLopHoc.ItemsSource = DanhSachLop;
            dgMonHoc.ItemsSource = DanhSachMon;

            // Gọi ThamSo.LayDanhSach() để lấy dữ liệu từ DB đổ lên TextBox khi vừa mở form
            //txtTuoiMin.Text = QuyDinhService.minAge.ToString();
            //txtTuoiMax.Text = QuyDinhService.maxAge.ToString();
            //txtSiSoMax.Text = QuyDinhService.maxClassSize.ToString();
            //txtPassScore.Text = QuyDinhService.passingGrade.ToString("0.0");

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
    }
}