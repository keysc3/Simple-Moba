using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelInfo", menuName = "Game/LevelInfo")]
public class LevelInfo : ScriptableObject
{
    public int maxLevel { get; private set; } = 18;
    public List<float> championKillXP { get; private set; } = new List<float>(){42f, 114f, 144f, 174f, 204f, 234f, 308f, 392f, 486f, 
    590f, 640f, 690f, 740f, 790f, 840f, 890f, 940f, 990f};
    public List<float> requiredXP { get; private set; } = new List<float>(){0f, 280f, 380f, 480f, 580f, 680f, 780f, 880f, 980f, 1080f, 
    1180f, 1280f, 1380f, 1480f, 1580f, 1680f, 1780f, 1880f};
    public float xShift { get; private set; } = 0f;
    public float yShift { get; private set; } = 15f;
    public int maxSpellLevel { get; private set; } = 5;
    public int maxUltLevel { get; private set; } = 3;
}
