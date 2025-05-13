using UnityEngine;

[CreateAssetMenu(fileName = "ActionEffect", menuName = "Action/ActionEffect")]
public class ActionEffect : ScriptableObject
{
    public enum EffectType {
        ATTENTION,
        CONFUSION,
        HUSTLING,
        ADD_TURNS
    };
    public EffectType type;
    public Sprite effectSprite;
}
