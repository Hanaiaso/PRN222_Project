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
        public async Task<IActionResult> Create(LearningMaterial material)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Edit(int id, LearningMaterial material)
        {
            if (id != material.MaterialID) return BadRequest();
            if (ModelState.IsValid)
            {
                await _materialService.UpdateMaterialAsync(material);
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
