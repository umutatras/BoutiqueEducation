namespace BoutiqueEducation.Entity.Interfaces;

public interface ICreatableEntity
{
    DateTime CreatedDate { get; set; }
    Guid? CreatedById { get; set; }
}
