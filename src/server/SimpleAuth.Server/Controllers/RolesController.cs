using Microsoft.AspNetCore.Mvc;
using MediatR;
using SimpleAuth.Application.Queries.Roles;
using SimpleAuth.Application.Commands.Roles;
using QueryX;
using SimpleAuth.Server.Helpers;

namespace SimpleAuth.Server.Controllers;

[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly QueryBuilder _queryBuilder;

    public RolesController(IMediator mediator, QueryBuilder queryBuilder)
    {
        _mediator = mediator;
        _queryBuilder = queryBuilder;
    }

    [HttpGet("{id}", Name = "GetRoleById")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var getByIdQuery = new GetRoleDtoById(id);
        var result = await _mediator.Send(getByIdQuery, cancellationToken);
        if (result.Data == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] QueryModel queryModel, CancellationToken cancellationToken)
    {
        var query = _queryBuilder.CreateQuery<ListRoleDto, RoleDto>(queryModel);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRole command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        response.Verify();
        var model = await _mediator.Send(new GetRoleDtoById(response.Value), cancellationToken);
        return CreatedAtRoute("GetRoleById", new { id = response.Value }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRole command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);
        response.Verify();
        return Accepted();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteRole(id);
        var response = await _mediator.Send(command, cancellationToken);
        response.Verify();
        return Accepted();
    }
}
