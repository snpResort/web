using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ThongTinPhong
    {
        private string id_DP, id_CTDP;
        private string ngaydat, ngaytra;
        private List<int> id_P;
        private string ten_kh;
        private string soluong, songayo;
        private List<string> tenphong;
        private List<string> slnl, slte;
        private double tongtien;
        private List<string> cactenP;
        private List<double> giaLP;

        public string Id_DP { get => id_DP; set => id_DP = value; }
        public string Id_CTDP { get => id_CTDP; set => id_CTDP = value; }
        public List<int> Id_P { get => id_P; set => id_P = value; }
        public string Ngaydat { get => ngaydat; set => ngaydat = value; }
        public string Ngaytra { get => ngaytra; set => ngaytra = value; }
        public string Ten_kh { get => ten_kh; set => ten_kh = value; }
        public string Soluong { get => soluong; set => soluong = value; }
        public string Songayo { get => songayo; set => songayo = value; }
        public List<string> Tenphong { get => tenphong; set => tenphong = value; }
        public List<string> Slnl { get => slnl; set => slnl = value; }
        public List<string> Slte { get => slte; set => slte = value; }
        public double Tongtien { get => tongtien; set => tongtien = value; }
        public List<string> CactenP { get => cactenP; set => cactenP = value; }
        public List<double> GiaLP { get => giaLP; set => giaLP = value; }
    }
}