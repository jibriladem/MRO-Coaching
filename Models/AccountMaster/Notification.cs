using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MROCoatching.DataObjects.Models.AccountMaster
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [ForeignKey("SenderUser")]
        [Display(Name = "Sender")]
        public string SenderId { get; set; }

        [ForeignKey("ReceiverUser")]
        [Display(Name = "Receiver")]
        public string ReceiverId { get; set; }

        [Display(Name = "Notification Type")]
        public NotificationType NotificationType { get; set; }

        [Display(Name = "Notification State")]
        public NotificationState NotificationState { get; set; }

        [Display(Name = "Notification Subject")]
        public string Subject { get; set; }

        [Display(Name = "Notification Description")]
        public string Description { get; set; }

        [Display(Name = "Object URL")]
        public string ObjectURL { get; set; }

        [Display(Name = "Time Sent")]
        public DateTime TimeSent { get; set; }
        public virtual ApplicationUser SenderUser { get; set; }
        public virtual ApplicationUser ReceiverUser { get; set; }
    }

    public enum NotificationType
    {
        [Display(Name ="BSC Approval Request")]
        BSC_Approval_Request,
        [Display(Name = "BSC Self Asessment Request")]
        BSC_Self_Asessment_Request,
    }

    public enum NotificationState
    {
        READ,
        SEEN,
        UNREAD
    }
}
