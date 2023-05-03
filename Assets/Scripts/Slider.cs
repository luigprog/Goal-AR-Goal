using System;

/// <summary>
/// Math slider.
/// </summary>
class Slider
{
    public float minValue = 0.0f;
    public float maxValue = 0.0f;

    public Slider(float minValue, float maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public float GetPoint(float value)
    {
        if (value >= maxValue)
            return 1.0f;
        if (value <= minValue)
            return 0.0f;

        return (value - minValue) / (maxValue - minValue);
    }

    public float GetValue(float point)
    {
        if (point < 0.0f)
            point = 0.0f;
        if (point > 1.0f)
            point = 1.0f;
        return minValue + (point * (maxValue - minValue));
    }

}

