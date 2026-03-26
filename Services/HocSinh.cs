using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    public class HocSinh
    {
        public required string MaHocSinh { get; set; }
        public required string HoTen { get; set; }
        public required string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? DiaChi { get; set; }
        public string? Email { get; set; }
        // public required string MaLop { get; set; } DB structure changed

        // READ:
        public static List<HocSinh> LayDanhSach()
        {
            List<HocSinh> danhSach = new List<HocSinh>();
            string query = "SELECT * FROM HocSinh";

            DataTable data = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                HocSinh hs = new HocSinh()
                {
                    MaHocSinh = row["MaHocSinh"] as string ?? "",
                    // MaLop = row["MaLop"] as string ?? "",
                    HoTen = row["HoTen"] as string ?? "",
                    GioiTinh = row["GioiTinh"] as string ?? "",
                    DiaChi = row["DiaChi"] as string,
                    Email = row["Email"] as string,
                    NgaySinh = row["NgaySinh"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NgaySinh"])
                };
                danhSach.Add(hs);
            }
            return danhSach;
        }

        // CREATE:
        public bool Them()
        {
            string query = "INSERT INTO HocSinh (MaHocSinh, HoTen, GioiTinh, NgaySinh, DiaChi, Email) " +
                           "VALUES ( @MaHocSinh , @HoTen , @GioiTinh , @NgaySinh , @DiaChi , @Email)";

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@MaHocSinh", this.MaHocSinh),
                new SqlParameter("@HoTen", this.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@GioiTinh", this.GioiTinh ?? (object)DBNull.Value),
                new SqlParameter("@NgaySinh", this.NgaySinh),
                new SqlParameter("@DiaChi", this.DiaChi ?? (object)DBNull.Value),
                new SqlParameter("@Email", this.Email ?? (object)DBNull.Value)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        // UPDATE:
        public bool Sua()
        {
            string query = "UPDATE HocSinh SET HoTen = @HoTen , GioiTinh = @GioiTinh , " +
                           "NgaySinh = @NgaySinh , DiaChi = @DiaChi , Email = @Email " +
                           "WHERE MaHocSinh = @MaHocSinh";

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@MaHocSinh", this.MaHocSinh),
                new SqlParameter("@HoTen", this.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@GioiTinh", this.GioiTinh ?? (object)DBNull.Value),
                new SqlParameter("@NgaySinh", this.NgaySinh),
                new SqlParameter("@DiaChi", this.DiaChi ?? (object)DBNull.Value),
                new SqlParameter("@Email", this.Email ?? (object)DBNull.Value)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        // DELETE:
        public static bool Xoa(string maHocSinh)
        {
            string query = "DELETE FROM HocSinh WHERE MaHocSinh = @MaHocSinh";
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@MaHocSinh", maHocSinh)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}