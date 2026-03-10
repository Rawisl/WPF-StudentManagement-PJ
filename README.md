## 📂 Cấu trúc dự án (MVVM)
- **Models**: Chứa các lớp thực thể dữ liệu (HocSinh, Lop,...) - Định dạng .cs (class thông thường)
- **ViewModels**: Xử lý logic nghiệp vụ và liên kết dữ liệu (Data Binding) - Định dạng .cs (class sử dụng thư viện CommunityToolkit)
- **Views**: Giao diện XAML (UserControls cho từng biểu mẫu BM) - Định dạng .xaml, .xaml.cs (cặp file giao diện)
- **Services**: Các lớp kết nối Database SQL Server - Định dạng .cs (file xử lý trực tiếp DB)
- **Resources**: Chứa Style Material Design và các icon của ứng dụng - Định dạng .xaml (Resouce Dictionary), .png, .svg,..
