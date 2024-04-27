﻿namespace AgileKnowledge.Service.Options;

public class ConnectionStringsOptions
{
    public const string Name = "ConnectionStrings";

    public static string DefaultConnection { get; set; }

    public static string TableNamePrefix { get; set; }
}