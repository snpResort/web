using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ThongTinHoaDon
    {
        private string id_HD, idDP, tenNV, ngayTaoHD;
        private double tienCoc, tongtien;
        private string id_NV;
        private string id_tknv;
        private string ngaydat, ngaytra;

        public string Id_HD { get => id_HD; set => id_HD = value; }
        public string IdDP { get => idDP; set => idDP = value; }
        public string TenNV { get => tenNV; set => tenNV = value; }
        public string NgayTaoHD { get => ngayTaoHD; set => ngayTaoHD = value; }
        public double TienCoc { get => tienCoc; set => tienCoc = value; }
        public string Id_NV { get => id_NV; set => id_NV = value; }
        public string Id_tknv { get => id_tknv; set => id_tknv = value; }
        public string Ngaydat { get => ngaydat; set => ngaydat = value; }
        public string Ngaytra { get => ngaytra; set => ngaytra = value; }
        public double Tongtien { get => tongtien; set => tongtien = value; }
    }
}