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

    // TODO: action upgrades;
    // TODO: RUP;
}
