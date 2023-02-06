using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_Resort.Models
{
    public class TaiKhoanNV
    {
        private string tenTK, matKhau, chucVu;
        private DateTime ngayTao;
        public TaiKhoanNV()
        {

        }

        public string TenTK { get => tenTK; set => tenTK = value; }
        public string MatKhau { get => matKhau; set => matKhau = value; }
        public string ChucVu { get => chucVu; set => chucVu = value; }
        public DateTime NgayTao { get => ngayTao; set => ngayTao = value; }
    }
}