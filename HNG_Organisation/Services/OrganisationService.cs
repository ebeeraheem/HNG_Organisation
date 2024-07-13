﻿using HNG_Organisation.Data;
using HNG_Organisation.Entities;
using HNG_Organisation.Models;
using HNG_Organisation.Results;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace HNG_Organisation.Services;

public class OrganisationService
{
    private readonly ApplicationDbContext _context;

    public OrganisationService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create organisation
    public async Task<Organisation> CreateOrganisationAsync(OrganisationModel model)
    {
        var organisation = new Organisation()
        {
            Name = model.Name,
            Description = model.Description
        };

        await _context.Organisations.AddAsync(organisation);
        await _context.SaveChangesAsync();

        return organisation;
    }

    // Get organisation
    public async Task<Organisation?> GetOrganisation(int id)
    {
        var organisation = await _context.Organisations.FindAsync(id);
        return organisation;
    }

    // Get a user's organisations

    // Add user to org
}
