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
    DECLARE @MinAge INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MinAge');
    DECLARE @MaxAge INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MaxAge');

    IF EXISTS (SELECT * FROM inserted WHERE (YEAR(GETDATE()) - YEAR(NgaySinh)) NOT BETWEEN @MinAge AND @MaxAge)
    BEGIN
        RAISERROR(N'Tuổi học sinh không hợp lệ so với quy định.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

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
    UPDATE LOP 
    SET SiSo = SiSo + (SELECT COUNT(*) FROM inserted WHERE MaLop = LOP.MaLop)
    FROM LOP JOIN inserted ON LOP.MaLop = inserted.MaLop;

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
    IF NOT (UPDATE(Diem15p) OR UPDATE(Diem1Tiet)) RETURN;

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
    ('101', N'Lớp 10A1', 10, 0),
    ('102', N'Lớp 10A2', 10, 0),
    ('103', N'Lớp 10A3', 10, 0),
    ('104', N'Lớp 10A4', 10, 0),
    ('111', N'Lớp 11A1', 11, 0),
    ('112', N'Lớp 11A2', 11, 0),
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
---------------------------------------------------- WEEK 2 CHANGES ----------------------------------------------------
-- DROP OLD TRIGGERS
DROP TRIGGER IF EXISTS TRG_HOCSINH_InsertUpdate;
DROP TRIGGER IF EXISTS TRG_UpdateSiSo;
GO

-- CREATE NEW 'PHANLOP' TABLE
CREATE TABLE PHANLOP (
    MaPhanLop INT IDENTITY(1,1) CONSTRAINT PK_PHANLOP PRIMARY KEY,
    MaHocSinh VARCHAR(10),
    MaLop VARCHAR(10),
    HocKy INT CONSTRAINT CHK_PHANLOP_HocKy CHECK (HocKy IN (1, 2)),
    -- Basic constraint to enforce YYYY - YYYY format
    NamHoc VARCHAR(15) CONSTRAINT CHK_PHANLOP_NamHoc CHECK (NamHoc LIKE '[0-9][0-9][0-9][0-9] - [0-9][0-9][0-9][0-9]' AND CAST(LEFT(NamHoc, 4) AS INT) + 1 = CAST(RIGHT(NamHoc, 4) AS INT)),
    CONSTRAINT FK_PHANLOP_HOCSINH FOREIGN KEY (MaHocSinh) REFERENCES HOCSINH(MaHocSinh),
    CONSTRAINT FK_PHANLOP_LOP FOREIGN KEY (MaLop) REFERENCES LOP(MaLop)
);
GO

-- MIGRATE DATA TO 'PHANLOP'
-- Insert HocKy 1 data based on student's birth year and assigned grade
INSERT INTO PHANLOP (MaHocSinh, MaLop, HocKy, NamHoc)
SELECT 
    H.MaHocSinh, 
    H.MaLop, 
    1, 
    -- Recalculate NamHoc based on NgaySinh and Khoi to ensure it reflects the correct academic year
    CAST(YEAR(H.NgaySinh) + L.Khoi + 5 AS VARCHAR(4)) + ' - ' + CAST(YEAR(H.NgaySinh) + L.Khoi + 6 AS VARCHAR(4))
FROM HOCSINH H
JOIN LOP L ON H.MaLop = L.MaLop
WHERE H.MaLop IS NOT NULL;

-- Insert HocKy 2 data if any student already has scores for HocKy 2 in DIEMSO
INSERT INTO PHANLOP (MaHocSinh, MaLop, HocKy, NamHoc)
SELECT DISTINCT 
    D.MaHocSinh, 
    H.MaLop, 
    2, 
    CAST(YEAR(H.NgaySinh) + L.Khoi + 5 AS VARCHAR(4)) + ' - ' + CAST(YEAR(H.NgaySinh) + L.Khoi + 6 AS VARCHAR(4))
FROM DIEMSO D
JOIN HOCSINH H ON D.MaHocSinh = H.MaHocSinh
JOIN LOP L ON H.MaLop = L.MaLop
WHERE D.HocKy = 2 AND NOT EXISTS (
    SELECT 1 FROM PHANLOP P WHERE P.MaHocSinh = D.MaHocSinh AND P.HocKy = 2
);
GO

-- UPDATE 'DIEMSO' TABLE
-- Add the new column
ALTER TABLE DIEMSO ADD MaPhanLop INT;
GO

-- Migrate data: Link DIEMSO to the newly created PHANLOP records
UPDATE D
SET D.MaPhanLop = P.MaPhanLop
FROM DIEMSO D
JOIN PHANLOP P ON D.MaHocSinh = P.MaHocSinh AND D.HocKy = P.HocKy;
GO

-- Drop old constraints and FKs from DIEMSO before dropping columns
ALTER TABLE DIEMSO DROP CONSTRAINT IF EXISTS FK_DIEMSO_HOCSINH;
ALTER TABLE DIEMSO DROP CONSTRAINT IF EXISTS CHK_HocKy;

-- Drop the outdated columns
ALTER TABLE DIEMSO DROP COLUMN MaHocSinh;
ALTER TABLE DIEMSO DROP COLUMN HocKy;

-- Add the new foreign key
ALTER TABLE DIEMSO ADD CONSTRAINT FK_DIEMSO_PHANLOP FOREIGN KEY (MaPhanLop) REFERENCES PHANLOP(MaPhanLop);
GO

-- UPDATE 'HOCSINH' TABLE
ALTER TABLE HOCSINH DROP CONSTRAINT IF EXISTS FK_HOCSINH_LOP;
ALTER TABLE HOCSINH DROP COLUMN MaLop;
GO

-- RECREATE TRIGGERS (Adapted for the new structure)
-- Age check now triggers from PHANLOP instead of HOCSINH
CREATE TRIGGER TRG_PHANLOP_CheckAge
ON PHANLOP
FOR INSERT, UPDATE
AS
BEGIN
    DECLARE @MinAge INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MinAge');
    DECLARE @MaxAge INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MaxAge');

    IF EXISTS (
        SELECT 1 
        FROM inserted I
        JOIN HOCSINH H ON I.MaHocSinh = H.MaHocSinh
        WHERE (CAST(LEFT(I.NamHoc, 4) AS INT) - YEAR(H.NgaySinh)) NOT BETWEEN @MinAge AND @MaxAge
    )
    BEGIN
        RAISERROR(N'Tuổi học sinh không hợp lệ so với năm học quy định.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Max Class Size check now moves to PHANLOP
CREATE TRIGGER TRG_PHANLOP_CheckSiSo
ON PHANLOP
FOR INSERT, UPDATE
AS
BEGIN
    DECLARE @MaxClassSize INT = (SELECT GiaTri FROM THAMSO WHERE MaThamSo = 'MaxClassSize');
    
    IF EXISTS (
        SELECT L.MaLop 
        FROM LOP L JOIN inserted I ON L.MaLop = I.MaLop
        WHERE L.SiSo >= @MaxClassSize
    )
    BEGIN
        RAISERROR(N'Lớp đã đủ sĩ số tối đa, không thể thêm vào phân lớp.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Trigger : SiSo updating now triggers from PHANLOP
CREATE TRIGGER TRG_PHANLOP_UpdateSiSo
ON PHANLOP
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
    UPDATE LOP 
    SET SiSo = SiSo + (SELECT COUNT(*) FROM inserted WHERE MaLop = LOP.MaLop)
    FROM LOP JOIN inserted ON LOP.MaLop = inserted.MaLop;

    UPDATE LOP 
    SET SiSo = SiSo - (SELECT COUNT(*) FROM deleted WHERE MaLop = LOP.MaLop)
    FROM LOP JOIN deleted ON LOP.MaLop = deleted.MaLop;
END;
GO
-- Trigger: Prevent deletion of LOP if there are related PHANLOP records, and prevent deletion of MONHOC if there are related DIEMSO records
CREATE TRIGGER TRG_PreventDelete_Lop
ON LOP
FOR DELETE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM PHANLOP P JOIN deleted D ON P.MaLop = D.MaLop)
    BEGIN
        RAISERROR(N'Không thể xóa lớp này vì đã có danh sách học sinh (phân lớp).', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

CREATE TRIGGER TRG_PreventDelete_MonHoc
ON MONHOC
FOR DELETE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM DIEMSO DS JOIN deleted D ON DS.MaMonHoc = D.MaMonHoc)
    BEGIN
        RAISERROR(N'Không thể xóa môn học này vì đã có dữ liệu điểm số.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
--