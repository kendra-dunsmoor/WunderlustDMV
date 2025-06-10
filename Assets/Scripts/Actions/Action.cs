using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Action/Action Asset")]
public class Action : ScriptableObject
{
    public string actionName;
    public float FRUSTRATION_MODIFIER;
    public float WILL_MODIFIER;
    public float BOSS_WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER;
    public float ATTENTION_MODIFIER;
    public float INCORRECT_CHOICE_ATTENTION_MODIFIER;
    public enum ActionType
    {
        BASIC,
        SPECIAL,
        BOSS,
    };
    public ActionType type;
    public enum ActionMovement
    {
        FRONT,
        BACK,
        AWAY
    };
    public ActionMovement movement;
    public List<ActionEffectStacks> effects;
    public List<ActionUpgrade> actionUpgrades;

    public Sprite baseButtonImage;
    public Sprite hoverButtonImage;
    public string generalDescription;
    public string GetDescription(bool inPerformanceReview = false)
    {
        if (type == ActionType.BOSS) return generalDescription;
        string description = generalDescription;
        if (generalDescription != "" && WILL_MODIFIER != 0) description += "\n";
        if (WILL_MODIFIER < 0 ) description += "Costs " + -WILL_MODIFIER + " Will";
        if (WILL_MODIFIER > 0 ) description += "Replenish " + WILL_MODIFIER + " Will";

        if (FRUSTRATION_MODIFIER > 0 && !inPerformanceReview) description += "\nAdds " + FRUSTRATION_MODIFIER + " Frustration";
        if (FRUSTRATION_MODIFIER < 0 && !inPerformanceReview) description += "\nRemoves " + -FRUSTRATION_MODIFIER + " Frustration";

        if (PERFORMANCE_MODIFIER > 0) description += "\nImproves Performance by " + PERFORMANCE_MODIFIER;
        if (PERFORMANCE_MODIFIER < 0) description += "\nReduces Performance by " + -PERFORMANCE_MODIFIER;

        if (ATTENTION_MODIFIER > 0) description += "\nAdds " + ATTENTION_MODIFIER + "% Attention";
        if (ATTENTION_MODIFIER < 0) description += "\nReduces Attention by " + -ATTENTION_MODIFIER + "%";

        if (BOSS_WILL_MODIFIER > 0 && inPerformanceReview) description += "\nIncreases Boss Will by " + BOSS_WILL_MODIFIER;
        if (BOSS_WILL_MODIFIER < 0 && inPerformanceReview) description += "\nReduces Boss Will by " + -BOSS_WILL_MODIFIER;
        foreach (ActionEffectStacks effectStacks in effects)
        {
            // TODO: Clearer descriptions for particular actions
            if (effectStacks != null)
            {
                if (effectStacks.effect.type == ActionEffect.EffectType.ADD_TURNS) description += "\nSkips a turn.";
                else if (effectStacks.effect.type == ActionEffect.EffectType.MADE_MISTAKE) description += "\nEach Mistake draws more Attention.";
                else description += "\nAdds effect \"" + effectStacks.effect.effectName + "\" for " + effectStacks.stacks + " turns.";
            }
        }
        return description;
    }

    public virtual Action GetCopy()
    {
        return this;
    }
}
