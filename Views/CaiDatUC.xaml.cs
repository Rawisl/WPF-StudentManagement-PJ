using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

    /// <summary>
    /// Interaction logic for CaiDatUC.xaml
    /// </summary>
    public partial class CaiDatUC : UserControl
    {

        ObservableCollection<LopHocModel> DanhSachLop = new ObservableCollection<LopHocModel>();
        ObservableCollection<MonHocModel> DanhSachMon = new ObservableCollection<MonHocModel>();

        public CaiDatUC()
        {
            InitializeComponent();

            dgLopHoc.ItemsSource = DanhSachLop;
            dgMonHoc.ItemsSource = DanhSachMon;
        }

        private void btnTangTuoiMin_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMin.Text, out int value))
            {
                if (int.TryParse(txtTuoiMax.Text, out int max) && value >= max) return;

                txtTuoiMin.Text = (value + 1).ToString();
            }
        }

        private void btnTangTuoiMax_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMax.Text, out int value))
            {
                txtTuoiMax.Text = (value + 1).ToString();
            }
        }

        private void btnGiamTuoiMin_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMin.Text, out int value) && value > 0)
            {
                txtTuoiMin.Text = (value - 1).ToString();
            }
        }

        private void btnGiamTuoiMax_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTuoiMax.Text, out int value))
            {
                if (int.TryParse(txtTuoiMin.Text, out int min) && value <= min) return;

                txtTuoiMax.Text = (value - 1).ToString();
            }
        }

        private void btnTangSiSo_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtSiSoMax.Text, out int value))
            {
                txtSiSoMax.Text = (value + 1).ToString();
            }
        }

        private void btnTangDiem_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtPassScore.Text, out double value))
            {
                if (value < 10.0)
                {
                    txtPassScore.Text = Math.Round(value + 0.1, 1).ToString("0.0");
                }
            }
        }

        private void btnGiamSiSo_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtSiSoMax.Text, out int value) && value > 0)
            {
                txtSiSoMax.Text = (value - 1).ToString();
            }
        }

        private void btnGiamDiem_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtPassScore.Text, out double value))
            {
                if (value > 0.0)
                {
                    txtPassScore.Text = Math.Round(value - 0.1, 1).ToString("0.0");
                }
            }
        }

        private void btnThemLop_Click(object sender, RoutedEventArgs e)
        {
            DanhSachLop.Add(new LopHocModel
            {
                STT = DanhSachLop.Count + 1,
                Khoi = "",
                TenLop = ""
            });
        }

        private void btnThemMon_Click(object sender, RoutedEventArgs e)
        {
            DanhSachMon.Add(new MonHocModel
            {
                STT = DanhSachMon.Count + 1,
                TenMon = ""
            });
        }
    }
}
