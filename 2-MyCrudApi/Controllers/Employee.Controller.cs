using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCrudApi.Data;
using MyCrudApi.Models;

namespace MyCrudApi.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _context.Employees.ToListAsync());
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        return emp == null ? NotFound() : Ok(emp);
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(Employee emp)
    {
        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();
        return Ok(emp);
    }

    // UPDATE
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Employee emp)
    {
        var data = await _context.Employees.FindAsync(id);
        if (data == null) return NotFound();

        data.Name = emp.Name;
        data.Email = emp.Email;

        await _context.SaveChangesAsync();
        return Ok(data);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();

        _context.Employees.Remove(emp);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
