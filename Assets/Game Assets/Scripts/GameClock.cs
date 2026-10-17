using UnityEngine;

public static class GameClock
{
    public static float SecondsPerStage = 60f;   // set to 6 to test quickly
    public const int Stages = 4;
    public const float StartHour = 4f;           // 4:00 AM = start of Dawn

    public static float SecondsPerDay => SecondsPerStage * Stages;
    public static float Elapsed => Time.realtimeSinceStartup;

    public static int Day => Mathf.FloorToInt(Elapsed / SecondsPerDay) + 1;
    public static float Hour24 => (StartHour + Elapsed / SecondsPerDay * 24f) % 24f;
}