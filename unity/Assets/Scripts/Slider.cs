using System;

/// <summary>
/// Math slider.
/// </summary>
class Slider
{
    public float minValue = 0f;
    public float maxValue = 0f;

    public Slider(float minValue, float maxValue) 
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public float GetPoint(float value) 
    {
        if (value >= maxValue)
            return 1;
        if (value <= minValue)
            return 0;

        return (value - minValue) / (maxValue - minValue);
    }

    public float GetValue(float point) 
    {
        if (point < 0)
            point = 0;
        if (point > 1)
            point = 1;
        return minValue + (point * (maxValue - minValue));
    }

}

