namespace ProjectAero96.Data.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
        bool Deleted { get; set; }
    }
}
