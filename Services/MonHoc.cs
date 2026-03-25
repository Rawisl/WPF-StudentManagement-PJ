using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    internal class MonHoc
    {
        public required string MaMonHoc { get; set; }
        public string? TenMonHoc { get; set; }
        /// <summary>
        /// Lấy toàn bộ danh sách môn học từ CSDL.
        /// </summary>
        /// <returns>Danh sách đối tượng MonHoc</returns>
        /// <remarks>
        /// HƯỚNG DẪN CHO FORM DESIGNER (WPF):
        /// Khi gọi hàm này để đổ dữ liệu vào ComboBox, hãy nhớ cấu hình 2 thuộc tính sau:
        /// 1. DisplayMemberPath = "TenMonHoc" (Để hiển thị chữ 'Toán', 'Lý' cho User xem)
        /// 2. SelectedValuePath = "MaMonHoc" (Để lấy mã 'MH01' lưu xuống Database)
        /// </remarks>
        /// <example>
        /// cbMonHoc.ItemsSource = MonHoc.LayDanhSach();
        /// cbMonHoc.DisplayMemberPath = "TenMonHoc";
        /// cbMonHoc.SelectedValuePath = "MaMonHoc";
        /// </example>
        public static List<MonHoc> LayDanhSach()
        {
            List<MonHoc> danhSach = new List<MonHoc>();
            string query = "SELECT MaMonHoc, TenMonHoc FROM MONHOC";

            DataTable data = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                MonHoc mh = new MonHoc()
                {
                    MaMonHoc = row["MaMonHoc"].ToString() ?? "",
                    TenMonHoc = row["TenMonHoc"].ToString() ?? ""
                };
                danhSach.Add(mh);
            }
            return danhSach;
        }
    }
}
