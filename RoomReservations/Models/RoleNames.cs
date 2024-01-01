namespace RoomReservations.Models;

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Receptionist = "Receptionist";
    public const string Employee = "Employee";
    public const string Client = "Client";

    public static readonly List<string> Roles =
    [
        Admin,
        Receptionist,
        Employee,
        Client
    ];
}