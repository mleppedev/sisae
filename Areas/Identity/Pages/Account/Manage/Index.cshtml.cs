using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace sisae.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
            [Phone(ErrorMessage = "El número de teléfono ingresado no es válido.")]
            [Display(Name = "Número de Teléfono")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Correo Electrónico")]
            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
            public string Email { get; set; }

            [Display(Name = "Correo Confirmado")]
            public bool EmailConfirmed { get; set; }

            [Display(Name = "Teléfono Confirmado")]
            public bool PhoneNumberConfirmed { get; set; }

            [Display(Name = "Autenticación de Dos Factores")]
            public bool TwoFactorEnabled { get; set; }

            [Display(Name = "Habilitar Bloqueo")]
            public bool LockoutEnabled { get; set; }

            [Display(Name = "Intentos Fallidos")]
            public int AccessFailedCount { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = await _userManager.GetUserNameAsync(user);

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.Email = Input.Email;
            user.PhoneNumber = Input.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Tu perfil ha sido actualizado correctamente.";
            return RedirectToPage();
        }
    }
}
