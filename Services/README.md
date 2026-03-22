## Nguyên tắc của các lệnh tác động lên CSDL
- Query: Là câu lệnh SQL có các chỗ trống (ký hiệu bằng chữ @). Ví dụ: SELECT * FROM LOP WHERE Khoi = @Khoi.
- Tham số: Là các giá trị thực tế bỏ vào chỗ trống đó (ví dụ: số 10).

Kết hợp lại: SELECT * FROM WHERE Khoi = 10

Nguyên tắc: "Cứ thấy dấu @ trong Query, thì phải chuẩn bị một giá trị tương ứng để điền vào."

## Quy trình liên kết cho các control:
Nhớ **``using WPF_StudentManagement_Project.Services;``** nếu control có dùng DB. 
#### Khi làm giao diện (WPF), cần dùng database "Lưu" hoặc "Tìm kiếm", luôn làm đúng 3 bước:
1. Viết sẵn câu Query mẫu có chứa các @parameter. (SELECT, INSERT, UPDATE, DELETE,...)
2. Lấy giá trị từ TextBox/ComboBox trên giao diện rồi bỏ vào một cái mảng (object[]). # Tạo thành 1 câu lệnh hoàn chỉnh
3. Gọi hàm Execute của DatabaseHelper để máy tự nối lệnh và gửi đi.

## Ví dụ minh họa cách sử dụng:
### 1. Hàm ExecuteQuery (Dùng cho lệnh SELECT - Lấy dữ liệu)
**Hàm này trả về một DataTable, phù hợp để đổ dữ liệu vào DataGrid hoặc ListView trong WPF.**

> Ví dụ 1.1: Lấy danh sách học sinh theo Mã Lớp

```
// Câu lệnh SQL: Lấy thông tin học sinh thuộc lớp cụ thể
// Lưu ý: Có khoảng trắng trước hàm @MaLop
string sqlGetStudents = "SELECT MaHocSinh, HoTen, GioiTinh, NgaySinh, Email FROM HOCSINH WHERE MaLop = @MaLop";

// Truyền giá trị: Lấy học sinh lớp 10A1 (Mã: '101')
object[] parameters = { "101" };

// Gọi hàm
DataTable dtHocSinh = DatabaseHelper.ExecuteQuery(sqlGetStudents, parameters);

// In kết quả ra Console (Hoặc gán vào DataGrid.ItemsSource = dtHocSinh.DefaultView)
foreach (DataRow row in dtHocSinh.Rows)
{
    Console.WriteLine($"Mã HS: {row["MaHocSinh"]} - Tên: {row["HoTen"]} - Ngày sinh: {Convert.ToDateTime(row["NgaySinh"]):dd/MM/yyyy}");
}
```
> Ví dụ 1.2: Lấy bảng điểm của một học sinh trong học kỳ 1

```
string sqlGetGrades = @"
    SELECT M.TenMonHoc, D.Diem15p, D.Diem1Tiet, D.DiemTB 
    FROM DIEMSO D 
    JOIN MONHOC M ON D.MaMonHoc = M.MaMonHoc 
    WHERE D.MaHocSinh = @MaHocSinh AND D.HocKy = @HocKy";

object[] gradeParams = { "HS001", 1 };

DataTable dtDiemSo = DatabaseHelper.ExecuteQuery(sqlGetGrades, gradeParams);
```
### 2. Hàm ExecuteNonQuery (Dùng cho INSERT, UPDATE, DELETE)
**Hàm này trả về số nguyên (int) đại diện cho số dòng bị tác động trong CSDL.**
> Ví dụ 2.1: Thêm một học sinh mới (INSERT)
```
// Chuỗi SQL có khoảng trắng cẩn thận quanh các @tham_số, nếu query có xuống dòng thì bao @ bên ngoài query
string sqlInsertHS = @"
    INSERT INTO HOCSINH (MaHocSinh, HoTen, GioiTinh, NgaySinh, DiaChi, Email, MaLop) 
    VALUES ( @MaHocSinh , @HoTen , @GioiTinh , @NgaySinh , @DiaChi , @Email , @MaLop )";

// Tạo mảng giá trị tương ứng ĐÚNG THỨ TỰ với các @tham_số ở trên
object[] insertParams = {
    "HS001", 
    "Nguyễn Văn A", 
    "Nam", 
    new DateTime(2008, 5, 15), // Lưu ý trigger độ tuổi sẽ kiểm tra ngày sinh này
    "123 Lê Lợi, Quận 1", 
    "nva@gmail.com", 
    "101" // Vào lớp 10A1. Trigger TRG_UpdateSiSo sẽ tự động tăng sĩ số.
};

try
{
    int rowsAffected = DatabaseHelper.ExecuteNonQuery(sqlInsertHS, insertParams);
    if (rowsAffected > 0)
    {
        Console.WriteLine("Thêm học sinh thành công!");
    }
}
catch (SqlException ex)
{
    // Bắt lỗi nếu Trigger RAISERROR (ví dụ: Lớp đầy, hoặc Sai tuổi)
    Console.WriteLine($"Lỗi từ CSDL: {ex.Message}");
}
```
> Ví dụ 2.2: Cập nhật điểm số cho học sinh (UPDATE)

