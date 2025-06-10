using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CertificationsPageController : MonoBehaviour
{
    [SerializeField] private GameObject certPrefab;
    [SerializeField] private Transform certsParent;
    [SerializeField] private GameObject upgradesScreen;
    [SerializeField] private Certificate[] certificatesAvailable;
    private AudioManager audioManager;
    private GameManager gameManager;

    // Purchase screen:
    [SerializeField] private GameObject opaqueScreen;
    [SerializeField] private GameObject purchasePopUp;
    [SerializeField] private TextMeshProUGUI purchaseDescription;

    void Awake()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        // These should probably get moved to prefab script but leaving all pop up logic in here for now:
        opaqueScreen.SetActive(false);
        purchasePopUp.SetActive(false);

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
        if (gameManager.FetchSoulCredits() >= cert.price)
        {
            if (audioManager != null) audioManager.PlaySFX(audioManager.buyUpgrade);
            gameManager.AddCertificate(cert);
            gameManager.UpdateSoulCredits(-cert.price);
            GameObject.FindGameObjectWithTag("Counter_SoulCredits")
                .GetComponent<CurrencyCounter>()
                .RefreshCounter();
            certUI.MarkAsPurchased();
        }
        else
        {
            // Not enough money
            if (audioManager != null) audioManager.PlaySFX(audioManager.noEnergy);
        }
    }

    public void ReturnToUpgrades()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Instantiate(upgradesScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void CloseComputer()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Destroy(gameObject);
    }

    public void PurchasePopUp(Certificate cert)
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        // Add opaque background
        opaqueScreen.SetActive(true);
        // Add purchase popup
        purchasePopUp.SetActive(true);
        purchasePopUp.GetComponentInChildren<CertificateUIController>().AddCertificate(cert);
        purchaseDescription.text = "Buy " + cert.title + " certification for " + cert.price + " Chthonic Credits?";
    }

    public void Cancel() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        purchasePopUp.SetActive(false);
        opaqueScreen.SetActive(false);
    }
}
