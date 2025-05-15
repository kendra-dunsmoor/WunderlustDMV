using UnityEngine;
using UnityEngine.UI;

public class UIEffectController : MonoBehaviour
{
    [SerializeField] Image displayedSprite;
    public ActionEffect effect;
    private int turns;

    public void AddEffect(ActionEffect effect, int turns) {
        this.effect = effect;
        this.turns = turns;
        displayedSprite.sprite = effect.effectSprite;
    }
    public void UpdateTurns(int diff) {
        turns += diff;
    }

    public int FetchTurns() {
        return turns;
    }
}
