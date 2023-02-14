using CrystalDecisions.CrystalReports.Engine;
using QL_Resort.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_Resort.Controllers
{
    public class CustomerController : Controller
    {
        //DbContext  
        QL_ResortDataContext db = new QL_ResortDataContext();
        // GET: Customer  
        public ActionResult Index(string idhd)
        {
            var hd = db.HOADONs.Where(t => t.Id == idhd).FirstOrDefault();
            var ttdp = db.THONGTINDATPHONGs
                .Where(dp => dp.Id == hd.Id_DP)
                .Select(dp => new ThongTinDP
                {
                    Gia = dp.DonGia ?? 0,
                    NgDat = dp.NgayDat.ToString(),
                    NgTra = dp.NgayTra.ToString(),
                    SoLuongNgLon = dp.SoLuongNguoiTH.ToString(),
                    SoLuongTreEm = dp.SoLuongTreEm.ToString(),
                    Id = dp.Id
                }).FirstOrDefault();

            /*
             * select Id_DatPhong, CTDATPHONG.Gia, TenLoai, count(*) SoLuong 
                from CTDATPHONG join Phong
                    on CTDATPHONG.Id_P = Phong.Id join LoaiPhong
                    on LoaiPhong.Id = Phong.Id_LP
                Group by Id_DatPhong, CTDATPHONG.Gia, TenLoai
             */
            var ctttlp = db.LOAIPHONGs.Select(lp => new ChiTietTTLP
            {
                Id = lp.Id,
                Gia = lp.Gia ?? 0,
                Ten = lp.TenLoai,
                Id_p = db.PHONGs.Where(p => p.Id_LP == lp.Id).Select(p => p.Id).ToList()
            }).ToList();

            var ctdpgr = db.CTDATPHONGs.ToList().Select(ct => new CtDP
            {
                Gia = ct.Gia.ToString(),
                Id_DatPhong = ct.Id_DatPhong.ToString(),
                TenLoai = ctttlp.Where(pp => pp.Id_p.Contains(ct.Id_P)).FirstOrDefault().Ten,
            }).ToList()
            .GroupBy(tt => new
            {
                tt.Id_DatPhong,
                tt.TenLoai,
                tt.Gia
            }).ToList().Select(tt => new CtDP
            {
                Gia = tt.Key.Gia,
                Id_DatPhong = tt.Key.Id_DatPhong,
                SoLuong = tt.Count().ToString(),
                TenLoai = tt.Key.TenLoai,
            }).ToList();



            //foreach (var line in )
            //{
            //    var a = line.Count();
            //}

            dynamic mymodel = new ExpandoObject();

            // thông tin dp
            mymodel.ttdp = new ThongTinDP
            {
                Gia = ttdp.Gia,
                NgDat = ttdp.NgDat,
                NgTra = ttdp.NgTra,
                SoLuongNgLon = ttdp.SoLuongNgLon,
                SoLuongTreEm = ttdp.SoLuongTreEm,
                Id = ttdp.Id,
                Ctdp = ctdpgr
                    .Where(ct => ct.Id_DatPhong == ttdp.Id)
                    .Select(ct => new CtDP
                    {
                        Gia = (double.Parse(ct.Gia) * int.Parse(ct.SoLuong) * (1 + Math.Floor(double.Parse((DateTime.Parse(ttdp.NgTra) - DateTime.Parse(ttdp.NgDat)).Days.ToString())))).ToString(),
                        Id_DatPhong = ct.Id_DatPhong,
                        SoLuong = ct.SoLuong,
                        TenLoai = ct.TenLoai,
                    })
                    .ToList(),
            };

            mymodel.HD = hd;
            var idtknv = db.NHANVIENs.Where(x => x.Id == hd.Id_NV).FirstOrDefault().Id_tk;
            mymodel.usernv= db.THONGTINCANHANs.Where(ttcn => ttcn.Id_tk == idtknv).FirstOrDefault();

            var dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == hd.Id_DP).Select(a => a.Id_P).ToList();

            List<string> dsLoaiP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.Id_LP).ToList();

            // rooms list booked
            mymodel.dsP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.TenPhong).ToList();

            // info user
            var user_id = db.THONGTINDATPHONGs.Where(tt => tt.Id == hd.Id_DP).FirstOrDefault().Id_KH;
            mymodel.userInfo = db.THONGTINCANHANs.Where(ttcn => ttcn.Id_tk == user_id).FirstOrDefault();


            Session["ctdp"] = mymodel.ttdp as ThongTinDP;


            return View(mymodel);
        }


        public ActionResult ExportCustomers(string idhd)
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report/XuatHD.rpt")));
            var hd = db.HOADONs.Where(t => t.Id == idhd).FirstOrDefault();
            var ttdp = db.THONGTINDATPHONGs
                .Where(dp => dp.Id == hd.Id_DP)
                .Select(dp => new ThongTinDP
                {
                    Gia = dp.DonGia ?? 0,
                    NgDat = dp.NgayDat.ToString(),
                    NgTra = dp.NgayTra.ToString(),
                    SoLuongNgLon = dp.SoLuongNguoiTH.ToString(),
                    SoLuongTreEm = dp.SoLuongTreEm.ToString(),
                    Id = dp.Id
                }).FirstOrDefault();

            var ctttlp = db.LOAIPHONGs.Select(lp => new ChiTietTTLP
            {
                Id = lp.Id,
                Gia = lp.Gia ?? 0,
                Ten = lp.TenLoai,
                Id_p = db.PHONGs.Where(p => p.Id_LP == lp.Id).Select(p => p.Id).ToList()
            }).ToList();

            var ctdpgr = db.CTDATPHONGs.ToList().Select(ct => new CtDP
            {
                Gia = ct.Gia.ToString(),
                Id_DatPhong = ct.Id_DatPhong.ToString(),
                TenLoai = ctttlp.Where(pp => pp.Id_p.Contains(ct.Id_P)).FirstOrDefault().Ten,
            }).ToList()
            .GroupBy(tt => new
            {
                tt.Id_DatPhong,
                tt.TenLoai,
                tt.Gia
            }).ToList().Select(tt => new CtDP
            {
                Gia = tt.Key.Gia,
                Id_DatPhong = tt.Key.Id_DatPhong,
                SoLuong = tt.Count().ToString(),
                TenLoai = tt.Key.TenLoai,
            }).ToList();
            dynamic mymodel = new ExpandoObject();

            // thông tin dp
            mymodel.ttdp = new ThongTinDP
            {
                Gia = ttdp.Gia,
                NgDat = ttdp.NgDat,
                NgTra = ttdp.NgTra,
                SoLuongNgLon = ttdp.SoLuongNgLon,
                SoLuongTreEm = ttdp.SoLuongTreEm,
                Id = ttdp.Id,
                Ctdp = ctdpgr
                    .Where(ct => ct.Id_DatPhong == ttdp.Id)
                    .Select(ct => new CtDP
                    {
                        Gia = (double.Parse(ct.Gia) * int.Parse(ct.SoLuong) * (1 + Math.Floor(double.Parse((DateTime.Parse(ttdp.NgTra) - DateTime.Parse(ttdp.NgDat)).Days.ToString())))).ToString(),
                        Id_DatPhong = ct.Id_DatPhong,
                        SoLuong = ct.SoLuong,
                        TenLoai = ct.TenLoai,
                    })
                    .ToList(),
            };

            mymodel.HD = hd;
            var idtknv = db.NHANVIENs.Where(x => x.Id == hd.Id_NV).FirstOrDefault().Id_tk;
            mymodel.usernv = db.THONGTINCANHANs.Where(ttcn => ttcn.Id_tk == idtknv).FirstOrDefault();
            var nv= db.THONGTINCANHANs.Where(ttcn => ttcn.Id_tk == idtknv).FirstOrDefault();

            var dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == hd.Id_DP).Select(a => a.Id_P).ToList();

            List<string> dsLoaiP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.Id_LP).ToList();

            // rooms list booked
            mymodel.dsP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.TenPhong).ToList();

            // info user
            var user_id = db.THONGTINDATPHONGs.Where(tt => tt.Id == hd.Id_DP).FirstOrDefault().Id_KH;
            mymodel.userInfo = db.THONGTINCANHANs.Where(ttcn => ttcn.Id_tk == user_id).FirstOrDefault();
            var kh= db.THONGTINCANHANs.Where(ttcn => ttcn.Id_tk == user_id).FirstOrDefault();

            rd.SetParameterValue("@NgayTao", hd.NgayTao);
            rd.SetParameterValue("@NgayDat", ttdp.NgDat);
            rd.SetParameterValue("@NgayTra", ttdp.NgTra);
            rd.SetParameterValue("@TongTien", ttdp.Gia);
            rd.SetParameterValue("@HoTenNV", nv.HoTen);
            rd.SetParameterValue("@HoTenKH", kh.HoTen);
            //rd.SetParameterValue("@TenLoai", );

            //rd.SetDataSource(ttlp);
            rd.SetDatabaseLogon("tuhueson", "123456789", "qlresort.mssql.somee.com", "qlresort");

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "HoaDon.pdf");
        }

    }
}