using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetWorldTime : MonoBehaviour
{
    public TMP_Text worldTimeText;

    private void Update()
    {
        worldTimeText.text = System.DateTime.Now.ToString("h:mm tt");
    }

    public void SetTime(int hours, int minutes)
    {
        System.DateTime newTime = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, hours, minutes, 0);
        worldTimeText.text = newTime.ToString("h:mm tt");
    }
}
