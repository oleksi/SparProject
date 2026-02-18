using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SparWebCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<SparWebCore.Models.ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<SparWebCore.Models.ApplicationUser> _signInManager;
        private readonly SparWebCore.Data.ApplicationDbContext _dbContext;

        public AccountController(
            Microsoft.AspNetCore.Identity.UserManager<SparWebCore.Models.ApplicationUser> userManager,
            Microsoft.AspNetCore.Identity.SignInManager<SparWebCore.Models.ApplicationUser> signInManager,
            SparWebCore.Data.ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
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
            PopulateViewBagForRegistration();
            return View(new SparWebCore.Models.RegisterFighterViewModel { Sex = true, IsSouthpaw = false });
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
                PopulateViewBagForRegistration();
                return View(model);
            }

            System.DateTime dateOfBirth;
            try
            {
                dateOfBirth = new System.DateTime(model.DateOfBirth.Year, model.DateOfBirth.Month, model.DateOfBirth.Day);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                ModelState.AddModelError("DateOfBirth", "Please enter a valid date of birth");
                PopulateViewBagForRegistration();
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
                PopulateViewBagForRegistration();
                return View(model);
            }

            var now = System.DateTime.UtcNow;
            var fighter = new SparWebCore.Models.Fighter
            {
                Name = model.Name,
                Sex = model.Sex,
                DateOfBirth = dateOfBirth,
                City = model.City,
                State = model.State,
                Height = model.Height,
                Weight = model.Weight,
                IsSouthpaw = model.IsSouthpaw,
                NumberOfAmateurFights = model.NumberOfAmateurFights,
                NumberOfProFights = model.NumberOfProFights,
                Rate = model.Rate,
                Comments = model.Comments,
                AspNetUserId = user.Id,
                ProfilePictureUploaded = false,
                InsertDate = now,
                UpdateDate = now,
                IsDemo = false
            };

            _dbContext.Fighters.Add(fighter);
            await _dbContext.SaveChangesAsync();

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

        private void PopulateViewBagForRegistration()
        {
            ViewBag.HeightToCentimetersMap = SparWebCore.Models.Util.HeightToCentimetersMap;
            ViewBag.WeightClassMap = SparWebCore.Models.Util.WeightClassMap;
            ViewBag.States = SparWebCore.Models.Util.States;
        }
    }
}
