using MedicoApp.Core.Application.Interfaces.Services;
using MedicoApp.Core.Application.Services;
using MedicoApp.Core.Application.ViewModels.Paciente;
using MedicoApp.Core.Application.ViewModels.PruebaDeLab;
using Microsoft.AspNetCore.Mvc;
using WebApp.MedicoApp.Middelwares;

namespace WebApp.MedicoApp.Controllers
{
    public class PruebaDeLabController : Controller
    {
        private readonly IPruebaDeLabService _pruebaDeLabService;
        private readonly IResultDeLabService _resultDeLabService;
        private readonly ValidateUserSession _validateUserSession;

        public PruebaDeLabController(IPruebaDeLabService pruebaDeLabService, ValidateUserSession validateUserSession, IResultDeLabService resultDeLabService)
        {
            _pruebaDeLabService = pruebaDeLabService;
            _resultDeLabService = resultDeLabService;
            _validateUserSession = validateUserSession;
        }
        public async Task<IActionResult> Index()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View(await _pruebaDeLabService.GetAllViewModel());
        }

        public IActionResult Create()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View("SavePruebaDeLab", new SavePruebaDeLabViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePruebaDeLabViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View("SavePruebaDeLab", vm);
            }

            await _pruebaDeLabService.Add(vm);
            return RedirectToRoute(new { controller = "PruebaDeLab", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View("SavePruebaDeLab", await _pruebaDeLabService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SavePruebaDeLabViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View("SavePruebaDeLab", vm);
            }

            await _pruebaDeLabService.Update(vm);
            return RedirectToRoute(new { controller = "PruebaDeLab", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View(await _pruebaDeLabService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }

            await _pruebaDeLabService.Delete(id);
            return RedirectToRoute(new { controller = "PruebaDeLab", action = "Index" });
        }

    }
}

