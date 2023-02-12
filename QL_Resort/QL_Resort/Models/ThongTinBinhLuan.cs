using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ThongTinBinhLuan
    {
        private string tenKH, binhLuan;
        private double binhChon;

        public string TenKH { get => tenKH; set => tenKH = value; }
        public string BinhLuan { get => binhLuan; set => binhLuan = value; }
        public double BinhChon { get => binhChon; set => binhChon = value; }
    }
}