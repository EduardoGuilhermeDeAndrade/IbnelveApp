using IbnelveApp.Application.DTOs;
using IbnelveApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IbnelveApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipamentosController : ControllerBase
{
    private readonly IEquipamentoService _service;

    public EquipamentosController(IEquipamentoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _service.ObterTodosAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _service.ObterPorIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EquipamentoDto dto)
    {
        await _service.AdicionarAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] EquipamentoDto dto)
    {
        if (id != dto.Id) return BadRequest();
        await _service.AtualizarAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.RemoverAsync(id);
        return NoContent();
    }
}
