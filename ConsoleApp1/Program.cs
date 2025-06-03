using ProjectAero96.Helpers;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Roles roles = Roles.Admin | Roles.Client;
            Console.WriteLine((byte)roles);
        }
    }
}
