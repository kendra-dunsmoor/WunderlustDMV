using UnityEngine;

public class ComputerController : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] private GameObject furnitureScreen;
    [SerializeField] private GameObject certificationsScreen;
    [SerializeField] private GameObject classesScreen;
    void Start() {
        audioManager = 	FindFirstObjectByType<AudioManager>();
    }
    public void OpenFurniture()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Instantiate(furnitureScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void OpenCertifications()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Instantiate(certificationsScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void OpenClasses()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Instantiate(classesScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    public void CloseComputer()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Destroy(gameObject);
    }
}
