using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public RunTiming timing; // available if you want to scale scoring by current speed later - see note below
    public AudioLoader audioLoader;
    public GameState gameState;

    [Header("Scoring")]
    public float scorePerSecond = 10f;
    public int baseBeatScore = 100;
    public float streakMultiplierStep = 0.1f;

    [Header("Streak")]
    public float streakGraceTime = 2.5f;

    public int Score { get; private set; } = 0;
    public int Streak { get; private set; } = 0;
    public float StreakTimeRemaining { get; private set; } = 0f;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnStreakChanged;

    void Update()
    {
        if (gameState.IsGameOver || !audioLoader.IsPlaying) return;

        Score += Mathf.RoundToInt(scorePerSecond * Time.deltaTime);
        OnScoreChanged?.Invoke(Score);

        if (Streak > 0)
        {
            StreakTimeRemaining -= Time.deltaTime;
            if (StreakTimeRemaining <= 0f) BreakStreak();
        }
    }

    private float lastHitTime = -1f;
    private const float minHitInterval = 0.05f;

    public void RegisterBeatHit()
    {
        if (Time.time - lastHitTime < minHitInterval) return; // swallow an accidental duplicate call
        lastHitTime = Time.time;

        Streak++;
        StreakTimeRemaining = streakGraceTime;

        float multiplier = 1f + (Streak - 1) * streakMultiplierStep;
        Score += Mathf.RoundToInt(baseBeatScore * multiplier);

        OnScoreChanged?.Invoke(Score);
        OnStreakChanged?.Invoke(Streak);
    }

    public void BreakStreak()
    {
        if (Streak == 0) return;
        Streak = 0;
        StreakTimeRemaining = 0f;
        OnStreakChanged?.Invoke(Streak);
    }
}