```
// Cập nhật điểm 15p và 1 tiết. Trigger TRG_TinhDiemTB sẽ tự tính lại DiemTB.
string sqlUpdateDiem = "UPDATE DIEMSO SET Diem15p = @Diem15p , Diem1Tiet = @Diem1Tiet WHERE MaDiemSo = @MaDiemSo";

object[] updateParams = { 8.5, 9.0, 1 }; // MaDiemSo = 1

int updatedRows = DatabaseHelper.ExecuteNonQuery(sqlUpdateDiem, updateParams);
if (updatedRows > 0)
{
    Console.WriteLine("Cập nhật điểm thành công. Điểm TB đã được tự động tính!");
}
```
> Ví dụ 2.3: Xóa một học sinh (DELETE)

```
// Xóa học sinh. Trigger TRG_UpdateSiSo sẽ tự động giảm sĩ số lớp.
// Lưu ý: Phải xóa dữ liệu bảng DIEMSO của học sinh này trước do có khóa ngoại (FK_DIEMSO_HOCSINH).
string sqlDeleteHS = "DELETE FROM HOCSINH WHERE MaHocSinh = @MaHocSinh";

object[] deleteParams = { "HS001" };

int deletedRows = DatabaseHelper.ExecuteNonQuery(sqlDeleteHS, deleteParams);
if (deletedRows > 0)
{
    Console.WriteLine("Xóa học sinh thành công!");
}
```
### 3. Mô hình Active Record (Tích hợp CRUD trực tiếp vào Object)
> Thay vì viết các hàm CRUD rời rạc ở các file Service bên ngoài, dự án hiện tại áp dụng mô hình thiết kế Active Record. Nghĩa là các thao tác Thêm, Sửa, Xóa, và Lấy danh sách được đóng gói trực tiếp vào bên trong class của thực thể đó (ví dụ: HocSinh, Lop).

Ưu điểm:

- Tư duy hướng đối tượng (OOP) rõ ràng hơn. Các entity tự biết cách quản lý dữ liệu của chính nó dưới Database.
- Khi gọi các hàm Them(), Sua(), ta không cần truyền tham số (như HocSinh hs) nữa. Hàm sẽ tự động dùng từ khóa this để lấy dữ liệu từ chính đối tượng đang gọi nó. Bên dưới, các hàm này vẫn sử dụng DatabaseHelper.ExecuteNonQuery trả về số nguyên (int) đại diện cho số dòng bị tác động trong CSDL.

> Ví dụ cho HocSinh:

