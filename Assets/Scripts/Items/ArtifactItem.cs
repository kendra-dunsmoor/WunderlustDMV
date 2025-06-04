using UnityEngine;

[CreateAssetMenu]
public class ArtifactItem : Item
{
    public int turnClock;
    // counter for how many turns since last trigger 
    public int currentTurnCounter;
    public bool isEndOfShiftEffect = false;
}
