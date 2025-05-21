using UnityEngine;

[CreateAssetMenu(fileName = "Certificate", menuName = "Scriptable Objects/Certificate")]
public class Certificate : ScriptableObject
{
    public enum CertificateType {
        SUPPLY_SOURCING,
        SIDE_GIG,
        IT_CERT,
        DATA_ENTRY,
        LEADERSHIP_TRAINING,
        FINANCIAL_LITERACY
    };
    public CertificateType type;
    [TextArea(3,10)]
    public string description;
    public string title;
    public int price;
}
