using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    [Range(0f, 1f)] public float hitMeter = 0f;
    public float hitAmount = 0.35f;
    public float decayRate = 0.08f;         // per second, passive recovery
    public float beatBonusRecovery = 0.1f;  // extra recovery on catching a beat block
    public Vignette vignette;

    public event Action OnGameOver;
    private bool gameOver = false;

    void Update()
    {
        if (gameOver) return;
        hitMeter = Mathf.Max(0f, hitMeter - decayRate * Time.deltaTime);
        vignette?.SetIntensity(hitMeter);
    }

    public void RegisterCarHit()
    {
        if (gameOver) return;
        hitMeter = Mathf.Min(1f, hitMeter + hitAmount);
        vignette?.SetIntensity(hitMeter);
        if (hitMeter >= 1f) TriggerGameOver();
    }

    public void RegisterBeatCatch()
    {
        if (gameOver) return;
        hitMeter = Mathf.Max(0f, hitMeter - beatBonusRecovery);
        vignette?.SetIntensity(hitMeter);
    }

    void TriggerGameOver()
    {
        gameOver = true;
        Debug.Log("GAME OVER - hook UI here once it exists");
        OnGameOver?.Invoke();
    }
    public event Action OnWin;
    public bool IsGameOver => gameOver;

    public void TriggerWin()
    {
        if (gameOver) return; // can't win after already losing
        gameOver = true;
        Debug.Log("YOU WIN - hook UI here");
        OnWin?.Invoke();
    }
}