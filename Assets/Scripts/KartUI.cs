using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartUI : MonoBehaviour
{
    public Text PlaceText;
    public Text TimeText;
    public Text SpeedText;

    public BetterButton GoBtn;
    public BetterButton FireBtn;
    public Slider WheelSlider;

    private void Awake()
    {
        Util.SetActive(GoBtn, false);
        Util.SetActive(FireBtn, false);
        Util.SetActive(WheelSlider, false);
    }

    void Start()
    {
        SpeedText.text = "0 mph";
        PlaceText.text = "1st";
        TimeText.text = "00:00";
    }

    public void EnableMobileControls()
    {
        Util.SetActive(GoBtn, true);
        Util.SetActive(FireBtn, true);
        Util.SetActive(WheelSlider, true);
    }

    public void SetSpeed(int speed)
    {
        SpeedText.text = speed.ToString() + " MPH";
    }
}