using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace QL_Resort.Models
{
    public class MailUtils
    {
        public static int SendMail(string _from, string _to, string _subject, SmtpClient client, int code)
        {
            // Tạo nội dung Email
            MailMessage message = new MailMessage(_from, _to);
            message.Subject = _subject;
            message.Body = $@"
                <html>
                    <body>
                        <table align=""center""  cellpadding=""0"" cellspacing=""0"" width=""600"" >
                            <tr>
                                <td style=""padding: 40px 0 30px 0;""><img src='https://snpresort.github.io/images/logo.png' alt=""Logo""  style=""display: block; width: 50%""></td>
                            </tr>
                            <tr>
                                <td>
                                    <div style=""margin: 0;"">
                                        <h1 style=""text-align: center; color: #000"">Xác nhận địa chỉ email của bạn</h1>
                                        <p style=""color: #000"">Mã xác minh của bạn</p>
                                        <div style=""padding: 5px 0px;margin: 10px 30%;background:#f5f4f5;"">
                                            <h2 style='text-align: center;text-align:center'>{code.ToString()}</h2>
                                        </div>
                                        <br />
                                        <br />
                                        <div style=""margin: 0 20%;"">Nếu bạn không yêu cầu email này, vui lòng bỏ qua nó.</div>
                                        <br />
                                        <br />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </body>
                </html>
            ";

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            try
            {
                client.Send(message);
                return code;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        
        public static bool Sendbooked(string _from, string _to, string _subject, SmtpClient client, string id_dp)
        {
            // Tạo nội dung Email
            MailMessage message = new MailMessage(_from, _to);
            message.Subject = _subject;
            message.Body = $@"
                <html>
                    <body>
                        <table align=""center""  cellpadding=""0"" cellspacing=""0"" width=""600"" >
                            <tr>
                                <td style=""padding: 40px 0 30px 0;""><img src='https://snpresort.github.io/images/logo.png' alt=""Logo""  style=""display: block; width: 50%""></td>
                            </tr>
                            <tr>
                                <td>
                                    <div style=""margin: 0;"">
                                        <h1 style=""text-align: center; color: #000"">Cảm ơn quý khách vì đã đặt phòng</h1>
                                        <h2 style=""color: #000"">Mã số đặt phòng của quý khách: <b style=""color: #ff914d"">{id_dp.ToString()}</b></h2>
                                    </div>
                                </td>
                            </tr>
                        </table>  
                    </body>
                </html>
            ";

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool AlertBookSuccess(string _from, string _to, string _subject, SmtpClient client, String id_dp, String tenLP, String giaPhong, String soNgayO, String soLuongNgLon, String soLuongTreEm)
        {
            Random _r = new Random();
            int code = _r.Next(10000, 99999);
            // Tạo nội dung Email
            MailMessage message = new MailMessage(_from, _to);
            message.Subject = _subject;
            message.Body = $@"
                <html>
                    <body>
                        <table align=""center""  cellpadding=""0"" cellspacing=""0"" width=""600"" >
                            <tr>
                                <td style=""padding: 40px 0 30px 0;""><img src='https://snpresort.github.io/images/logo.png' alt=""Logo""  style=""display: block; width: 50%""></td>
                            </tr>
                            <tr>
                                <td>
                                    <div style=""margin: 0;"">
                                        <h1 style=""text-align: center; color: #000"">Cảm ơn quý khách vì đã đặt phòng</h1>
                                        <h2 style=""color: #000"">Mã số đặt phòng của quý khách: <b style=""color: #ff914d"">{id_dp.ToString()}</b></h2>
                                        <h2 style=""color: #000"">Số ngày ở: {soNgayO}</h2>
                                        <h2 style=""color: #000"">Số lượng người lớn: {soLuongNgLon}</h2>
                                        <h2 style=""color: #000"">Số lượng trẻ em: {soLuongTreEm}</h2>
                                        <h2 style=""color: #000"">Thông tin đặt phòng</h2>

                                    </div>
                                </td>
                            </tr>
                            <tr>
                                 <table align=""center""  cellpadding=""10"" cellspacing=""10"" width=""600"" >
                                    <tr>
                                        <th>Phòng</th>                                     
                                        <th>Số lượng</th>
                                        <th>Giá</th>
                                    </tr>
                                    <tr>
                                        <td>{tenLP}</td>                                     
                                        <td>1</td>
                                        <td>{giaPhong}</td>
                                    </tr>
                                    <tr>
                                        <td>Tổng hoá đơn</td>                                     
                                        <td>1</td>
                                        <td>{giaPhong}</td>
                                    </tr>
                                </table>
                            </tr>
                            <tr>
                                <td>
                                    <div style=""margin: 0 20%;"">Nếu bạn không yêu cầu email này, vui lòng bỏ qua nó.</div>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>           
                    </body>
                </html>
            ";

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static bool VerifyBookSuccess(string _from, string _to, string _subject, SmtpClient client, String id_dp, String tenLP, String giaPhong, String soNgayO, String soLuongNgLon, String soLuongTreEm)
        {
            Random _r = new Random();
            int code = _r.Next(10000, 99999);
            // Tạo nội dung Email
            MailMessage message = new MailMessage(_from, _to);
            message.Subject = _subject;
            message.Body = $@"
                <html>
                    <body>
                        <table align=""center""  cellpadding=""0"" cellspacing=""0"" width=""600"" >
                            <tr>
                                <td style=""padding: 40px 0 30px 0;""><img src='https://snpresort.github.io/images/logo.png' alt=""Logo""  style=""display: block; width: 50%""></td>
                            </tr>
                            <tr>
                                <td>
                                    <div style=""margin: 0;"">
                                        <h1 style=""text-align: center; color: #000"">Cảm ơn quý khách vì đã đặt phòng</h1>
                                        <h2 style=""color: #000"">Mã số đặt phòng của quý khách: <b style=""color: #ff914d"">{id_dp.ToString()}</b></h2>
                                        <h2 style=""color: #000"">Số ngày ở: {soNgayO}</h2>
                                        <h2 style=""color: #000"">Số lượng người lớn: {soLuongNgLon}</h2>
                                        <h2 style=""color: #000"">Số lượng trẻ em: {soLuongTreEm}</h2>
                                        <h2 style=""color: #000"">Thông tin đặt phòng</h2>

                                    </div>
                                </td>
                            </tr>
                            <tr>
                                 <table align=""center""  cellpadding=""10"" cellspacing=""10"" width=""600"" >
                                    <tr>
                                        <th>Phòng</th>                                     
                                        <th>Số lượng</th>
                                        <th>Giá</th>
                                    </tr>
                                    <tr>
                                        <td>{tenLP}</td>                                     
                                        <td>1</td>
                                        <td>{giaPhong}</td>
                                    </tr>
                                    <tr>
                                        <td>Tổng hoá đơn</td>                                     
                                        <td>1</td>
                                        <td>{giaPhong}</td>
                                    </tr>
                                </table>
                            </tr>
                            <tr>
                                <td>
                                    <div style=""margin: 0 20%;"">Nếu bạn không yêu cầu email này, vui lòng bỏ qua nó.</div>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>           
                    </body>
                </html>
            ";

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static int SendMailVerifyCode(string _to)
        {

            String _gmail_send = "noreply.pikanote@gmail.com";
            String _gmail_password = "koapeozuqczpyewz";

            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmail_send, _gmail_password);
                client.EnableSsl = true;

                Random _r = new Random();
                int code = _r.Next(10000, 99999);

                String _subject = String.Format("Mã xác nhận: {0}", code);
                return MailUtils.SendMail(_gmail_send, _to, _subject, client, code);
            }

        }

        public static bool SendMailBookSuccess(string _to, String id_dp, String tenLP, String giaPhong, String soNgayO, String soLuongNgLon, String soLuongTreEm)
        {

            String _gmail_send = "noreply.pikanote@gmail.com";
            String _gmail_password = "koapeozuqczpyewz";

            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmail_send, _gmail_password);
                client.EnableSsl = true;

                String _subject = "Thông báo đặt phòng thành công";
                return MailUtils.AlertBookSuccess(_gmail_send, _to, _subject, client, id_dp, tenLP, giaPhong, soNgayO, soLuongNgLon, soLuongTreEm);
            }

        }
        public static bool SendMailVerifyBookSuccess(string _to, String id_dp, String tenLP, String giaPhong, String soNgayO, String soLuongNgLon, String soLuongTreEm)
        {

            String _gmail_send = "noreply.pikanote@gmail.com";
            String _gmail_password = "koapeozuqczpyewz";

            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmail_send, _gmail_password);
                client.EnableSsl = true;

                String _subject = "Xác nhận đặt phòng";
                return MailUtils.VerifyBookSuccess(_gmail_send, _to, _subject, client, id_dp, tenLP, giaPhong, soNgayO, soLuongNgLon, soLuongTreEm);
            }

        }
        public static bool SendMailBookSuccess(string _to, String id_dp)
        {

            String _gmail_send = "noreply.pikanote@gmail.com";
            String _gmail_password = "koapeozuqczpyewz";

            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmail_send, _gmail_password);
                client.EnableSsl = true;

                String _subject = "Xác nhận đặt phòng";
                return MailUtils.Sendbooked(_gmail_send, _to, _subject, client, id_dp);
            }

        }
    }

}