using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Paperwork : MonoBehaviour
{
    [SerializeField] private Image image;
    public bool isAcceptable;

    [Header("------------- Temp two options -------------")]
    [SerializeField] private Sprite rejectImage;
    [SerializeField] private Sprite acceptImage;
    private System.Random randomGenerator;

    void Start()
    {
        randomGenerator = new System.Random(); 
        gameObject.SetActive(false);
    }

    public void CreatePaperwork(float enemyOdds)
    {
        Debug.Log("Enemy odds of correct paperwork = " + enemyOdds);
        if (RandomBool(enemyOdds))
        {
            Debug.Log("Add correct paperwork");
            if (acceptImage != null) image.sprite = acceptImage;
            isAcceptable = true;
        }
        else
        {
            Debug.Log("Add reject paperwork");
            if (rejectImage != null) image.sprite = acceptImage;
            isAcceptable = false;
        }
        gameObject.SetActive(true);
    }

    private bool RandomBool(double probability)
    {
        // Ensure probability is within the valid range [0, 1]
        if (probability < 0.0 || probability > 1.0) probability = 0.5;

        // Return true if the random value is less than or equal to the desired probability
        return randomGenerator.NextDouble() <= probability; 
    }
}
