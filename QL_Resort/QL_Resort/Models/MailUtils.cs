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
        public static int SendMail(string _from, string _to, string _subject, SmtpClient client)
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

        public static int SendMailVerifyCode(string _to)
        {

            String _gmail_send = "noreply.pikanote@gmail.com";
            String _gmail_password = "koapeozuqczpyewz";
            String _subject = "Mã xác nhận: ";

            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmail_send, _gmail_password);
                client.EnableSsl = true;
                return MailUtils.SendMail(_gmail_send, _to, _subject, client);
            }

        }
    }

}