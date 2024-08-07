﻿namespace HNG_Organisation.Entities;

public class Organisation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<User> Users { get; set; } = [];
}
