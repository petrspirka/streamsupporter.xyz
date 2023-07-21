using NewStreamSupporter.Data.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStreamSupporter.Data
{
    public class NotificationModel : BaseComponentModel
    {
        [NotMapped]
        public new string Name { get; set; } = "Notification";
    }
}
