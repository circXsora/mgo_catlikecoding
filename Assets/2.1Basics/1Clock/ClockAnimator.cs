using System;
using UnityEngine;

public class ClockAnimator : MonoBehaviour
{
    private const float hoursToDegrees = 360f / 12f, minutesToDegrees = 360f / 60f, secondsToDegrees = 360 / 60f;
    public Transform hours, minutes, seconds;
    public bool analog;


    private void UpdateContinuous() {
        TimeSpan timespan = DateTime.Now.TimeOfDay;
        hours.localRotation = Quaternion.Euler(0f, 0f, (float)timespan.TotalHours * -hoursToDegrees);
        minutes.localRotation = Quaternion.Euler(0f, 0f, (float)timespan.TotalMinutes * -minutesToDegrees);
        seconds.localRotation = Quaternion.Euler(0f, 0f, (float)timespan.TotalSeconds * -secondsToDegrees);
    }

    private void UpdateDiscrete() {
        DateTime time = DateTime.Now;
        hours.localRotation = Quaternion.Euler(0f, 0f, time.Hour * -hoursToDegrees);
        minutes.localRotation = Quaternion.Euler(0f, 0f, time.Minute * -minutesToDegrees);
        seconds.localRotation = Quaternion.Euler(0f, 0f, time.Second * -secondsToDegrees);
    }
    private void Update() {
        if (analog) {
            UpdateContinuous();
        }
        else {
            UpdateDiscrete();
        }
    }

}
