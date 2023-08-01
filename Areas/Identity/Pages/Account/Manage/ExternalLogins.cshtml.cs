// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Services;
using System.Security.Claims;

namespace NewStreamSupporter.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IYouTubeProviderService _youtubeProviderService;
        private readonly IDataStore _dataStore;
        private readonly ICurrencyService _currencyService;
        private readonly TwitchListenerService _twitchService;
        private readonly YouTubeListenerService _youtubeService;
        private readonly NotificationService _notificationService;

        public ExternalLoginsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserStore<ApplicationUser> userStore,
            IYouTubeProviderService youtubeProviderService,
            IDataStore dataStore,
            ICurrencyService currencyService,
            TwitchListenerService twitchService,
            YouTubeListenerService youtubeService,
            NotificationService notificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _youtubeProviderService = youtubeProviderService;
            _dataStore = dataStore;
            _currencyService = currencyService;
            _twitchService = twitchService;
            _youtubeService = youtubeService;
            _notificationService = notificationService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> OtherLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool ShowRemoveButton { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            string passwordHash = null;
            if (_userStore is IUserPasswordStore<ApplicationUser> userPasswordStore)
            {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
            }

            ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IdentityResult result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                StatusMessage = "The external login was not removed.";
                return RedirectToPage();
            }
            string oldId;
            switch (loginProvider)
            {
                case "Twitch":
                    oldId = user.TwitchId;
                    user.TwitchId = null;
                    user.TwitchUsername = null;
                    user.TwitchAccessToken = null;
                    user.TwitchRefreshToken = null;
                    user.TwitchAccessTokenExpiry = null;
                    if (user.IsTwitchActive)
                    {
                        await _twitchService.RemoveAllUserListeners(oldId);
                        user.IsTwitchActive = false;
                        await _notificationService.AddNotification(user.Id, "You have removed your external login to Twitch. Your settings have been adjusted accordingly.", "#FF0000FF");
                    }
                    break;
                case "Google":
                    await _dataStore.DeleteAsync<TokenResponse>(user.GoogleBrandId);
                    oldId = user.GoogleBrandId;
                    user.GoogleBrandId = null;
                    user.GoogleBrandName = null;
                    if (user.IsGoogleActive)
                    {
                        await _youtubeService.RemoveAllUserListeners(oldId);
                        user.IsGoogleActive = false;
                        await _notificationService.AddNotification(user.Id, "You have removed your external login to Google. Your settings have been adjusted accordingly.", "#FF0000FF");
                    }
                    break;
            }
            await _userStore.UpdateAsync(user, new CancellationToken());

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "The external login was removed.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            string redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string userId = await _userManager.GetUserIdAsync(user);
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(userId);
            if (info == null)
            {
                throw new InvalidOperationException($"Unexpected error occurred loading external login info.");
            }

            IdentityResult result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                StatusMessage = "The external login was not added. External logins can only be associated with one account.";
                return RedirectToPage();
            }

            switch (info.LoginProvider)
            {
                case "Twitch":
                    user.TwitchId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                    user.TwitchUsername = info.Principal.FindFirst(ClaimTypes.Name).Value;
                    user.TwitchAccessToken = info.AuthenticationTokens.Where(token => token.Name == "access_token").First().Value;
                    user.TwitchRefreshToken = info.AuthenticationTokens.Where(token => token.Name == "refresh_token").First().Value;
                    user.TwitchAccessTokenExpiry = DateTime.Parse(info.AuthenticationTokens.Where(token => token.Name == "expires_at").First().Value);
                    if (user.IsTwitchActive)
                    {
                        await _twitchService.AddAllUserListeners(user.TwitchId);
                    }
                    break;
                case "Google":
                    if (!info.AuthenticationTokens.Any(t => t.Name == "refresh_token") || !info.AuthenticationTokens.Any(t => t.Name == "access_token"))
                    {
                        return RedirectToPage("/Account/GoogleError");
                    }
                    string googleAccessToken = info.AuthenticationTokens.Where(token => token.Name == "access_token").First().Value;
                    string googleRefreshToken = info.AuthenticationTokens.Where(token => token.Name == "refresh_token").First().Value;
                    DateTime GoogleAccessTokenExpiry = DateTime.Parse(info.AuthenticationTokens.Where(token => token.Name == "expires_at").First().Value);
                    Models.PlatformUser youtubeInfo;
                    try
                    {
                        youtubeInfo = await _youtubeProviderService.GetYoutubeInfoForBrandAccount(googleAccessToken);
                    }
                    catch (Exception)
                    {
                        StatusMessage = "Error fetching YouTube info, make sure you've granted the app valid permissions";
                        return RedirectToPage();
                    }

                    if (youtubeInfo == null)
                    {
                        StatusMessage = "It seems you do not have a valid YouTube account. Create one by heading over to YouTube, choosing your profile and selecting \"Create Channel\"";
                        return RedirectToPage();
                    }
                    else
                    {
                        user.GoogleBrandId = youtubeInfo.Id;
                        user.GoogleBrandName = youtubeInfo.Name;
                    }
                    await _youtubeProviderService.SaveUserCredential(user.GoogleBrandId, googleRefreshToken);
                    if(user.IsGoogleActive) {
                        await _youtubeService.AddAllUserListeners(user.GoogleBrandId);
                    }
                    break;
            }

            IdentityResult externalIdResult = await _userStore.UpdateAsync(user, new CancellationToken());
            if (!externalIdResult.Succeeded)
            {
                StatusMessage = "Failed adding account information to user.";
            }
            else
            {
                await _currencyService.Pair(user);
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToPage();
        }
    }
}
