using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Action/Action Asset")]
public class Action: ScriptableObject
{
    public string actionName;
    public float FRUSTRATION_MODIFIER;
    public float WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER; // TODO: will depend on correct answer?
    public enum ActionType {
        BASIC,
        SPECIAL
    };
    public ActionType type;
    public enum ActionMovement {
        FRONT,
        BACK,
        AWAY
    };
    public ActionMovement movement;
    public ActionEffect effect;
    public int turnsOfEffect;

    public Sprite baseButtonImage;
    public Sprite hoverButtonImage;
    public string generalDescription;

    public string GetDescription() {
        string description = generalDescription;
        if (WILL_MODIFIER != 0) description += "Will cost: " + WILL_MODIFIER;
        if (FRUSTRATION_MODIFIER != 0) description += " Frustration modifier: " + FRUSTRATION_MODIFIER;
        if (PERFORMANCE_MODIFIER != 0) description += " Performance modifier: " + PERFORMANCE_MODIFIER;
        if (effect != null) description += " Adds effect " + effect.type + " for " + turnsOfEffect + " turns.";
        switch (movement) {
            case ActionMovement.FRONT:
                description += " Customer does not move.";
                break;
            case ActionMovement.BACK:
                description += " Customer moves to back of queue.";
                break;
            case ActionMovement.AWAY:
                description += " Customer leaves queue.";
                break;
        }
        return description;
    }


    // TODO: action upgrades;
}
