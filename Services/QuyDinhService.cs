using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    //class tĩnh để lưu các giá trị có thể thay đổi trong QĐ 6
    public static class QuyDinhService
    {
        //tạm thời gán cứng, sau nâng cấp lên cho đọc file DB
        public static int minTuoi = 15;
        public static int maxTuoi = 20;
        public static int maxSiSo = 40;
        public static int maxSoLop = 9;
        public static int maxMonHoc = 9;
        public static double DiemDat = 5.0; 
        // Nhớ auto-start, review app.xaml.cs  
        /// <summary>
        /// Tải các quy định/tham số từ Database lên dùng toàn cục.
        /// </summary>
        public static void LoadTuDatabase()
        {
            string query = "SELECT MaThamSo, GiaTri FROM THAMSO";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                string maThamSo = row["MaThamSo"]?.ToString() ?? "";

                // GiaTri trong DB là kiểu số thực (FLOAT/DECIMAL), nên ta ép kiểu an toàn về double
                double giaTri = row["GiaTri"] != DBNull.Value ? Convert.ToDouble(row["GiaTri"]) : 0;

                switch (maThamSo)
                {
                    case "minTuoi":
                        minTuoi = (int)giaTri;
                        break;

                    case "maxTuoi":
                        maxTuoi = (int)giaTri;
                        break;

                    case "maxSiSo":
                        maxSiSo = (int)giaTri;
                        break;

                    case "maxSoLop":
                        maxSoLop = (int)giaTri;
                        break;

                    case "maxMonHoc":
                        maxMonHoc = (int)giaTri;
                        break;

                    case "DiemDat":
                        DiemDat = giaTri;
                        break;
                }
            }
        }
    }
}
