using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    internal class DiemSo
    {
        public required int MaDiemSo { get; set; }
        public required string MaHocSinh { get; set; }
        public required string MaMonHoc { get; set; }
        public int HocKy { get; set; }
        public double Diem15p { get; set; }
        public double Diem1Tiet { get; set; }
        public double DiemTB { get; set; }
    }
}
