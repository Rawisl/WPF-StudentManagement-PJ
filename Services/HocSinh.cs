using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    internal class HocSinh
    {
        public required string MaHocSinh { get; set; }
        public string? HoTen { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? DiaChi { get; set; }
        public string? Email { get; set; }
        public required string MaLop { get; set; }
    }
}
