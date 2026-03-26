using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    public class DiemSo
    {
        public required int MaDiemSo { get; set; }
        // public required string MaHocSinh { get; set; }
        public required string MaMonHoc { get; set; }
        // public int HocKy { get; set; }
        public double Diem15p { get; set; }
        public double Diem1Tiet { get; set; }
        public double DiemTB { get; set; }
        public int MaPhanLop { get; set; }

        public static bool LuuDiem(string maHocSinh, string maMonHoc, int hocKy, string namHoc, double diem15p, double diem1Tiet)
        {
            // Xử lý logic check điều kiện Điểm 0 -> 10
            if (diem15p < 0 || diem15p > 10 || diem1Tiet < 0 || diem1Tiet > 10)
            {
                throw new ArgumentException("Điểm phải nằm trong khoảng từ 0 đến 10.");
            }

            // Tìm MaPhanLop dựa trên học sinh, học kỳ và năm học
            string findPlQuery = "SELECT MaPhanLop FROM PHANLOP WHERE MaHocSinh = @MaHocSinh AND HocKy = @HocKy AND NamHoc = @NamHoc";
            SqlParameter[] plParams = {
                new SqlParameter("@MaHocSinh", maHocSinh),
                new SqlParameter("@HocKy", hocKy),
                new SqlParameter("@NamHoc", namHoc)
            };
            DataTable dtPhanLop = DatabaseHelper.ExecuteQuery(findPlQuery, plParams);

            if (dtPhanLop.Rows.Count == 0)
            {
                throw new Exception("Học sinh này chưa được phân lớp trong học kỳ/năm học đã chọn.");
            }
            int maPhanLop = Convert.ToInt32(dtPhanLop.Rows[0]["MaPhanLop"]);

            // Kiểm tra xem đã có điểm cho môn này trong phân lớp này chưa
            string checkDiemQuery = "SELECT MaDiemSo FROM DIEMSO WHERE MaPhanLop = @MaPhanLop AND MaMonHoc = @MaMonHoc";
            SqlParameter[] checkParams = {
                new SqlParameter("@MaPhanLop", maPhanLop),
                new SqlParameter("@MaMonHoc", maMonHoc)
            };
            DataTable dtDiem = DatabaseHelper.ExecuteQuery(checkDiemQuery, checkParams);

            if (dtDiem.Rows.Count > 0)
            {
                // Đã có -> Cập nhật (UPDATE)
                string updateQuery = "UPDATE DIEMSO SET Diem15p = @Diem15p , Diem1Tiet = @Diem1Tiet WHERE MaPhanLop = @MaPhanLop AND MaMonHoc = @MaMonHoc";
                SqlParameter[] updateParams = {
                    new SqlParameter("@Diem15p", diem15p),
                    new SqlParameter("@Diem1Tiet", diem1Tiet),
                    new SqlParameter("@MaPhanLop", maPhanLop),
                    new SqlParameter("@MaMonHoc", maMonHoc)
                };
                return DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams) > 0;
            }
            else
            {
                // Chưa có -> Thêm mới (INSERT)
                string insertQuery = "INSERT INTO DIEMSO (MaPhanLop, MaMonHoc, Diem15p, Diem1Tiet) VALUES ( @MaPhanLop , @MaMonHoc , @Diem15p , @Diem1Tiet )";
                SqlParameter[] insertParams = {
                    new SqlParameter("@MaPhanLop", maPhanLop),
                    new SqlParameter("@MaMonHoc", maMonHoc),
                    new SqlParameter("@Diem15p", diem15p),
                    new SqlParameter("@Diem1Tiet", diem1Tiet)
                };
                return DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams) > 0;
            }
        }

    }
}