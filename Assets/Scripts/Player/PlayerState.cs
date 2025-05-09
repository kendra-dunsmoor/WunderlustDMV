using UnityEngine;

/*
* Player State
* ~~~~~~~~~~~~~
* Information for player status/inventory/abilities
*/
public class PlayerState : MonoBehaviour
{
    /* Office Bucks:
    Only valid within DMV. Purchase Consumables and Supplies
	Rewarded after each shift and review. 
    Rewarded by Events.
	Spent at Shop.
    tems and Unspent are lost when Fired or Reincarnated.
    */
    private int officeBucks;
    /* Soul Credits:
    Real Money. Rent / Certifications / Personal Life
	Rewarded by Reviews and Rare Events.
	Spent in Apartment
	Small amount is lost when Fired or Reincarnated.
    */    private int soulCredits;

    private Artifact[] artifacts;

    // TODO: consumables
    // TODO: certifications
    // TODO: specializations

    /* TODO: Social Currency with Coworkers
	Rewarded by Events, Modifiers.
	Spent in NPC interactions
    All unspent is lost when Fired or Reincarnated.
    */

}
