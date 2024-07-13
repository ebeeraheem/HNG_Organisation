using HNG_Organisation.Models;
using HNG_Organisation.Results;
using HNG_Organisation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNG_Organisation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrganisationsController : ControllerBase
{
    private readonly OrganisationService _organisationService;

    public OrganisationsController(OrganisationService organisationService)
    {
        _organisationService = organisationService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrganisationSuccessResponse>> GetOrganisation(string id)
    {
        var organisation = await _organisationService.GetOrganisation(id);
        if (organisation is null)
        {
            return NotFound();
        }

        var response = new OrganisationSuccessResponse()
        {
            Status = "success",
            Message = "organisation retrieved successfully",
            Data = new OrganisationSuccessData()
            {
                orgId = organisation.Id,
                name = organisation.Name,
                description = organisation.Description
            }
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganisation([FromBody] OrganisationModel model)
    {
        if (!ModelState.IsValid)
        {
            var failureResponse = new UnsuccessfulResponse()
            {
                Status = "Bad request",
                Message = "Client error",
                StatusCode = 400
            };

            return BadRequest(failureResponse);
        }

        try
        {
            var organisation = await _organisationService
                .CreateOrganisationAsync(model);

            var response = new OrganisationSuccessResponse()
            {
                Status = "success",
                Message = "Organisation created successfully",
                Data = new OrganisationSuccessData()
                {
                    orgId = organisation.Id,
                    name = organisation.Name,
                    description = organisation.Description
                }
            };
            return Created("", response);
        }
        catch (Exception)
        {
            var failureResponse = new UnsuccessfulResponse()
            {
                Status = "Bad request",
                Message = "Client error",
                StatusCode = 400
            };

            return BadRequest(failureResponse);
        }
    }

    [HttpPost("{orgId}/users")]
    public async Task<AddUserResponse> AddUserToOrganisation(string orgId, [FromBody] AddUserToOrganisationModel addUserModel)
    {
        var response = await _organisationService.AddUserToOrganisation(orgId, addUserModel);

        return response;
    }
}
