using System.Configuration;
using System.Data;
using System.Windows;
using WPF_StudentManagement_Project.Services;
using Microsoft.Data.SqlClient;

namespace WPF_StudentManagement_Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Nạp tất cả quy định từ Database ngay khi app vừa khởi chạy
                QuyDinhService.LoadTuDatabase();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi khi tải tham số quy định từ CSDL: " + ex.Message,
                                "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
