using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakRoomManager : MonoBehaviour
{
    void Start()
    {
    }
    public void StartShift()
    {
        SceneManager.LoadSceneAsync(3);
    }

}
