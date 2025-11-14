using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        public SubjectController(ISubjectService subjectService) => _subjectService = subjectService;

        public async Task<IActionResult> Index() => View(await _subjectService.GetAllSubjectsAsync());

        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _subjectService.AddSubjectAsync(subject);
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.SuId) return BadRequest();
            if (ModelState.IsValid)
            {
                await _subjectService.UpdateSubjectAsync(subject);
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _subjectService.DeleteSubjectAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
