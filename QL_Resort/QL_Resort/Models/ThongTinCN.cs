using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class ThongTinCN
    {
        int idnv, id_ttcn;
        string id_tk;
        string hoten;

        public int Idnv { get => idnv; set => idnv = value; }
        public int Id_ttcn { get => id_ttcn; set => id_ttcn = value; }
        public string Id_tk { get => id_tk; set => id_tk = value; }
        public string Hoten { get => hoten; set => hoten = value; }
    }
}