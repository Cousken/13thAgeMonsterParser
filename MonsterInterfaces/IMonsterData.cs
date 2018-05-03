using System.Collections.Generic;

namespace MonsterInterfaces
{
    public interface IMonsterSize : IMonsterWord
    {
    }

    public interface IMonsterNamePart : IMonsterWord
    {
    }

    public interface IMonsterRole : IMonsterWord
    {
    }

    public interface IMonsterType : IMonsterWord
    {
    }
    
    public interface IMonsterData
    {
        IList<IMonsterNamePart> Name { get; }
        IMonsterSize Size { get; set; }
        int Level { get; set; }
        IMonsterRole Role { get; set; }
        IMonsterType Type { get; set; }
        int Initiative { get; set; }
        int ArmorClass { get; set; }
        int PhysicalDefense { get; set; }
        int MentalDefense { get; set; }
        int HealthPoints { get; set; }
    }
}