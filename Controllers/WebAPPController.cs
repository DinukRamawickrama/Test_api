using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPP.Model;


namespace WebAPP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // Create Task
        [HttpPost]
        public async Task<IActionResult> CreateTask(Model.Task task)
        {
            task.CreatedOn = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskById), new { id = task.ID }, task);
        }

        // Update Task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, Model.Task task)
        {
            if (id != task.ID)
            {
                return BadRequest();
            }

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.Name = task.Name;
            existingTask.Description = task.Description;
            existingTask.EstimatedDurationInHours = task.EstimatedDurationInHours;
            existingTask.StatusId = task.StatusId;
            existingTask.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Complete Task
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.StatusId = 2; // Assuming 2 represents 'Completed'
            task.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete Task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("authenticate")]
        public IActionResult Authitcate() 
            {
            var Claims = new[]
            {
                new Claim("FullName","Figri Ismail"),
                new Claim(JwtRegisteredClaimNames.Sub, "task.id")

            };
            var keyBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.Aes128CbcHmacSha256 );

            var tokenString = new JwtSecurityToken(Constants.Issuer, Constants.Audience,Claims,notBefore: DateTime.Now,expires: DateTime.Now.AddHours(1),signingCredentials);

            return Ok(new { accessToken = tokenString });
        }


        // Get Task by Id (for CreateTask method)
        [HttpGet("{id}")]
        [Authorize]

        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }
    }
}