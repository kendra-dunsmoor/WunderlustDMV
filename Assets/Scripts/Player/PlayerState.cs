using System.Collections.Generic;
using UnityEngine;

/*
* Player State
* ~~~~~~~~~~~~~
* Information for player status/inventory/abilities
*/
public class PlayerState
{
    /* Office Bucks:
    Only valid within DMV. Purchase Consumables and Supplies
	Rewarded after each shift and review. 
    Rewarded by Events.
	Spent at Shop.
    tems and Unspent are lost when Fired or Reincarnated.
    */
    private int officeBucks = 0;
    /* Soul Credits:
    Real Money. Rent / Certifications / Personal Life
	Rewarded by Reviews and Rare Events.
	Spent in Apartment
	Small amount is lost when Fired or Reincarnated.
    */
    private int soulCredits;

    private Artifact[] artifacts;

    // TODO: consumables
    // TODO: certifications
    // TODO: specializations

    /* TODO: Social Currency with Coworkers
	Rewarded by Events, Modifiers.
	Spent in NPC interactions
    All unspent is lost when Fired or Reincarnated.
    */
    private Action[] actionLoadout;

    private List<Certificate> activeCertificates = new List<Certificate>();

    public void UpdateOfficeBucks(int amount) {
        officeBucks += amount;
    }

    public int GetOfficeBucks() {
        return officeBucks;
    }

    public void UpdateSoulCredits(int amount) {
        soulCredits += amount;
    }

    public int GetSoulCredits() {
        return soulCredits;
    }

    public void AddCertificate(Certificate cert) {
        activeCertificates.Add(cert);
    }

    public List<Certificate> GetCertificates() {
            return activeCertificates;
    }
}
