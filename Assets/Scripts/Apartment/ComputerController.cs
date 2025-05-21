using UnityEngine;

public class ComputerController : MonoBehaviour
{
    [SerializeField] private GameObject furnitureScreen;
    [SerializeField] private GameObject certificationsScreen;

    public void OpenFurniture()
    {
        Instantiate(furnitureScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void OpenCertifications()
    {
        Instantiate(certificationsScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void CloseComputer()
    {
        Destroy(gameObject);
    }
}
