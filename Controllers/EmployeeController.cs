using Employee_Api.Applicationdbcontext;
using Employee_Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Api.Controllers
{
    [Route("api/employee")]
    [ApiController]
   [Authorize(Roles ="Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetEmployee()
        {
            return Ok(_context.employees.ToList());
        }
        [HttpPost]
        public IActionResult SaveEmployee([FromBody] Employee employee)
        {
            if (employee == null) return NotFound();
            if (!ModelState.IsValid)
                return BadRequest();
            _context.employees.Add(employee);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateEmployee([FromBody] Employee employee)
        {
            if (employee == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            _context.employees.Update(employee);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteEmployee(int id)
        {
            var data =_context.employees.Find(id);
            if (data == null) return NotFound();
            _context.employees.Remove(data);
            _context.SaveChanges();
            return Ok();
        }
    }
}

    

