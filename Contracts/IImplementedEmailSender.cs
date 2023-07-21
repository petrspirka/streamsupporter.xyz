using Microsoft.AspNetCore.Identity.UI.Services;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní sloužící pro specifikaci toho, že máme funkčního emailového klienta
    /// </summary>
    public interface IImplementedEmailSender : IEmailSender
    {
    }
}
