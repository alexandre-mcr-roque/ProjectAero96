namespace ProjectAero96.Helpers
{
    [Flags]
    public enum Roles
    {
        // If adding new roles, also add them in wwwroot/js/users-dataTable.js
        None = 0b000, Client = 0b001, Employee = 0b010, Admin = 0b100
    }
}
