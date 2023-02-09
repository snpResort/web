using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ThongTinDP
    {
        private String ngDat, ngTra, soLuongNgLon, soLuongTreEm, id;
        double gia;
        private List<CtDP> ctdp;

        public ThongTinDP()
        {
        }

        public string NgDat { get => ngDat; set => ngDat = value; }
        public string NgTra { get => ngTra; set => ngTra = value; }
        public string SoLuongNgLon { get => soLuongNgLon; set => soLuongNgLon = value; }
        public string SoLuongTreEm { get => soLuongTreEm; set => soLuongTreEm = value; }
        public double Gia { get => gia; set => gia = value; }
        public List<CtDP> Ctdp { get => ctdp; set => ctdp = value; }
        public string Id { get => id; set => id = value; }
    }

    public class CtDP
    {
        private String id_DatPhong, gia, tenLoai, soLuong;

        public CtDP()
        {
        }

        public string Id_DatPhong { get => id_DatPhong; set => id_DatPhong = value; }
        public string Gia { get => gia; set => gia = value; }
        public string TenLoai { get => tenLoai; set => tenLoai = value; }
        public string SoLuong { get => soLuong; set => soLuong = value; }
    }
}