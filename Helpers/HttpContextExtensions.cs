using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using System.Security.Claims;

namespace NewStreamSupporter.Helpers
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Statická metoda sloužící pro získání UserId z kontextu
        /// </summary>
        /// <param name="context">HttpContext, ze kterého se má získat Id</param>
        /// <returns>Id uživatele, pokud je přihlášen</returns>
        /// <exception cref="UnauthorizedAccessException">Pokud není v HttpContextu žádný uživatel</exception>
        public static string GetUserId(this HttpContext context)
        {
            IEnumerable<Claim> claims = context.User.Claims;
            Claim? nameIdentifierClaim = claims.Where(claim => claim.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            if (nameIdentifierClaim == null)
            {
                throw new UnauthorizedAccessException("NameIdentifier claim missing");
            }
            return nameIdentifierClaim.Value;
        }

        /// <summary>
        /// Statická metoda sloužící pro získání uživatele
        /// </summary>
        /// <param name="context">HttpContext, ze kterého se má získat Id</param>
        /// <param name="dataContext">Instance ApplicationContext sloužící pro interakci s databází</param>
        /// <returns>Instance ApplicationUser</returns>
        /// <exception cref="UnauthorizedAccessException">Pokud není v HttpContextu žádný uživatel</exception>
        public static async Task<ApplicationUser> GetUser(this HttpContext context, ApplicationContext dataContext)
        {
            string userId = context.GetUserId();
            ApplicationUser? user = await dataContext.Users.Where(user => user.Id == userId).Include(user => user.OwnedMarquees).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Used is currently unauthorized");
            }
            return user;
        }
    }
}