```
// Lấy danh sách: Gọi trực tiếp từ Class (hàm static)
DataGridHocSinh.ItemsSource = HocSinh.LayDanhSach();

// Thêm mới: Tạo đối tượng từ UI và ra lệnh cho nó tự lưu
HocSinh hsMoi = new HocSinh()
{
    MaHocSinh = txtMaHS.Text,
    HoTen = txtHoTen.Text,
    MaLop = cbLop.SelectedValue?.ToString() ?? "",
    NgaySinh = dpNgaySinh.SelectedDate ?? DateTime.Now
};

if (hsMoi.Them()) 
{
    MessageBox.Show("Lưu học sinh thành công!");
}
// Sửa thông tin học sinh
if (DataGridHocSinh.SelectedItem is HocSinh hsDangChon)
{
    // Cập nhật các thông tin mới từ giao diện vào đối tượng đang chọn
    // Lưu ý: MaHocSinh thường không cho sửa vì là Khóa chính (Primary Key)
    hsDangChon.HoTen = txtHoTen.Text;
    hsDangChon.GioiTinh = cbGioiTinh.Text;
    hsDangChon.NgaySinh = dpNgaySinh.SelectedDate ?? DateTime.Now;
    hsDangChon.DiaChi = txtDiaChi.Text;
    hsDangChon.Email = txtEmail.Text;
    hsDangChon.MaLop = cbLop.SelectedValue?.ToString() ?? ""; // Gán mã lớp mới từ ComboBox

    // Ra lệnh cho đối tượng tự cập nhật chính nó xuống Database
    // Hàm Sua() sẽ tự động lấy các giá trị @tham_số từ chính 'this'
    if (hsDangChon.Sua())
    {
        MessageBox.Show($"Cập nhật thông tin học sinh {hsDangChon.MaHocSinh} thành công!");
        
        // Làm mới danh sách hiển thị để thấy dữ liệu mới nhất
        DataGridHocSinh.ItemsSource = HocSinh.LayDanhSach();
    }
    else
    {
        MessageBox.Show("Có lỗi xảy ra khi cập nhật. Vui lòng kiểm tra lại dữ liệu.");
    }
}
else
{
    MessageBox.Show("Vui lòng chọn một học sinh trong danh sách để sửa!");
}

// Xóa: Gọi từ Class và truyền ID (hàm static)
HocSinh.Xoa("HS001");
```
> Ví dụ cho Lop:
```
// Lấy danh sách Lớp: Đổ dữ liệu vào DataGrid khi vừa mở màn hình
// Hàm LayDanhSach là static nên gọi trực tiếp từ tên Class
DataGridLop.ItemsSource = Lop.LayDanhSach();

// Thêm một lớp mới: Lấy thông tin từ các TextBox/ComboBox trên giao diện
// Giả sử bạn có các control: txtMaLop, txtTenLop, cbKhoi
Lop lopMoi = new Lop()
{
    MaLop = txtMaLop.Text,
    TenLop = txtTenLop.Text,
    // Vì Khoi là kiểu int, cần ép kiểu từ chuỗi nhập vào
    Khoi = int.TryParse(cbKhoi.Text, out int k) ? k : 0,
    // Sĩ số ban đầu thường là 0, Database Trigger sẽ tự cập nhật khi có học sinh vào lớp
    SiSo = 0 
};

// Gọi hàm Them() của đối tượng lopMoi
// Hàm này sẽ tự lấy this.MaLop, this.TenLop... để gửi vào DatabaseHelper
if (lopMoi.Them())
{
    MessageBox.Show("Tạo lớp mới thành công!");
    // Cập nhật lại danh sách hiển thị sau khi thêm
    DataGridLop.ItemsSource = Lop.LayDanhSach();
}
else
{
    MessageBox.Show("Lỗi: Không thể tạo lớp. Vui lòng kiểm tra lại mã lớp.");
}

// Cập nhật thông tin Lớp (Sửa)
// Giả sử đang chọn một dòng trong DataGrid để sửa
if (DataGridLop.SelectedItem is Lop lopDangChon)
{
    lopDangChon.TenLop = txtTenLop.Text;
    lopDangChon.Khoi = int.Parse(cbKhoi.Text);
    
    if (lopDangChon.Sua())
    {
        MessageBox.Show("Cập nhật thông tin lớp thành công!");
    }
}

// Xóa một lớp: Thường dùng mã lớp làm điều kiện xóa
// Lưu ý: Cần kiểm tra xem lớp có học sinh không trước khi xóa (do ràng buộc khóa ngoại)
string maLopCanXoa = "101";
if (Lop.Xoa(maLopCanXoa))
{
    MessageBox.Show($"Đã xóa lớp {maLopCanXoa} thành công.");
}
```
## Một số ưu điểm

1. Chống SQL Injection: Nếu cộng trực tiếp chuỗi (string concatenation), kẻ xấu có thể nhập những câu lệnh phá hủy CSDL. Dùng @parameter giúp tách biệt "Lệnh" và "Dữ liệu" an toàn hơn.

2. Tự động hóa: Hàm DatabaseHelper xử lí tất cả việc "Mở kết nối", "Thực thi" và quan trọng nhất là "Tự đóng kết nối" sau khi xong việc thông qua khối using. Giúp app không bị treo do chiếm dụng CSDL quá lâu.