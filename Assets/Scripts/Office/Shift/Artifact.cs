using UnityEngine;

public class Artifact: ScriptableObject
{
    public string artifactName;
    public Sprite sprite;
    public int index;
    public int price;
    public int turnClock;
    public int currentTurnCounter;
    public enum Rarity {
        COMMON,
        UNCOMMON,
        RARE,
        STARTER,
        QUEST
    }
    public Rarity rarity;
    public enum ArtifactEffectType {
        PLAYER_CONDITION,
        CURR_CUSTOMER_CONDITION
    } // gotta iterate on this
    public ArtifactEffectType[] effects;
    public string description;
    // is there a way to track what their effects are besides manually hard-coding each? They all effect different meters
}
