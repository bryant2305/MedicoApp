using MedicoApp.Core.Application.Interfaces.Services;
using MedicoApp.Core.Application.Services;
using MedicoApp.Core.Application.ViewModels.Citas;
using MedicoApp.Core.Application.ViewModels.ResultDeLab;
using Microsoft.AspNetCore.Mvc;
using WebApp.MedicoApp.Middelwares;

namespace WebApp.MedicoApp.Controllers
{
    public class ResultDeLabController : Controller
    {

        private readonly IPacientesService _pacientesService;
        private readonly IPruebaDeLabService _pruebaDeLabService;
        private readonly IResultDeLabService _resultDeLabService;
        private readonly ValidateUserSession _validateUserSession;


        public ResultDeLabController(IPacientesService pacientesService, IResultDeLabService resultDeLabService, ValidateUserSession validateUserSession, IPruebaDeLabService pruebaDeLabService)
        {

            _pruebaDeLabService = pruebaDeLabService;
            _pacientesService = pacientesService;
            _resultDeLabService = resultDeLabService;
            _validateUserSession = validateUserSession;
        }
        public async Task<IActionResult> Index(FilterResultDeLabViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            ViewBag.Pacientes = await _pacientesService.GetAllViewModel();
            ViewBag.PruebaDeLab = await _pruebaDeLabService.GetAllViewModel();
            return View(await _resultDeLabService.GetAllViewModelWithFilters(vm));
        }
        [HttpPost]
        public async Task<IActionResult> Index(String Busqueda = null, int Id = 0)
        {
            ViewBag.Pacientes = await _pacientesService.GetAllViewModel();
            ViewBag.PruebaDeLab = await _pruebaDeLabService.GetAllViewModel();
            ViewBag.Busqueda = Busqueda;
            return View(await _resultDeLabService.GetAllViewModel());

        }
        public async Task<IActionResult> Create()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }

            SaveResultDeLabViewModel vm = new();
            vm.PacinetesList = await _pacientesService.GetAllViewModel();
            vm.PruebaDeLabList = await _pruebaDeLabService.GetAllViewModel();
            return View("SaveResultDeLab", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveResultDeLabViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                vm.PacinetesList = await _pacientesService.GetAllViewModel();
                vm.PruebaDeLabList = await _pruebaDeLabService.GetAllViewModel();
                return View("SaveResultDeLab", vm);
            }

            await _resultDeLabService.Add(vm);
            return RedirectToRoute(new { controller = "ResultDeLab", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            SaveResultDeLabViewModel vm = await _resultDeLabService.GetByIdSaveViewModel(id);

            vm.PacinetesList = await _pacientesService.GetAllViewModel();
            vm.PruebaDeLabList = await _pruebaDeLabService.GetAllViewModel();
            return View("SaveResultDeLab", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveResultDeLabViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {

                vm.PacinetesList = await _pacientesService.GetAllViewModel();
                vm.PruebaDeLabList = await _pruebaDeLabService.GetAllViewModel();
                return View("SaveResultDeLab", vm);
            }

            await _resultDeLabService.Update(vm);
            return RedirectToRoute(new { controller = "ResultDeLab", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View(await _resultDeLabService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            await _resultDeLabService.Delete(id);
            return RedirectToRoute(new { controller = "ResultDeLab", action = "Index" });
        }
        public async Task<IActionResult> ConfirmResult(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View("ConfirmResult");

        }
    }
}
