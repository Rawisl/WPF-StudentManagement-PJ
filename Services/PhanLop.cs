using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    internal class PhanLop
    {
        public int MaPhanLop { get; set; } // IDENTITY column, usually not 'required' for inserts
        public required string MaHocSinh { get; set; }
        public required string MaLop { get; set; }
        public int HocKy { get; set; }
        public required string NamHoc { get; set; }
    }
}
