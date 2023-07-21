namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Record obsahující informaci o uživateli na nějaké platformě.
    /// </summary>
    /// <param name="Id">Id uživatele v rámci dané platformy</param>
    /// <param name="Name">Jméno uživatele v rámci dané platformy</param>
    /// <param name="Platform">Platforma, na které se uživatel nachází</param>
    public record PlatformUser(string Id, string Name, Platform Platform);
}
