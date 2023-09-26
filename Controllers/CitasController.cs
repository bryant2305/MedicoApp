using MedicoApp.Core.Application.Interfaces.Services;
using MedicoApp.Core.Application.Services;
using MedicoApp.Core.Application.ViewModels.Citas;
using Microsoft.AspNetCore.Mvc;
using WebApp.MedicoApp.Middelwares;

namespace WebApp.MedicoApp.Controllers
{
    public class CitasController : Controller
    {

        private readonly IMedicoService _medicoService;
        private readonly IPacientesService _pacientesService;
        private readonly IPruebaDeLabService _pruebaDeLabService;
        private readonly ICitasService _citasService;
        private readonly ValidateUserSession _validateUserSession;

        public CitasController(IMedicoService medicoService, IPacientesService pacientesService, IPruebaDeLabService pruebaDeLabService, ICitasService citasService, ValidateUserSession validateUserSession)
        {

            _medicoService = medicoService;
            _pacientesService = pacientesService;
            _citasService = citasService;
            _pruebaDeLabService = pruebaDeLabService;
            _validateUserSession = validateUserSession;
        }
        public async Task<IActionResult> Index()
        {
            {
                if (!_validateUserSession.HasUser())
                {
                    return RedirectToRoute(new { controller = "User", action = "Index" });
                }
                ViewBag.Pacientes = await _pacientesService.GetAllViewModel();
                ViewBag.Medicos = await _medicoService.GetAllViewModel();
                return View(await _citasService.GetAllViewModel());
            }
        }

        public async Task<IActionResult> Create()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            SaveCitasViewModel vm = new();
            vm.MedicosList = await _medicoService.GetAllViewModel();
            vm.PacinetesList = await _pacientesService.GetAllViewModel();
            return View("SaveCita", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveCitasViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                vm.MedicosList = await _medicoService.GetAllViewModel();
                vm.PacinetesList = await _pacientesService.GetAllViewModel();
                return View("SaveCita", vm);
            }

            await _citasService.Add(vm);
            return RedirectToRoute(new { controller = "Citas", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            SaveCitasViewModel vm = await _citasService.GetByIdSaveViewModel(id);
            vm.MedicosList = await _medicoService.GetAllViewModel();
            vm.PacinetesList = await _pacientesService.GetAllViewModel();
            return View("SaveCita", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveCitasViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                vm.MedicosList = await _medicoService.GetAllViewModel();
                vm.PacinetesList = await _pacientesService.GetAllViewModel();
                return View("SaveCita", vm);
            }

            await _citasService.Update(vm);
            return RedirectToRoute(new { controller = "Citas", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View(await _citasService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            await _citasService.Delete(id);
            return RedirectToRoute(new { controller = "Citas", action = "Index" });
        }
    }
}

