using EmployeeManagement.Shared;
using EmployeeManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public NotesController(EmployeeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            var note = await _context.Notes.ToListAsync();

            if (note == null)
            {
                return NotFound();
            }

            return note;
        }
        [HttpPost]
        public async Task<ActionResult<NoteDto>> AddNoteDto(int taskId, NoteDto noteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.EmployeeTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            var note = new Note
            {
                TaskId = taskId,
                Content = noteDto.Content,
                CreatedByUserId = noteDto.CreatedByUserId,
                CreatedDate = noteDto.CreatedDate
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            var noteToReturn = new NoteDto
            {
                NoteId = note.NoteId,
                Content = note.Content,
                CreatedByUserId = note.CreatedByUserId,
                CreatedDate = note.CreatedDate
            };

            return CreatedAtAction(nameof(GetNote), new { taskId = taskId, noteId = note.NoteId }, noteToReturn);
        }


        [HttpGet("{noteId}")]
        public async Task<ActionResult<Note>> GetNote(int taskId, int noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);

            if (note == null)
            {
                return NotFound();
            }

            return note;
        }

        [HttpGet("taskitem/{taskItemId}")]
        public async Task<IEnumerable<Note>> GetNotesByTaskItemIdAsync(int taskItemId)
        {
            return await _context.Notes.Where(n => n.TaskId == taskItemId).ToListAsync();
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int taskId, int noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);

            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

