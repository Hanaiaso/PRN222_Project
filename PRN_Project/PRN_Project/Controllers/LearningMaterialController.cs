using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Controllers
{
    public class LearningMaterialController : Controller
    {
        private readonly ILearningMaterialService _materialService;
        private readonly ISubjectService _subjectService;
        public LearningMaterialController(ILearningMaterialService materialService, ISubjectService subjectService)
        {
            _materialService = materialService;
            _subjectService = subjectService;
        } 
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Index(int subjectId)
        {
            ViewBag.SubjectId = subjectId;
            var materials = await _materialService.GetMaterialsBySubjectIdAsync(subjectId);
            return View(materials);
        }
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create(int subjectId)
        {
            ViewBag.SubjectId = subjectId;
            return View(new LearningMaterial { SubjectID = subjectId });
        }
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(LearningMaterial material, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {

                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".zip" };
                    var extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("File", "File upload chưa đúng định dạng.");
                        return View(material);
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    material.FilePath = "/uploads/" + uniqueFileName;
                }
                material.UploadDate = DateTime.Now;
                await _materialService.AddMaterialAsync(material);
                return RedirectToAction("Index", new { subjectId = material.SubjectID });
            }
            return View(material);
        }
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null) return NotFound();
            return View(material);
        }
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, LearningMaterial material, IFormFile? file)
        {
            if (id != material.MaterialID) return BadRequest();
            if (ModelState.IsValid)
            {
                // Lấy file cũ trước khi update
                var existing = await _materialService.GetMaterialByIdAsync(id);
                if (existing == null) return NotFound();
                existing.Title = material.Title;
                existing.Description = material.Description;
                // Nếu người dùng upload file mới
                if (file != null && file.Length > 0)
                {
                    // Xóa file cũ
                    if (!string.IsNullOrEmpty(existing.FilePath))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    // Lưu file mới
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    existing.FilePath = "/uploads/" + uniqueFileName;
                }

                await _materialService.UpdateMaterialAsync(existing);
                return RedirectToAction("Index", new { subjectId = material.SubjectID });
            }
            return View(material);
        }
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null)
            {
                return NotFound();
            };
            return View(material);
        }
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null) 
            {
                return NotFound();
            };
            // Xóa file trên server
            if (!string.IsNullOrEmpty(material.FilePath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", material.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            var subjectId = material.SubjectID;
            await _materialService.DeleteMaterialAsync(id);
            return RedirectToAction("Index", new { subjectId });
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentIndex(int? subjectId)
        {
            // Lấy danh sách môn học để hiển thị dropdown
            var subjects = await _subjectService.GetAllSubjectsAsync();
            ViewBag.Subjects = subjects;

            List<LearningMaterial> materials = new List<LearningMaterial>();

            // Nếu có chọn môn học
            if (subjectId.HasValue)
            {
                materials = await _materialService.GetMaterialsBySubjectIdAsync(subjectId.Value);
            }

            ViewBag.SelectedSubjectId = subjectId;
            return View(materials);
        }
    }
}
