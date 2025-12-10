using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SparWebCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<SparWebCore.Models.ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<SparWebCore.Models.ApplicationUser> _signInManager;

        public AccountController(
            Microsoft.AspNetCore.Identity.UserManager<SparWebCore.Models.ApplicationUser> userManager,
            Microsoft.AspNetCore.Identity.SignInManager<SparWebCore.Models.ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Lightweight stub used during migration to supply the register popup HTML
        [HttpGet]
        public IActionResult GetRegisterPopupModal()
        {
            // Return the Razor partial so markup can be maintained in a view file
            return PartialView("_RegisterPopupModal");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(int? m)
        {
            // m=1 -> Fighter; m=2 -> Trainer
            return View(m);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterFighter()
        {
            // Stubbed page for migration; real form implementation will be added later.
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterTrainer()
        {
            // Stubbed page for migration; real form implementation will be added later.
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> RegisterFighter(SparWebCore.Models.RegisterFighterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-display the register view with validation errors
                return View(model);
            }

            var user = new SparWebCore.Models.ApplicationUser
            {
                UserName = model.UserName,
                Email = model.UserName,
                Name = model.Name,
                City = model.City,
                State = model.State,
                GymName = model.GymName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }

            // Optionally sign in the user immediately
            await _signInManager.SignInAsync(user, isPersistent: false);

            return View("DisplayEmail", model.UserName);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> RegisterTrainer(SparWebCore.Models.RegisterTrainerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new SparWebCore.Models.ApplicationUser
            {
                UserName = model.UserName,
                Email = model.UserName,
                Name = model.Name,
                City = model.City,
                State = model.State,
                GymName = model.GymName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return View("DisplayEmail", model.UserName);
        }
    }
}
