namespace Common.Extensions;

public static class NumberExtension
{
    public const double ScaleFactor = 0.00001;
    public static double ConverToGraduce(this int coord)
    {
        return coord * ScaleFactor;
    }
}