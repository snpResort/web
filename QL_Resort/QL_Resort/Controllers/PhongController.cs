using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_Resort.Models;
using System.Dynamic;
using System.Web.UI;

namespace QL_Resort.Controllers
{
    public class PhongController : Controller
    {
        // GET: Phong
        QL_ResortDataContext db = new QL_ResortDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult XemChiTiet(string id)
        {
            dynamic mymodel = new ExpandoObject();
            var ha = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == id).ToList();
            var mota = db.THONGTINLOAIPHONGs.Where(t => t.Id_LP == id).ToList().Distinct();
            var layLP = db.LOAIPHONGs.Where(t => t.Id == id).ToList();
            var _ttlp = db.LOAIPHONGs.Join(db.THONGTINLOAIPHONGs, lp => lp.Id, ttlp => ttlp.Id_LP, (lp, ttlp) => new ThongTinLP { Ten = lp.TenLoai, Motalp = lp.MoTa, Gia = (double)lp.Gia });
            mymodel.TTLP = _ttlp;
            mymodel.HA = ha;
            mymodel.MT = mota;
            mymodel.LayLP = layLP;
            return View(mymodel);
        }
        public ActionResult DatPhong(string id)
        {

            dynamic mymodel = new ExpandoObject();
            var _layP = db.PHONGs.Where(t => t.Id_LP == id).ToList();
            var _LayTTDP = db.THONGTINDATPHONGs.ToList();
            var layLP = db.LOAIPHONGs.Where(t => t.Id == id).FirstOrDefault();
            var ha = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == id).ToList();
            var mota = db.THONGTINLOAIPHONGs.Where(t => t.Id_LP == id).ToList().Distinct();
            var _ttlp = db.LOAIPHONGs.Join(db.THONGTINLOAIPHONGs, lp => lp.Id, ttlp => ttlp.Id_LP, (lp, ttlp) => new ThongTinLP { Ten = lp.TenLoai, Motalp = lp.MoTa, Gia = (double)lp.Gia });
            mymodel.LayP = _layP;
            mymodel.LayLP = layLP;
            mymodel.HA = ha;
            mymodel.TTLP = _ttlp;
            mymodel.LTTDP = _LayTTDP;
            mymodel.MT = mota;

            Session["loaiPhong"] = layLP;


