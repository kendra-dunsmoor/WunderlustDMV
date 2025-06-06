using UnityEngine;
using UnityEngine.UI;

public class UIEffectController : MonoBehaviour
{
    [SerializeField] Image displayedSprite;
    public ActionEffect effect;
    private int turns;

    public void AddEffect(ActionEffect effect, int turns)
    {
        this.effect = effect;
        this.turns = turns;
        if (effect.effectSprite != null) displayedSprite.sprite = effect.effectSprite;
        gameObject.GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "\nTurns: " + turns, effect.effectName);
    }
    public void UpdateTurns(int diff)
    {
        turns += diff;
        gameObject.GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "\nTurns: " + turns, effect.effectName);
        Debug.Log("Updated effect turns to: " + turns);
    }

    public int FetchTurns() {
        return turns;
    }
}
