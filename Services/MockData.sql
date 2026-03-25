
------------------------------------- Tạo ngẫu nhiên 10 học sinh cho mỗi lớp hiện có trong bảng LOP
SET NOCOUNT ON;

-- Khai báo các biến hỗ trợ sinh dữ liệu ngẫu nhiên
DECLARE @MaLop VARCHAR(10);
DECLARE @i INT, @j INT;
DECLARE @HoTen NVARCHAR(100);
DECLARE @NgaySinh DATE;
DECLARE @MaHS VARCHAR(10);
DECLARE @RandomYear INT;

-- Bảng tạm chứa danh sách Họ và Tên để ghép ngẫu nhiên
DECLARE @Ho TABLE (TenHo NVARCHAR(20));
INSERT INTO @Ho VALUES (N'Nguyễn'), (N'Trần'), (N'Lê'), (N'Phạm'), (N'Hoàng'), (N'Vũ'), (N'Phan'), (N'Đặng');

DECLARE @Dem TABLE (TenDem NVARCHAR(20));
INSERT INTO @Dem VALUES (N'Văn'), (N'Thị'), (N'Anh'), (N'Minh'), (N'Hữu'), (N'Quốc'), (N'Bảo'), (N'Gia');

DECLARE @Ten TABLE (TenChinh NVARCHAR(20));
INSERT INTO @Ten VALUES (N'Hùng'), (N'Dũng'), (N'Hạnh'), (N'Phúc'), (N'Tuấn'), (N'Lan'), (N'Cường'), (N'Linh'), (N'Nam'), (N'Ngọc');

-- Con trỏ để duyệt qua tất cả các lớp hiện có trong bảng LOP
DECLARE ClassCursor CURSOR FOR SELECT MaLop FROM LOP;
OPEN ClassCursor;

FETCH NEXT FROM ClassCursor INTO @MaLop;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @j = 1;
    WHILE @j <= 10 -- Tạo 10 học sinh cho mỗi lớp
    BEGIN
        -- Tạo mã học sinh: HS + MaLop + Số thứ tự (VD: HS10101, HS10102...)
        SET @MaHS = 'HS' + @MaLop + RIGHT('0' + CAST(@j AS VARCHAR(2)), 2);
        
        -- Ghép tên ngẫu nhiên
        SELECT TOP 1 @HoTen = TenHo FROM @Ho ORDER BY NEWID();
        SELECT TOP 1 @HoTen = @HoTen + ' ' + TenDem FROM @Dem ORDER BY NEWID();
        SELECT TOP 1 @HoTen = @HoTen + ' ' + TenChinh FROM @Ten ORDER BY NEWID();

        -- Tính toán ngày sinh ngẫu nhiên để thỏa mãn tuổi 15-20 (Năm hiện tại 2026)
        -- Học sinh sinh năm 2006 đến 2011
        SET @RandomYear = 2006 + ABS(CHECKSUM(NEWID())) % 6;
        SET @NgaySinh = DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, CAST(CAST(@RandomYear AS VARCHAR) + '-01-01' AS DATE));

        -- Insert vào bảng HOCSINH
        INSERT INTO HOCSINH (MaHocSinh, HoTen, GioiTinh, NgaySinh, DiaChi, Email, MaLop)
        VALUES (
            @MaHS, 
            @HoTen, 
            CASE WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 THEN N'Nam' ELSE N'Nữ' END,
            @NgaySinh,
            N'Địa chỉ số ' + CAST(ABS(CHECKSUM(NEWID())) % 100 AS NVARCHAR(10)) + N', TP. HCM',
            LOWER(@MaHS) + '@school.edu.vn',
            @MaLop
        );

        SET @j = @j + 1;
    END
    FETCH NEXT FROM ClassCursor INTO @MaLop;
END

CLOSE ClassCursor;
DEALLOCATE ClassCursor;

PRINT 'Da tao xong 10 hoc sinh cho moi lop.';
GO
---
-------------------------------------- Tạo điểm số ngẫu nhiên cho tất cả học sinh và môn học
INSERT INTO DIEMSO (MaHocSinh, MaMonHoc, HocKy, Diem15p, Diem1Tiet)
SELECT 
    H.MaHocSinh, 
    M.MaMonHoc, 
    1 AS HocKy, -- Mặc định là học kỳ 1
    -- Tạo điểm 15p ngẫu nhiên (0.0 - 10.0)
    CAST(ABS(CHECKSUM(NEWID())) % 101 AS FLOAT) / 10 AS Diem15p,
    -- Tạo điểm 1 Tiết ngẫu nhiên (0.0 - 10.0)
    CAST(ABS(CHECKSUM(NEWID())) % 101 AS FLOAT) / 10 AS Diem1Tiet
FROM HOCSINH H
CROSS JOIN MONHOC M; -- Kết hợp mọi học sinh với mọi môn học

PRINT 'Da tao xong diem so cho tat ca hoc sinh bang phương phap Set-based!';
GO