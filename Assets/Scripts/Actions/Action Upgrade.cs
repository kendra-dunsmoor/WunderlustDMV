using UnityEngine;

[CreateAssetMenu(fileName = "New Action Upgrade", menuName = "Action/Action Upgrade")]
public class ActionUpgrade : ScriptableObject
{
    public string upgradeName;
    public float FRUSTRATION_MODIFIER;
    public float WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER;
    public Action.ActionMovement movement;
    public ActionEffect effect;
    public int turnsOfEffect;
}
