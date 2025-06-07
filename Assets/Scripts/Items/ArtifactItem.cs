using UnityEngine;

[CreateAssetMenu]
public class ArtifactItem : Item
{
    public int turnClock;
    // counter for how many turns since last trigger 
    public int currentTurnCounter = 0;
    public bool isEndOfShiftEffect = false;
}
