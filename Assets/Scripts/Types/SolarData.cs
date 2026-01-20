using System;

class SolarData
{
    public TimeSpan timeOfDay { get; private set; }
    public float azimuth { get; private set; }
    public float altitude { get; private set; }
    public IPeriod period { get; set; }

    public SolarData(TimeSpan span, float az, float alt, Period p)
    {
        timeOfDay = span;
        azimuth = az;
        altitude = alt;
        period = p;
    }
}
