using UnityEngine;

[System.Serializable]
public struct ColorRangeHSV
{
    [FloatRangeSlider(0f, 1f)]
    public FloatRange hue, saturation, value;

    public Color RandomInRange
    {
        get
        {
            return Random.ColorHSV(
                hue.Min, hue.Max,
                saturation.Min, saturation.Max,
                value.Min, value.Max,
                1f, 1f
            );
        }
    }
}