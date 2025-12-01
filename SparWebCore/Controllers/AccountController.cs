using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SparWebCore.Controllers
{
    public class AccountController : Controller
    {
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
        public IActionResult RegisterFighter(SparWebCore.Models.RegisterFighterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-display the register view with validation errors
                return View(model);
            }

            // TODO: integrate with Identity/UserManager and repositories to create user and fighter.
            // For now, simulate success and show confirmation.
            return View("DisplayEmail");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterTrainer(SparWebCore.Models.RegisterTrainerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: integrate with Identity/UserManager and repositories to create user and trainer.
            return View("DisplayEmail");
        }
    }
}
