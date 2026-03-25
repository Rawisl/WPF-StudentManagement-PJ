using System;
using System.Collections.Generic;
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
    }
}
