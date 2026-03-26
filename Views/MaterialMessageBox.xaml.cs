using System.Windows;
using System.Windows.Media;

namespace WPF_StudentManagement_Project.Views
{
    public enum MsgType { Info, Success, Warning, Error, Confirm }

    public partial class MaterialMessageBox : Window
    {
        public bool Result { get; private set; } = false;

        public MaterialMessageBox(string title, string message, MsgType type)
        {
            InitializeComponent();
            txtTitle.Text = title.ToUpper();
            txtMessage.Text = message;

            // Đổi màu theo loại thông báo chuẩn Material Colors
            switch (type)
            {
                case MsgType.Success:
                    HeaderBorder.Background = (Brush)new BrushConverter().ConvertFrom("#4CAF50"); // Xanh lá
                    btnOK.Background = (Brush)new BrushConverter().ConvertFrom("#4CAF50");
                    break;
                case MsgType.Error:
                    HeaderBorder.Background = (Brush)new BrushConverter().ConvertFrom("#F44336"); // Đỏ
                    btnOK.Background = (Brush)new BrushConverter().ConvertFrom("#F44336");
                    break;
                case MsgType.Warning:
                    HeaderBorder.Background = (Brush)new BrushConverter().ConvertFrom("#FF9800"); // Cam
                    btnOK.Background = (Brush)new BrushConverter().ConvertFrom("#FF9800");
                    break;
                case MsgType.Confirm:
                    HeaderBorder.Background = (Brush)new BrushConverter().ConvertFrom("#2196F3"); // Xanh dương
                    btnOK.Background = (Brush)new BrushConverter().ConvertFrom("#2196F3");
                    btnCancel.Visibility = Visibility.Visible; // Hiện nút Hủy
                    break;
                default: // Info
                    HeaderBorder.Background = (Brush)new BrushConverter().ConvertFrom("#00BCD4"); // Cyan
                    btnOK.Background = (Brush)new BrushConverter().ConvertFrom("#00BCD4");
                    break;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.Close();
        }
    }
}