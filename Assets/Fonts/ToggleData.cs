using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleData : MonoBehaviour
{
    [SerializeField] public Image[] backgroundImages;
    [SerializeField] public string[] difficultyText;
    [SerializeField] public string[] mineProbabilityText;
    [SerializeField] public string[] scoreMultiplierText;
    [SerializeField] public int[] mineProbability;

    [SerializeField] public TMP_Text difficultyTextUI;
    [SerializeField] public TMP_Text mineProbabilityTextUI;
    [SerializeField] public TMP_Text scoreMultiplierTextUI;
    [SerializeField] public int currentMineProbability;



    public void SetMineProbability(int difficulty)
    {
        currentMineProbability = mineProbability[difficulty];
    }

    public void SetDifficultyText(int difficulty)
    {
        difficultyTextUI.text = difficultyText[difficulty];
    }

    public void SetMineProbabilityText(int mineProbability)
    {
        mineProbabilityTextUI.text = mineProbabilityText[mineProbability];
    }

    public void SetScoreMultiplierText(int scoreMultiplier)
    {
        scoreMultiplierTextUI.text = scoreMultiplierText[scoreMultiplier];
    }

}
