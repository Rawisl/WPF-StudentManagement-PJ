using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_StudentManagement_Project.Services
{
    internal class ThamSo
    {
        public required string MaThamSo { get; set; }
        public string? TenThamSo { get; set; } // Use '?' if it's okay to be null
        public double GiaTri { get; set; }
    }
}
