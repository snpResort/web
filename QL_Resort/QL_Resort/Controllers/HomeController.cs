using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_Resort.Models;


namespace QL_Resort.Controllers
{
    public class HomeController : Controller
    {
        QL_ResortDataContext db = new QL_ResortDataContext();
        public ActionResult Index()
        {
            var lp = db.LOAIPHONGs;
            List<LoaiPhong> data=new List<LoaiPhong>();
            foreach(var item in lp)
            {
                LoaiPhong _loaiphong = new LoaiPhong();
                _loaiphong.Id = item.Id;
                _loaiphong.Ten = item.TenLoai;
                _loaiphong.Mota = item.MoTa;
                _loaiphong.Gia = (double)item.Gia;
                _loaiphong.Hinhanh = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == item.Id).FirstOrDefault().imagePath;
                //_loaiphong.Id_P = (Int32.Parse(db.PHONGs.Where(a => a.Id_LP == item.Id).ToString()));
                _loaiphong.Ha = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == item.Id).ToList();
                data.Add(_loaiphong);
            }
            dynamic mymodel = new ExpandoObject();
            mymodel.LoaiPhong = data;
            return View(mymodel);
        }
        public ActionResult ThongTinTK()
        {
            var user = Session["User"] as TAIKHOAN;
            
            var infoAccount = db.THONGTINCANHANs.Where(tk => tk.Id_tk == user.Id).FirstOrDefault();
            dynamic mymodel = new ExpandoObject();
            mymodel.Info = infoAccount;
            return View(mymodel);
        }
        public ActionResult CapNhatThongTinTK()
        {
           
            return View();
        }

    }
}