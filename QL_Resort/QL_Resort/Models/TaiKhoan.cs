using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QL_Resort.Models
{    public class TaiKhoan
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string UserName { get; set; }

        [RegularExpression("^(?=.*[0-9])(?=.*[a-z]).{8,20}$",
        ErrorMessage = "Your password must contain at least 1 numberic and non numberic and a length of at least 8 characters and a maximum of 20 characters!")]
        [Required(ErrorMessage = "Enter Password")]
        public string Pass { get; set; }


        [Required(ErrorMessage = "Comfirm your password")]
        public string RePass { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập CCCD")]
        public string Cccd { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression("(84|0[3|5|7|8|9])+([0-9]{8})", ErrorMessage = "You have entered an invalid phone number!")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Enter your email")]
        [EmailAddress(ErrorMessage = "You have entered an invalid email address!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter your sex")]
        public bool Gioitinh { get; set; }

        [Required(ErrorMessage = "Enter your address")]
        public string DiaChi { get; set; }
        
    }
}