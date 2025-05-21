using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CertificationsPageController : MonoBehaviour
{
    [SerializeField] private GameObject certPrefab;
    [SerializeField] private Transform certsParent;
    [SerializeField] private GameObject upgradesScreen;
    [SerializeField] private Certificate[] certificatesAvailable;
    private GameManager gameManager;

    // Purchase screen:
    [SerializeField] private GameObject opaqueScreen;
    [SerializeField] private GameObject purchasePopUp;
    [SerializeField] private TextMeshProUGUI purchaseDescription;

    void Start()
    {
        // These should probably get moved to prefab script but leaving all pop up logic in here for now:
        opaqueScreen.SetActive(false);
        purchasePopUp.SetActive(false);

        // Get Game Manager to check and add certs
        gameManager = FindFirstObjectByType<GameManager>();
        // check if player has cert and mark as purchased
        List<Certificate> playerCerts = gameManager.FetchCertificates();
        // Instantiate all available certs for sale. For now just going to serialize a field for this
        foreach (Certificate cert in certificatesAvailable) {
            GameObject certUI = Instantiate(certPrefab, certsParent);
            certUI.GetComponent<CertificateUIController>().AddCertificate(cert);
            if (playerCerts != null && playerCerts.Contains(cert)) certUI.GetComponent<CertificateUIController>().MarkAsPurchased();
        }
    }

    public void BuyCert(CertificateUIController certUI)
    {
        Certificate cert = certUI.certificate;
        if (gameManager.FetchSoulCredits() >= cert.price) {
            gameManager.AddCertificate(cert);
            gameManager.UpdateSoulCredits(cert.price);
            certUI.MarkAsPurchased();
        }
    }

    public void ReturnToUpgrades()
    {
        Instantiate(upgradesScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void CloseComputer()
    {
        Destroy(gameObject);
    }

    public void PurchasePopUp(Certificate cert)
    {
        // Add opaque background
        opaqueScreen.SetActive(true);
        // Add purchase popup
        purchasePopUp.SetActive(true);
        Debug.Log("here");
        purchasePopUp.GetComponentInChildren<CertificateUIController>().AddCertificate(cert);
        Debug.Log("Do we get here?");
        purchaseDescription.text = "Buy " + cert.title + " certification for " + cert.price + " soul credits?";
    }

    public void Cancel() {
        purchasePopUp.SetActive(false);
        opaqueScreen.SetActive(false);
    }
}
