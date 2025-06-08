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

    public string GetDescription()
    {
        string description = generalDescription;
        description += "Will: " + WILL_MODIFIER;
        if (FRUSTRATION_MODIFIER != 0) description += "\nFrustration: " + FRUSTRATION_MODIFIER;
        if (PERFORMANCE_MODIFIER != 0) description += "\nPerformance: " + PERFORMANCE_MODIFIER;
        if (ATTENTION_MODIFIER != 0) description += "\nAttention: " + ATTENTION_MODIFIER + "%";
        if (BOSS_WILL_MODIFIER != 0) description += "\nBoss Will: " + BOSS_WILL_MODIFIER + "%";
        foreach (ActionEffectStacks effectStacks in effects)
        {
            // TODO: Clearer descriptions for particular actions
            /*
             if (effectStacks.effect.type == ADD_TURNS) description += "\nSkips a turn.";
             else if (effectStacks.effect.type == MADE_MISTAKE) description += "\nEach Mistake draws more Attention." ;
             else */
            if (effectStacks != null) description += "\nAdds effect " + effectStacks.effect.type + " for " + effectStacks.stacks + " turns.";
        }
        switch (movement)
        {
            case ActionMovement.FRONT:
                description += "\nCustomer does not move.";
                break;
            case ActionMovement.BACK:
                description += "\nCustomer moves to back of queue.";
                break;
            case ActionMovement.AWAY:
                description += "\nCustomer leaves queue.";
                break;
        }
        // TODO: Add actionUpgrades descriptions
        return description;
    }

    public virtual Action GetCopy()
    {
        return this;
    }
}
