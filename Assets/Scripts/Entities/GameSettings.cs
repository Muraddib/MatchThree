using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GameSettings
{
    [Header("Gems")]
    public GemData[] Gems;

    [Header("Gem Grid")]
    public int GridWidth;
    public int GridHeight;

    [Header("Game Settings")]
    public int StartSteps;
    public float StartTime;
    public int BonusSteps;
    public int BonusSeconds;
}
