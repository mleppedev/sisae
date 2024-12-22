// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace sisae.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
        ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
        ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
        ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
        ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
        ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
            ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
            ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     Esta API respalda la infraestructura de la interfaz de usuario predeterminada de ASP.NET Core Identity y no está destinada a 
            ///     usarse directamente desde tu código. Esta API puede cambiar o eliminarse en versiones futuras.
            /// </summary>
            [Display(Name = "¿Recordarme?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Limpiar la cookie externa existente para asegurar un proceso de inicio de sesión limpio
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Esto no cuenta los errores de inicio de sesión a
                // hacia el bloqueo de cuenta.
                // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, establezca lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario inició sesión.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Cuenta de usuario bloqueada.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    return Page();
                }
            }

            // Si llegamos hasta aquí, algo falló, volver a mostrar el formulario
            return Page();
        }
    }
}
