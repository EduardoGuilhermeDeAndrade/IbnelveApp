using IbnelveApp.Application.DTOs.Equipamento;
using IbnelveApp.Application.Interfaces;
using IbnelveApp.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnelveApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // <-- Exige que qualquer usuário esteja autenticado
public class EquipamentosController : ControllerBase
{
    private readonly IEquipamentoService _service;

    public EquipamentosController(IEquipamentoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dados = await _service.ObterTodosAsync();
        return Ok(ApiResponse<IEnumerable<EquipamentoDto>>.Ok(dados));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var equipamento = await _service.ObterPorIdAsync(id);
        if (equipamento == null)
            return NotFound(ApiResponse<EquipamentoDto>.Falha("Equipamento não encontrado."));

        return Ok(ApiResponse<EquipamentoDto>.Ok(equipamento));
    }

    [HttpGet("numerocontrole")]
    public async Task<IActionResult> GetByNumeroControle(string numeroControle)
    {
        var equipamento = await _service.ObterPorNumeroControleAsync(numeroControle);
        if (equipamento == null)
            return NotFound(ApiResponse<EquipamentoDto>.Falha("Equipamento não encontrado."));

        return Ok(ApiResponse<EquipamentoDto>.Ok(equipamento));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EquipamentoDto dto)
    {
        var exists = await _service.ObterPorNumeroControleAsync(dto.NumeroControle);
        if (exists != null)
            return Conflict(ApiResponse<object>.Falha("Equipamento com o mesmo número de controle já existe."));

        await _service.AdicionarAsync(dto);
        return CreatedAtAction(nameof(GetById), 
            new { 
                id = dto.Id }, 
            ApiResponse<EquipamentoDto>.Ok(dto, "Equipamento cadastrado com sucesso."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] EquipamentoDto dto)
    {
        if (id != dto.Id)
            return BadRequest(ApiResponse<object>.Falha("O ID da URL não corresponde ao corpo da requisição."));

        await _service.AtualizarAsync(dto);
        return Ok(ApiResponse<EquipamentoDto>.Ok(dto, "Equipamento atualizado com sucesso."));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "GerenteOuSuperior")] // Apenas Gerentes ou Admins podem deletar
    public async Task<IActionResult> DeleteLogical(Guid id)
    {
        await _service.RemoverAsync(id, true);
        return Ok(ApiResponse<object>.Ok(null, "Equipamento removido logicamente."));
    }
}
