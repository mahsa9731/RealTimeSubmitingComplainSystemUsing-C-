using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public enum ComplaintStatus
    {
        [Display(Name = "ثبت شده")]
        New,
        [Display(Name = "در دست بررسی")]
        InProgress,
        [Display(Name = "بررسی شده و در انتظار پاسخ کاربر")]
        WaitingForUserResponse,
        [Display(Name = " در انتظار پاسخ ادمین")]
        WaitingForAdminResponse,
        [Display(Name = "بسته شده")]
        Resolved

    }
    public class ComplaintModel
    {

        [Key]
        public int ComplaintId { get; set; }
        [Required(ErrorMessage = "ثبت نام و نام خانوادگی الزامی است.")]
        public string UserName { get; set; }
        public string UserId { get; set; } = null;
        public string TrackingCode { get; set; } = null;
        [Required(ErrorMessage = "ثبت عنوان شکایت الزامی است.")]
        public string TitleOfComplaint { get; set; }
        [Required(ErrorMessage = "ارائه توضیحات الزامی است.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "لطفا کد مرسوله دریافتی خود را وارد کنید.")]
        public string PackageCode { get; set; } = null;

        public DateTime DateOfCreateComplaint{ get; set; }
        public ComplaintStatus Status { get; set; } = new();

        public List<ComplaintMessageModel> Messages { get; set; } = new List<ComplaintMessageModel>();


    }
}
