using QL_Resort.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_Resort.Controllers
{
    public class DichVuController : Controller
    {
        // GET: DichVu
        QL_ResortDataContext db = new QL_ResortDataContext();
        public ActionResult DichVu(String id)
        {
            dynamic mymodel = new ExpandoObject();
            var ha = db.HINHANH_DICHVUs.ToList();
            var mota = db.DICHVUs.Where(t => t.Id == id);
            var _ttdv = db.DICHVUs.Join(db.HINHANH_DICHVUs, dv => dv.Id, hadv => hadv.Id_DV, (dv, hadv) => new ThongTinDV { Ten = dv.TenDV, Mota = dv.MoTa });
            mymodel.HA = ha;
            mymodel.TTDV = _ttdv;
            return View(mymodel);
        }
    }
}