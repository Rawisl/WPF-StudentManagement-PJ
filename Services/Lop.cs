using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    internal class Lop
    {
        public required string MaLop { get; set; }
        public string? TenLop { get; set; }
        public int Khoi { get; set; }
        public int SiSo { get; set; }

        // 1. READ
        public static List<Lop> LayDanhSach() {
            List<Lop> danhSach = new List<Lop>();
            string query = "SELECT * FROM Lop";

            DataTable data = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in data.Rows) {
                Lop lop = new Lop() {
                    // Xử lý null
                    MaLop = row["MaLop"]?.ToString() ?? "",
                    TenLop = row["TenLop"] == DBNull.Value ? null : row["TenLop"]?.ToString(),
                    Khoi = row["Khoi"] == DBNull.Value ? 0 : Convert.ToInt32(row["Khoi"]),
                    SiSo = row["SiSo"] == DBNull.Value ? 0 : Convert.ToInt32(row["SiSo"])
                };
                danhSach.Add(lop);
            }
            return danhSach;
        }

        // 2. CREATE
        public bool Them() {
            string query = "INSERT INTO Lop (MaLop, TenLop, Khoi, SiSo) " +
                           "VALUES ( @MaLop , @TenLop , @Khoi , @SiSo )";

            object[] parameters = {
                this.MaLop,
                this.TenLop ?? (object)DBNull.Value,
                this.Khoi,
                this.SiSo
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        // 3. UPDATE
        public bool Sua() {
            string query = "UPDATE Lop SET TenLop = @TenLop , Khoi = @Khoi , SiSo = @SiSo " +
                           "WHERE MaLop = @MaLop";

            object[] parameters = {
                this.TenLop ?? (object)DBNull.Value,
                this.Khoi,
                this.SiSo,
                this.MaLop
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        // 4. DELETE
        public static bool Xoa(string maLop) {
            string query = "DELETE FROM Lop WHERE MaLop = @MaLop";
            object[] parameters = { maLop };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
