using Microsoft.AspNetCore.Mvc;
using MediatR;
using QueryX;
using SimpleAuth.Server.Helpers;
using SimpleAuth.Application.Users.Commands;
using SimpleAuth.Application.Users.Queries;
using SimpleAuth.Server.Attributes;
using static SimpleAuth.Application.AuthConstants;

namespace SimpleAuth.Server.Controllers;

[AuthorizeRoles(Roles.AuthAdmin)]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly QueryBuilder _queryBuilder;

    public UsersController(IMediator mediator, QueryBuilder queryBuilder)
    {
        _mediator = mediator;
        _queryBuilder = queryBuilder;
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var getByIdQuery = new GetUserDtoById(id);
        var result = await _mediator.Send(getByIdQuery, cancellationToken);
        if (result.Data == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] QueryModel queryModel, CancellationToken cancellationToken)
    {
        var query = _queryBuilder.CreateQuery<ListUserDto, UserDto>(queryModel);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUser command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        response.Verify();
        var model = await _mediator.Send(new GetUserDtoById(response.Value), cancellationToken);
        return CreatedAtRoute("GetUserById", new { id = response.Value }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUser command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);
        response.Verify();
        return Accepted();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUser(id);
        var response = await _mediator.Send(command, cancellationToken);
        response.Verify();
        return Accepted();
    }

    [HttpPost("{id}/lockout")]
    public async Task<IActionResult> SetUserLockout(Guid id, [FromBody] SetUserLockout command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);
        response.Verify();
        return Accepted();
    }
}
