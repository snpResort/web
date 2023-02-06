using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ThongTinDV
    {
        private string ten, mota, hinhanh, id;

        public string Ten { get => ten; set => ten = value; }
        public string Mota { get => mota; set => mota = value; }
        public string Hinhanh { get => hinhanh; set => hinhanh = value; }
        public string Id { get => id; set => id = value; }
    }
}