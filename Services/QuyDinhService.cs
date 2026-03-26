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
        public static int minAge = 15;
        public static int maxAge = 20;
        public static int maxClassSize = 40;
        public static int maxClass = 9;
        public static int maxSubject = 9;
        public static double passingGrade = 5.0;
        // Nhớ auto-start, review app.xaml.cs  
        /// <summary>
        /// Tải các quy định/tham số từ Database lên dùng toàn cục.
        /// </summary>
        public static void LoadTuDatabase()
        {
            string query = "SELECT MaThamSo, GiaTri FROM THAMSO";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            // THÊM DÒNG NÀY ĐỂ TRÁNH CRASH APP:
            if (dt == null) return;
            foreach (DataRow row in dt.Rows)
            {
                string maThamSo = row["MaThamSo"]?.ToString() ?? "";

                // GiaTri trong DB là kiểu số thực (FLOAT/DECIMAL), nên ta ép kiểu an toàn về double
                double giaTri = row["GiaTri"] != DBNull.Value ? Convert.ToDouble(row["GiaTri"]) : 0;

                switch (maThamSo)
                {
                    case "MinAge":
                        minAge = (int)giaTri;
                        break;

                    case "MaxAge":
                        maxAge = (int)giaTri;
                        break;

                    case "MaxClassSize":
                        maxClassSize = (int)giaTri;
                        break;

                    case "maxSoLop":
                        maxClass = (int)giaTri;
                        break;

                    case "maxMonHoc":
                        maxSubject = (int)giaTri;
                        break;

                    case "PassingGrade":
                        passingGrade = giaTri;
                        break;
                }
            }
        }
    }
}