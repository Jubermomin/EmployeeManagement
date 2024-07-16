using EmployeeManagement.Shared;
using EmployeeManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public TasksController(EmployeeContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetTasks()
        {
            {
                var tasks = await _context.EmployeeTasks
             .Include(t => t.AssignedToUser)
             .Include(t => t.CreatedByUser)
             .ToListAsync();
                Console.WriteLine(tasks);
                // Map entities to DTOs
                var taskDtos = tasks.Select(task => new EmployeeDto
                {
                    Id = task.TaskId,
                    Name = task.AssignedToUser.Name,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Completed = task.IsCompleted,
                    //Name = task.Name,
                    // Map other properties as needed, excluding navigation properties causing the issue
                });

                return Ok(taskDtos);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetTask(int id)
        {
            var task = await _context.EmployeeTasks.Include(t => t.AssignedToUser).Include(t => t.CreatedByUser).FirstOrDefaultAsync(t => t.TaskId == id);

            var taskDtos = new EmployeeDto {
                Id = task.TaskId,
                Name = task.AssignedToUser.Name,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Completed = task.IsCompleted,
            };

            if (task == null)
            {
                return NotFound();
            }

            return taskDtos;
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> AddNoteDto(EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var note = new EmployeeTask
            {
                Title = employeeDto.Title,
                Description = employeeDto.Description,
                AssignedToUserId = 1,
                CreatedByUserId = 2,
                DueDate = DateTime.Now.AddDays(2),
                IsCompleted = false
            };

            _context.EmployeeTasks.Add(note);
            await _context.SaveChangesAsync();

            return Ok();
        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTask(int id, EmployeeTask task)
        //{
        //    if (id != task.TaskId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(task).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TaskExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, EmployeeDto taskDto)
        {
            if (id != taskDto.Id)
            {
                return BadRequest();
            }

            var task = await _context.EmployeeTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Map the fields from the DTO to the entity
            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.AssignedToUserId = 1;
            task.CreatedByUserId = 1;
            task.DueDate = taskDto.DueDate;
            task.IsCompleted = taskDto.Completed;

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.EmployeeTasks.Any(e => e.TaskId == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.EmployeeTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            _context.EmployeeTasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
       
    }
}
