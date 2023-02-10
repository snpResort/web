using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ThongKe
    {
        private string id_hd, id_dp;
        private double gia;
        private DateTime ngaytao;
        private double tongtien;

        public string Id_hd { get => id_hd; set => id_hd = value; }
        public string Id_dp { get => id_dp; set => id_dp = value; }
        public double Gia { get => gia; set => gia = value; }
        public DateTime Ngaytao { get => ngaytao; set => ngaytao = value; }
        public double Tongtien { get => tongtien; set => tongtien = value; }
    }
}