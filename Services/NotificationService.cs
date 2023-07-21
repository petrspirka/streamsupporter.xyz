using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;

namespace NewStreamSupporter.Services
{
    /// <summary>
    /// Služba obstarávající správu notifikací
    /// </summary>
    public class NotificationService
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Vytoří novou instanci třídy NotificationService
        /// </summary>
        /// <param name="context">Kontext pro komunikaci s databází</param>
        public NotificationService(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Přidá novou notifikaci uživateli
        /// </summary>
        /// <param name="userId">Id uživatele</param>
        /// <param name="message">Zpráva uživateli</param>
        /// <param name="color">Barva pozadí notifikace, defaultně bílá</param>
        /// <exception cref="ArgumentException">Pokud daný uživatel neexistuje</exception>
        public async Task AddNotification(string userId, string message, string color = "#FFFFFFFF")
        {
            ApplicationUser user = await _context.Users.Include(u => u.Notifications).Where(u => u.Id == userId).FirstOrDefaultAsync() ?? throw new ArgumentException("The specified user does not exist");
            NotificationModel notification = new()
            {
                Owner = user,
                BackgroundColor = color,
                Text = message,
            };
            user.Notifications.Add(notification);
            _context.SaveChanges();
        }
    }
}
