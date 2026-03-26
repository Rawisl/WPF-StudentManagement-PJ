-- TABLE STRUCTURE -- 
CREATE TABLE THAMSO (
    MaThamSo VARCHAR(20) CONSTRAINT PK_THAMSO PRIMARY KEY,
    TenThamSo NVARCHAR(100),
    GiaTri FLOAT
);
CREATE TABLE LOP (
    MaLop VARCHAR(10) CONSTRAINT PK_LOP PRIMARY KEY,
    TenLop NVARCHAR(50),
    Khoi INT,
    SiSo INT DEFAULT 0
);
CREATE TABLE HOCSINH (
    MaHocSinh VARCHAR(10) CONSTRAINT PK_HOCSINH PRIMARY KEY,
    HoTen NVARCHAR(100),
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    DiaChi NVARCHAR(200),
    Email VARCHAR(100),
    MaLop VARCHAR(10),
    CONSTRAINT FK_HOCSINH_LOP FOREIGN KEY (MaLop) REFERENCES LOP(MaLop)
);
CREATE TABLE MONHOC (
    MaMonHoc VARCHAR(10) CONSTRAINT PK_MONHOC PRIMARY KEY,
    TenMonHoc NVARCHAR(50) 
);
CREATE TABLE DIEMSO (
    MaDiemSo INT IDENTITY(1,1) CONSTRAINT PK_DIEMSO PRIMARY KEY,
    MaHocSinh VARCHAR(10),
    MaMonHoc VARCHAR(10),
    HocKy INT CONSTRAINT CHK_HocKy CHECK (HocKy IN (1, 2)),
    Diem15p FLOAT CONSTRAINT CHK_Diem15p CHECK (Diem15p BETWEEN 0 AND 10),
    Diem1Tiet FLOAT CONSTRAINT CHK_Diem1Tiet CHECK (Diem1Tiet BETWEEN 0 AND 10),
    DiemTB FLOAT CONSTRAINT CHK_DiemTB CHECK (DiemTB BETWEEN 0 AND 10),
    CONSTRAINT FK_DIEMSO_HOCSINH FOREIGN KEY (MaHocSinh) REFERENCES HOCSINH(MaHocSinh),
    CONSTRAINT FK_DIEMSO_MONHOC FOREIGN KEY (MaMonHoc) REFERENCES MONHOC(MaMonHoc)
);
-- CONSTRAINT, TRIGGERS --
GO
CREATE TRIGGER TRG_HOCSINH_InsertUpdate
ON HOCSINH
FOR INSERT, UPDATE
AS
BEGIN
    -- 1. Kiểm tra tuổi
    DECLARE @MinAge INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MinAge');
    DECLARE @MaxAge INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MaxAge');

    IF EXISTS (SELECT * FROM inserted WHERE (YEAR(GETDATE()) - YEAR(NgaySinh)) NOT BETWEEN @MinAge AND @MaxAge)
    BEGIN
        RAISERROR(N'Tuổi học sinh không hợp lệ so với quy định.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Kiểm tra sĩ số tối đa khi thêm vào lớp
    DECLARE @MaxClassSize INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MaxClassSize');
    
    IF EXISTS (
        SELECT L.MaLop 
        FROM LOP L JOIN inserted I ON L.MaLop = I.MaLop
        WHERE L.SiSo >= @MaxClassSize
    )
    BEGIN
        RAISERROR(N'Lớp đã đủ sĩ số tối đa, không thể thêm học sinh.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
CREATE TRIGGER TRG_UpdateSiSo
ON HOCSINH
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
    -- Tăng sĩ số khi thêm mới
    UPDATE LOP 
    SET SiSo = SiSo + (SELECT COUNT(*) FROM inserted WHERE MaLop = LOP.MaLop)
    FROM LOP JOIN inserted ON LOP.MaLop = inserted.MaLop;

    -- Giảm sĩ số khi xóa
    UPDATE LOP 
    SET SiSo = SiSo - (SELECT COUNT(*) FROM deleted WHERE MaLop = LOP.MaLop)
    FROM LOP JOIN deleted ON LOP.MaLop = deleted.MaLop;
END;
GO
CREATE TRIGGER TRG_TinhDiemTB
ON DIEMSO
AFTER INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra nếu việc cập nhật không liên quan đến điểm số thì bỏ qua để tối ưu
    IF NOT (UPDATE(Diem15p) OR UPDATE(Diem1Tiet)) RETURN;

    -- Cập nhật DiemTB dựa trên inserted
    UPDATE DIEMSO
    SET DiemTB = CASE 
                    WHEN I.Diem15p IS NOT NULL AND I.Diem1Tiet IS NOT NULL 
                    THEN (I.Diem15p + I.Diem1Tiet) / 2
                    ELSE NULL 
                 END
    FROM DIEMSO D
    JOIN inserted I ON D.MaDiemSo = I.MaDiemSo;
END;
GO
-- MOCK DATA --
INSERT INTO THAMSO (MaThamSo, TenThamSo, GiaTri) 
VALUES 
    ('MinAge', N'Tuổi tối thiểu', 15),
    ('MaxAge', N'Tuổi tối đa', 20),
    ('MaxClassSize', N'Sĩ số tối đa', 40),
    ('PassingGrade', N'Điểm đạt môn', 5);
INSERT INTO LOP(MaLop, TenLop, Khoi, SiSo)
VALUES 
    -- Lớp 10
    ('101', N'Lớp 10A1', 10, 0),
    ('102', N'Lớp 10A2', 10, 0),
    ('103', N'Lớp 10A3', 10, 0),
    ('104', N'Lớp 10A4', 10, 0),
    -- Lớp 11
    ('111', N'Lớp 11A1', 11, 0),
    ('112', N'Lớp 11A2', 11, 0),
    -- Lớp 12
    ('113', N'Lớp 11A3', 11, 0),
    ('121', N'Lớp 12A1', 12, 0),
    ('122', N'Lớp 12A2', 12, 0);
INSERT INTO MONHOC (MaMonHoc, TenMonHoc) 
VALUES 
    ('MH01', N'Toán'),
    ('MH02', N'Lý'),
    ('MH03', N'Hóa'),
    ('MH04', N'Sinh'),
    ('MH05', N'Sử'),
    ('MH06', N'Địa'),
    ('MH07', N'Văn'),
    ('MH08', N'Đạo Đức'),
    ('MH09', N'Thể Dục');
