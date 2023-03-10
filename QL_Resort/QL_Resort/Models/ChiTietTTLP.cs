using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ChiTietTTLP
    {
        private string ten, motalp, motattlp, hinhanh, id;
        private double gia;
        private List<int> id_p;

        public string Ten { get => ten; set => ten = value; }
        public string Motalp { get => motalp; set => motalp = value; }
        public string Motattlp { get => motattlp; set => motattlp = value; }
        public string Hinhanh { get => hinhanh; set => hinhanh = value; }
        public string Id { get => id; set => id = value; }
        public double Gia { get => gia; set => gia = value; }
        public List<int> Id_p { get => id_p; set => id_p = value; }
    }
}