            return View(mymodel);
        }
        public ActionResult TimKiem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TimKiem(FormCollection f)
        {
            dynamic mymodel = new ExpandoObject();
            string SLNL = f["SLNL"];
            string SLTE = f["SLTE"];
            int nl = 0;
            int te = 0;
            var lp = db.LOAIPHONGs.ToList();
            List<LoaiPhong> data = new List<LoaiPhong>();
            if (String.IsNullOrEmpty(SLTE) && !String.IsNullOrEmpty(SLNL))
            {
                nl = int.Parse(SLNL);
                lp = db.LOAIPHONGs.Where(t => t.SoLuongNguoiLon >= nl).ToList();
            }
            else if (String.IsNullOrEmpty(SLNL) && !String.IsNullOrEmpty(SLTE))
            {
                te = int.Parse(SLTE);
                lp = db.LOAIPHONGs.Where(t => t.SoLuongTreEm >= te).ToList();
            }
            else if (!String.IsNullOrEmpty(SLNL) && !String.IsNullOrEmpty(SLTE))
            {
                nl = int.Parse(SLNL);
                te = int.Parse(SLTE);
                lp = db.LOAIPHONGs.Where(t => t.SoLuongNguoiLon >= nl && t.SoLuongTreEm >= te).ToList();
            }
            foreach (var item in lp)
            {
                LoaiPhong _loaiphong = new LoaiPhong();
                _loaiphong.Id = item.Id;
                _loaiphong.Ten = item.TenLoai;
                _loaiphong.Mota = item.MoTa;
                _loaiphong.Gia = (double)item.Gia;
                _loaiphong.Hinhanh = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == item.Id).FirstOrDefault().imagePath;
                //_loaiphong.Id_P = (Int32.Parse(db.PHONGs.Where(a => a.Id_LP == item.Id).ToString()));
                data.Add(_loaiphong);
            }
            mymodel.LayLP = data;
            //mymodel.HA = db.HINHANH_LOAIPHONGs.ToList();
            return View(mymodel);
        }
        public ActionResult ThanhToanThanhCong()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckNgay(FormCollection f)
        {
            var ND = f["ngaydat"];
            var NT = f["ngaytra"];

            var ttdp = db.THONGTINDATPHONGs;
            var ctdp = db.CTDATPHONGs;
            List<ThongTinPhong> data = new List<ThongTinPhong>();
            foreach (var item in ttdp)
            {
                ThongTinPhong _thongtinphong = new ThongTinPhong();
                _thongtinphong.Id_DP = item.Id;
                _thongtinphong.Ngaydat = item.NgayDat.ToString();
                _thongtinphong.Ngaytra = item.NgayTra.ToString();
                data.Add(_thongtinphong);
            }
            foreach (var item in ctdp)
            {
                ThongTinPhong _thongtinphong = new ThongTinPhong();
                _thongtinphong.Id_CTDP = item.Id_DatPhong;
                _thongtinphong.Id_P = db.CTDATPHONGs.Select(t => t.Id_P).ToList();
                data.Add(_thongtinphong);
            }
            List<String> dsPhong = new List<String>();
            for (DateTime d = DateTime.Parse(ND); d.CompareTo(ND) <= d.CompareTo(NT); d = d.AddDays(1.0))
            {
                dsPhong.AddRange(data.Where(t => d.CompareTo(t.Ngaydat) >= 0 && d.CompareTo(t.Ngaytra) <= 0).Select(a => a.Id_P).ToHashSet().ToList().ConvertAll<string>(x => x.ToString()));

            }
            dsPhong = dsPhong.ToHashSet().ToList();
            List<String> dsPhongTrong = db.PHONGs.Where(t => !dsPhong.Contains(t.Id.ToString())).Select(t => t.Id.ToString()).ToList();

            dynamic mymodel = new ExpandoObject();
            mymodel.TTP = data;
            mymodel.DSPhong = dsPhong;
            mymodel.DSPT = dsPhongTrong;
            return View(mymodel);
        }
        public ActionResult ThanhToan()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThanhToan(string idlp, string strURL, FormCollection f)
        {
            //var idTK = Session["Id"] as TAIKHOAN;
            //var user = Session["TenTK"] as TAIKHOAN;
            //var id_KH = Session["Id_KH"] as KHACHHANG;
            //var khuyenmai = Session["KhuyenMai"] as UuDai;
            var user = Session["User"] as TAIKHOAN;
            if (user == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            DateTime ND = Convert.ToDateTime(f["ngaydat"]);
            DateTime NT = Convert.ToDateTime(f["ngaytra"]);
            //string idlp = f["idLP"];

            var ttdp = db.THONGTINDATPHONGs;
            var ctdp = db.CTDATPHONGs;
            List<ThongTinPhong> data = new List<ThongTinPhong>();
            foreach (var item in ttdp)
            {
                ThongTinPhong _thongtinphong = new ThongTinPhong();
                _thongtinphong.Id_DP = item.Id;
                _thongtinphong.Ngaydat = item.NgayDat.ToString();
                _thongtinphong.Ngaytra = item.NgayTra.ToString();
                //_thongtinphong.Id_CTDP = item.Id;
                _thongtinphong.Id_P = db.CTDATPHONGs.Where(r => r.Id_DatPhong == item.Id).Select(t => t.Id_P).ToList();
                data.Add(_thongtinphong);
            }
            List<String> dsPhong = new List<String>();
            for (DateTime d = ND; d.CompareTo(NT) <= 0; d = d.AddDays(1.0))
            {
                //dsPhong.AddRange(data.Where(t => d.CompareTo(t.Ngaydat) >= 0 && d.CompareTo(t.Ngaytra) <= 0).Select(a => a.Id_P).ToHashSet().ToList().ConvertAll<string>(x => x.ToString()));
                var booked = data.Where(t => d.CompareTo(DateTime.Parse(t.Ngaydat)) >= 0 && d.CompareTo(DateTime.Parse(t.Ngaytra)) <= 0).Select(c => c.Id_P).ToList();

                foreach (var dataCtDP in booked)
                {
                    dsPhong.AddRange(dataCtDP.ToHashSet().ToList().ConvertAll<string>(x => x.ToString()).ToList());
                }

            }
            dsPhong = dsPhong.ToHashSet().ToList();
            var loaiPhong = Session["loaiPhong"] as LOAIPHONG;
            var _p = db.PHONGs.Where(p => p.Id_LP == loaiPhong.Id).ToList();

            List<String> dsPhongTrong = _p.Where(t => !dsPhong.Contains(t.Id.ToString())).Select(t => t.Id.ToString()).ToList();
            Session["TB"] = null;
            if (dsPhongTrong.Count == 0)
            {
                Session["TB"] = "Het phong!!!";
                return Redirect(strURL);
            }
            var username = user.TenTK;
            var id_KH = user.Id;
            string soluong = f["txtsoluong"];

            var phong = db.PHONGs.Where(p => p.Id_LP == loaiPhong.Id).ToList();
            var id_DP = dsPhongTrong[new Random().Next(dsPhongTrong.Count)];
            //int idp = int.Parse(id_DP);
            var SLNL = loaiPhong.SoLuongNguoiLon;
            var SLTE = loaiPhong.SoLuongTreEm;
            var Gia = f["sum"];/// ngày ở   

            var kq = db.sp_AddTTDatPhong(username, SLNL, SLTE, double.Parse(Gia), ND, NT, ref id_DP);
            if (id_DP != null)
            {
                if (int.Parse(soluong) > dsPhongTrong.Count)
                {
                    Session["TB"] = "Qua So Luong!!!";  
                    return Redirect(strURL);
                }
                else
                {
                    for (int i = 1; i <= int.Parse(soluong); i++)
                    {
                        CTDATPHONG ct = new CTDATPHONG();
                        ct.Id_DatPhong = id_DP;
                        ct.Gia = double.Parse(Gia);
                        var lstPT = dsPhongTrong[new Random().Next(dsPhongTrong.Count)];
                        int idp = int.Parse(lstPT);
                        ct.Id_P = idp;
                        dsPhongTrong.Remove(lstPT);
                        db.CTDATPHONGs.InsertOnSubmit(ct);
                    }
                    db.SubmitChanges();
                }
            }
            return RedirectToAction("ThanhToanThanhCong", "Phong");

        }
    }
}