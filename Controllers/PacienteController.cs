using MedicoApp.Core.Application.Interfaces.Services;
using MedicoApp.Core.Application.Services;
using MedicoApp.Core.Application.ViewModels.Medico;
using MedicoApp.Core.Application.ViewModels.Paciente;
using Microsoft.AspNetCore.Mvc;
using WebApp.MedicoApp.Middelwares;

namespace WebApp.MedicoApp.Controllers
{
    public class PacienteController : Controller
    {

        private readonly IPacientesService _pacienteService;
        private readonly ValidateUserSession _validateUserSession;

        public PacienteController(IPacientesService pacienteService, ValidateUserSession validateUserSession)
        {
            _pacienteService = pacienteService;
            _validateUserSession = validateUserSession;
        }
        public async Task<IActionResult> Index()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View(await _pacienteService.GetAllViewModel());
            ViewBag.Pacientes = await _pacienteService.GetAllViewModel();
        }

        public IActionResult Create()
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View("SavePaciente", new SavePacienteViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePacienteViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View("SavePaciente", vm);
            }
            SavePacienteViewModel pacienteVm = await _pacienteService.Add(vm);

            if (pacienteVm.Id != 0 && pacienteVm != null)
            {
                pacienteVm.ImageUrl = UploadFile(vm.File, pacienteVm.Id);

                await _pacienteService.Update(pacienteVm);

            }

            return RedirectToRoute(new { controller = "Paciente", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View("SavePaciente", await _pacienteService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SavePacienteViewModel vm)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            if (!ModelState.IsValid)
            {
                return View("SavePaciente", vm);
            }

            SavePacienteViewModel pacienteVm = await _pacienteService.GetByIdSaveViewModel(vm.Id);
            vm.ImageUrl = UploadFile(vm.File, pacienteVm.Id, true, pacienteVm.ImageUrl);
            await _pacienteService.Update(vm);
            return RedirectToRoute(new { controller = "Paciente", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            return View(await _pacienteService.GetByIdSaveViewModel(id));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (!_validateUserSession.HasUser())
            {
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            await _pacienteService.Delete(id);
            //get directory path
            string basePath = $"/Images/Pacientes/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new(path);

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo folder in directory.GetDirectories())
                {
                    folder.Delete(true);
                }

                Directory.Delete(path);
            }

            return RedirectToRoute(new { controller = "Paciente", action = "Index" });
        }

        private string UploadFile(IFormFile file, int id, bool isEditMode = false, string imagePath = "")
        {
            if (isEditMode)
            {
                if (file == null)
                {
                    return imagePath;
                }
            }
            string basePath = $"/Images/Pacientes/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            //create folder if not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //get file extension
            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new(file.FileName);
            string fileName = guid + fileInfo.Extension;

            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            if (isEditMode)
            {
                string[] oldImagePart = imagePath.Split("/");
                string oldImagePath = oldImagePart[^1];
                string completeImageOldPath = Path.Combine(path, oldImagePath);

                if (System.IO.File.Exists(completeImageOldPath))
                {
                    System.IO.File.Delete(completeImageOldPath);
                }
            }
            return $"{basePath}/{fileName}";
        }
    }
}



