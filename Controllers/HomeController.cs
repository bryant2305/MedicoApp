using MedicoApp.Core.Application.Interfaces.Services;
using MedicoApp.Core.Application.ViewModels.Medico;
using MedicoApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.MedicoApp.Middelwares;

namespace MedicoApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly IMedicoService _medicoService;
        private readonly IPacientesService _pacientesService;
        private readonly IPruebaDeLabService _pruebaDeLabService;
        private readonly ICitasService _citasService;
        private readonly IResultDeLabService _resultDeLabService;
        private readonly ValidateUserSession _validateUserSession;


        public HomeController(IMedicoService medicoService, IPacientesService pacientesService, IPruebaDeLabService preubaDeLabService, ICitasService citasService, IResultDeLabService resultDeLabService, ValidateUserSession validateUserSession)
        {
            _medicoService = medicoService;
            _pacientesService = pacientesService;
            _citasService = citasService;
            _pruebaDeLabService = preubaDeLabService;
            _resultDeLabService = resultDeLabService;
            _validateUserSession = validateUserSession;
        }
        public async Task<IActionResult> Index()
        {

            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View();
        }
    }
}


