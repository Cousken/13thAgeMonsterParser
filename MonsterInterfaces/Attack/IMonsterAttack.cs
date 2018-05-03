namespace MonsterInterfaces.Attack
{
    public interface IMonsterAttack
    {
        int AttackBonus { get; set; }
        int NumberOfTargetsMin { get; set; }
        int NumberOfTargetsMax { get; set; }
        IAttackRange Range { get; set; }
        IAttackDefense Defense { get; set; }
        IAttackType Type { get; set; }
        
    }

    public interface IAttackType : IMonsterWord
    {
    }

    public interface IAttackDefense : IMonsterWord
    {
    }

    public interface IAttackRange : IMonsterWord
    {
    }
}