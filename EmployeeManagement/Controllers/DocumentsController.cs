using EmployeeManagement.Shared;
using EmployeeManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public DocumentsController(EmployeeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<DocumentDto>> AddDoc(int taskId, DocumentDto documentDto)
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

            var note = new Document
            {
                TaskId = taskId,
                DocumentId = documentDto.DocId,
                FilePath = documentDto.FileName
            };

            _context.Documents.Add(note);
            await _context.SaveChangesAsync();

            var noteToReturn = new DocumentDto
            {
                DocId = taskId,
                TaskItemId=note.TaskId,
                FileName=note.FilePath
            };

            return (noteToReturn);
        }

        [HttpGet("taskitem/{taskItemId}")]
        public async Task<IEnumerable<Document>> GetDocsByTaskItemIdAsync(int taskItemId)
        {
            return await _context.Documents.Where(n => n.TaskId == taskItemId).ToListAsync();
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<Document>> GetDocument(int taskId, int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int taskId, int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);

            if (document == null)
            {
                return NotFound();
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }
       
    }
}

