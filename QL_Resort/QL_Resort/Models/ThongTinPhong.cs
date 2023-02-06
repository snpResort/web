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

        public string Id_DP { get => id_DP; set => id_DP = value; }
        public string Id_CTDP { get => id_CTDP; set => id_CTDP = value; }
        public List<int> Id_P { get => id_P; set => id_P = value; }
        public string Ngaydat { get => ngaydat; set => ngaydat = value; }
        public string Ngaytra { get => ngaytra; set => ngaytra = value; }
    }
}