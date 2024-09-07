public class TestAbility : AbstractAbility
{
    public override string AbilityName { get; set; } = "Test";
    public override string Description { get; set; } = "test description";
    public override float Cooldown { get; set; } = 3f;
    public override float Damage { get; set; } = 1f;
    public override string ProjectilePrefabPath { get; set; } = "Abilities/TestAbility";
}
