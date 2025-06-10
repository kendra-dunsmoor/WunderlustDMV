using UnityEngine;
using UnityEngine.SceneManagement;

public class EndReviewController : MonoBehaviour
{
    public void ReturnHome()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
