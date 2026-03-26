using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using WPF_StudentManagement_Project.Services;

namespace WPF_StudentManagement_Project.ViewModels;

// 1. CLASS MODEL CHO DATAGRID
public partial class HocSinhDiemDisplay : ObservableObject
{
    public int Stt { get; set; }
    public string MaHocSinh { get; set; } // ID ẩn để lưu CSDL
    public string HoTen { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DiemTB))]
    private double? _diem15Phut;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DiemTB))]
    private double? _diem1Tiet;

    public double? DiemTB
    {
        get
        {
            if (Diem15Phut.HasValue && Diem1Tiet.HasValue)
                return Math.Round((Diem15Phut.Value + Diem1Tiet.Value * 2) / 3, 1);
            return null;
        }
    }
}

public partial class NhapDiemViewModel : ObservableObject
{
    // === COMBOBOX ===
    public ObservableCollection<Lop> DanhSachLop { get; set; } = new();
    public ObservableCollection<MonHoc> DanhSachMon { get; set; } = new();
    public ObservableCollection<int> DanhSachHocKy { get; set; } = new() { 1, 2 };

    [ObservableProperty] private Lop? _lopDuocChon;
    [ObservableProperty] private MonHoc? _monDuocChon;
    [ObservableProperty] private int _hocKyDuocChon = 1;

    // Năm học lấy chuẩn theo DB của Long
    private string _namHocHienTai = "2023 - 2024";

    // === DATAGRID ===
    public ObservableCollection<HocSinhDiemDisplay> DanhSachHocSinh { get; set; } = new();

    public NhapDiemViewModel()
    {
        LoadDuLieuComboBox();
    }

    private void LoadDuLieuComboBox()
    {
        var dsLop = Lop.LayDanhSach();
        var dsMon = MonHoc.LayDanhSach();

        foreach (var l in dsLop) DanhSachLop.Add(l);
        foreach (var m in dsMon) DanhSachMon.Add(m);

        if (DanhSachLop.Count > 0) LopDuocChon = DanhSachLop[0];
        if (DanhSachMon.Count > 0) MonDuocChon = DanhSachMon[0];
    }

    // === LẤY DANH SÁCH ===
    [RelayCommand]
    private void LayDanhSach()
    {
        if (LopDuocChon == null || MonDuocChon == null)
        {
            MessageBox.Show("Vui lòng chọn Lớp và Môn học!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            DanhSachHocSinh.Clear();

            string sqlQuery = @"
                SELECT hs.MaHocSinh, hs.HoTen, d.Diem15p, d.Diem1Tiet 
                FROM HOCSINH hs
                INNER JOIN PHANLOP pl ON hs.MaHocSinh = pl.MaHocSinh
                LEFT JOIN DIEMSO d ON pl.MaPhanLop = d.MaPhanLop AND d.MaMonHoc = @MaMonHoc
                WHERE pl.MaLop = @MaLop AND pl.HocKy = @HocKy AND pl.NamHoc = @NamHoc";

            SqlParameter[] sqlParams = {
                new SqlParameter("@MaMonHoc", MonDuocChon.MaMonHoc),
                new SqlParameter("@MaLop", LopDuocChon.MaLop),
                new SqlParameter("@HocKy", HocKyDuocChon),
                new SqlParameter("@NamHoc", _namHocHienTai)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sqlQuery, sqlParams);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy học sinh nào trong Lớp/Học kỳ này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int count = 1;
            foreach (DataRow row in dt.Rows)
            {
                var hs = new HocSinhDiemDisplay
                {
                    Stt = count++,
                    MaHocSinh = row["MaHocSinh"]?.ToString() ?? "",
                    HoTen = row["HoTen"]?.ToString() ?? "N/A"
                };

                // Ép kiểu điểm số an toàn
                if (row["Diem15p"] != DBNull.Value) hs.Diem15Phut = Convert.ToDouble(row["Diem15p"]);
                if (row["Diem1Tiet"] != DBNull.Value) hs.Diem1Tiet = Convert.ToDouble(row["Diem1Tiet"]);

                DanhSachHocSinh.Add(hs);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi Database:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // === LƯU BẢNG ĐIỂM ===
    [RelayCommand]
    private void LuuBangDiem()
    {
        if (LopDuocChon == null || MonDuocChon == null) return;

        if (DanhSachHocSinh.Count == 0)
        {
            MessageBox.Show("Bảng điểm đang trống, không có gì để lưu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int thanhCong = 0;
        int thatBai = 0;

        foreach (var hs in DanhSachHocSinh)
        {
            // Chỉ lưu những bạn có nhập đủ cả 2 cột điểm
            if (hs.Diem15Phut.HasValue && hs.Diem1Tiet.HasValue)
            {
                try
                {
                    bool result = DiemSo.LuuDiem(
                        maHocSinh: hs.MaHocSinh,
                        maMonHoc: MonDuocChon.MaMonHoc,
                        hocKy: HocKyDuocChon,
                        namHoc: _namHocHienTai,
                        diem15p: hs.Diem15Phut.Value,
                        diem1Tiet: hs.Diem1Tiet.Value
                    );

                    if (result) thanhCong++;
                    else thatBai++;
                }
                catch (Exception)
                {
                    thatBai++;
                }
            }
        }

        MessageBox.Show($"Đã lưu thành công điểm của {thanhCong} học sinh.\nThất bại (hoặc điểm không hợp lệ): {thatBai}", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}