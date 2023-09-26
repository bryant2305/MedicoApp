using MedicoApp.Core.Application.Helpers;
using MedicoApp.Core.Application.Interfaces.Services;
using MedicoApp.Core.Application.Services;
using MedicoApp.Core.Application.ViewModels.Paciente;
using MedicoApp.Core.Application.ViewModels.User;
using MedicoApp.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MedicoApp.Core.Application.ViewModels.User;
using WebApp.MedicoApp.Middelwares;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace WebApp.MedicoApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ValidateUserSession _validateUserSession;
        private readonly IMedicoService _medicoService;
        private readonly IPacientesService _pacientesService;
        private readonly IPruebaDeLabService _pruebaDeLabService;
        private readonly ICitasService _citasService;


        public UserController(IUserService userService, ValidateUserSession validateUserSession)
        {
            _userService = userService;
            _validateUserSession = validateUserSession;
        }
        public IActionResult Index()
        {
            if (_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            return View();
        }
        public async Task<IActionResult> MantUser()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            ViewBag.User = await _userService.GetAllViewModel();
            return View(await _userService.GetAllViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginVm)
        {
            if (_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View(loginVm);
            }

            UserViewModel userVm = await _userService.Login(loginVm);
            if (userVm != null)
            {
                HttpContext.Session.Set<UserViewModel>("user", userVm);

                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            else
            {
                ModelState.AddModelError("UserValidation", "Datos Incorrecto");
            }
            return View(loginVm);
        }
        public IActionResult Register()
        {
            if (_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            return View(new SaveUserViewModel());
        }
        public IActionResult LogOut()
        {

            HttpContext.Session.Remove("user");
            return RedirectToRoute(new { controller = "User", action = "Index" });
        }
        [HttpPost]
        public async Task<IActionResult> Register(SaveUserViewModel userVm)
        {


            if (_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View(userVm);
            }
            bool UserVmExist = await _userService.IfUserExiste(userVm);
            if (UserVmExist)
            {
                ModelState.AddModelError("UserValidation", "El usuario ya existe");
                return View("Register", userVm);
            }

            await _userService.Add(userVm);


            return RedirectToRoute(new { controller = "User", action = "Index" });
        }

        public IActionResult Create()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View("Create", new SaveUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveUserViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View("Create", vm);
            }

            await _userService.Add(vm);
            return RedirectToRoute(new { controller = "User", action = "MantUser" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View("SaveUser", await _userService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveUserViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View("SaveUser", vm);
            }

            await _userService.Update(vm);
            return RedirectToRoute(new { controller = "User", action = "MantUser" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View(await _userService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            await _userService.Delete(id);
            return RedirectToRoute(new { controller = "User", action = "MantUser" });
        }

    }
}
