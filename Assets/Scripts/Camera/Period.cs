using System;

interface IPeriod
{
    public TimeSpan Duration { get; set; }
}

class Period : IPeriod
{
    public TimeSpan Duration { get; set; }
}
