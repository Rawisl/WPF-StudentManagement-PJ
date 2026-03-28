using WPF_StudentManagement_Project.Views;

namespace WPF_StudentManagement_Project.Services // Hoặc Helpers tùy bro
{
    public static class NotificationHelper
    {
        // 1. Thông báo LỖI (Màu đỏ)
        public static void ShowError(string message)
        {
            var msgBox = new MaterialMessageBox("Lỗi", message, MsgType.Error);
            msgBox.ShowDialog();
        }

        // 2. Thông báo THÀNH CÔNG (Màu xanh lá)
        public static void ShowSuccess(string message)
        {
            var msgBox = new MaterialMessageBox("Thành công", message, MsgType.Success);
            msgBox.ShowDialog();
        }

        // 3. Thông báo CẢNH BÁO (Màu cam)
        public static void ShowWarning(string message)
        {
            var msgBox = new MaterialMessageBox("Cảnh báo", message, MsgType.Warning);
            msgBox.ShowDialog();
        }

        // 4. Hộp thoại XÁC NHẬN (Có nút Yes/No - Trả về true/false)
        public static bool ShowConfirm(string message)
        {
            var msgBox = new MaterialMessageBox("Xác nhận", message, MsgType.Confirm);
            msgBox.ShowDialog();
            return msgBox.Result; // Trả về true nếu bấm OK, false nếu bấm Hủy
        }
    }
}