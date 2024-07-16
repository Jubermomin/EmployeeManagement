using EmployeeManagement.Shared;
using EmployeeManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public ReportsController(EmployeeContext context)
        {
            _context = context;
        }

        [HttpGet("weekly")]
        public async Task<ActionResult> GetWeeklyReport()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7);
            var tasks = await _context.EmployeeTasks.Where(t => t.DueDate >= oneWeekAgo && t.DueDate <= DateTime.Now).ToListAsync();

            var report = tasks.GroupBy(t => t.AssignedToUser.TeamId)
                              .Select(g => new
                              {
                                  TeamId = g.Key,
                                  TasksDueThisWeek = g.Count()
                              }).ToList();

            return Ok(report);
        }

        [HttpGet("monthly")]
        public async Task<ActionResult> GetMonthlyReport()
        {
            var oneMonthAgo = DateTime.Now.AddMonths(-1);
            var tasks = await _context.EmployeeTasks.Where(t => t.DueDate >= oneMonthAgo && t.DueDate <= DateTime.Now).ToListAsync();

            var report = tasks.GroupBy(t => t.AssignedToUser.TeamId)
                              .Select(g => new
                              {
                                  TeamId = g.Key,
                                  TasksDueThisMonth = g.Count()
                              }).ToList();

            return Ok(report);
        }

    }
}
