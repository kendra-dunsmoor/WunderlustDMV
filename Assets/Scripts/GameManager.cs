using UnityEngine;


/*
* Game Manager
* ~~~~~~~~~~~~~
* Repsonsible for saving cross scene player status/progression logic
*/
public class GameManager : MonoBehaviour
{
    // ~~~~~~ Game State ~~~~~~~
    public static GameManager instance;
    private GameState gameStatus;
    private PlayerState playerStatus;

    // ~~~~~~ Functions ~~~~~~
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // TODO: logic to fetch for saved game vs new game
        gameStatus = new GameState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
