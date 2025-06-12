using UnityEngine;
using TMPro;

public class CertificateUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject priceTag;
    [SerializeField] private GameObject purchasedTag;

    public Certificate certificate;
    private int gridIndex;

    public void AddCertificate(Certificate cert, int index)
    {
        gridIndex = index;
        certificate = cert;
        title.text = cert.title;
        if (price != null) purchasedTag.SetActive(false);
        if (price != null) price.text = cert.price.ToString();
        if (gameObject.GetComponent<MouseOverDescription>() != null)
        {
            gameObject.GetComponent<MouseOverDescription>().UpdateDescription(cert.description);
        }
    }
    public void MarkAsPurchased()
    {
        priceTag.SetActive(false);
        purchasedTag.SetActive(true);
    }

    public void PurchaseScreen(CertificateUIController cert)
    {
        FindFirstObjectByType<CertificationsPageController>().PurchasePopUp(cert.certificate, gridIndex);
    }
}
