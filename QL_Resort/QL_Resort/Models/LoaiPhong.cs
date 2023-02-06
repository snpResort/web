using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class LoaiPhong
    {
        private string ten, mota, hinhanh, id;
        private double gia;
        private int id_P;
        private int slnl, slte;
        private List<HINHANH_LOAIPHONG> ha;
        public LoaiPhong()
        {

        }

        public string Ten { get => ten; set => ten = value; }
        public string Mota { get => mota; set => mota = value; }
        public string Hinhanh { get => hinhanh; set => hinhanh = value; }
        public double Gia { get => gia; set => gia = value; }
        public string Id { get => id; set => id = value; }
        public int Id_P { get => id_P; set => id_P = value; }
        public List<HINHANH_LOAIPHONG> Ha { get => ha; set => ha = value; }
        public int Slnl { get => slnl; set => slnl = value; }
        public int Slte { get => slte; set => slte = value; }
    }
}