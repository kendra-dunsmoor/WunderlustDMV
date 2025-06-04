using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ActionEffect", menuName = "Action/ActionEffect")]
public class ActionEffect : ScriptableObject
{
    public enum EffectType {
        ATTENTION,
        HUSTLING,
        ADD_TURNS,
        DRAINED,
        CAFFIENATED,
        IRATE,
        CONFUSED,
        CALMED,
        INCOHERENT,
        SHORTFUSE,
        MELLOW,
        ELATED,
        CHEERFUL
    };
    public EffectType type;
    public enum TargetType {
        PLAYER,
        ENEMY
    };
    public TargetType target; 
    public Sprite effectSprite;
    [TextArea(3,10)]
    public string effectDescription;
    public bool shouldStack;
    public bool shouldDecay;

    // Generalize effects:
    public bool isPercent; // Determine if multiplier or addition
    public float FRUSTRATION_MODIFIER;
    public float WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER; // TODO: will depend on correct answer?
}
