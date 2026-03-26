-- Sao chép danh sách lớp từ Học kỳ 1 sang Học kỳ 2 cho cùng một Năm học
INSERT INTO PHANLOP (MaHocSinh, MaLop, HocKy, NamHoc)
SELECT 
    P1.MaHocSinh, 
    P1.MaLop, 
    2 AS HocKy, 
    P1.NamHoc
FROM PHANLOP P1
WHERE P1.HocKy = 1
  -- Đảm bảo học sinh này chưa có dữ liệu ở Học kỳ 2 của năm học đó
  AND NOT EXISTS (
      SELECT 1 
      FROM PHANLOP P2 
      WHERE P2.MaHocSinh = P1.MaHocSinh 
        AND P2.HocKy = 2 
        AND P2.NamHoc = P1.NamHoc
  );

PRINT N'Đã khởi tạo thành công danh sách Học kỳ 2 dựa trên Học kỳ 1!';