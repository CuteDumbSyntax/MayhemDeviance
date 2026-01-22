using MayhemDeviance.Common.Systems;
using Terraria.GameContent.ItemDropRules;

public class MayhemDifCondition : IItemDropRuleCondition
{
    public bool CanDrop(DropAttemptInfo info)
        => MayhemDifficulty.MayhemMode;

    public bool CanShowItemDropInUI()
        => true;

    public string GetConditionDescription()
        => "Mayhem Mode";
}