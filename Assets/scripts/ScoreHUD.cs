using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreHUD : MonoBehaviour
{
    public ScoreManager scoreManager;
    public TMP_Text scoreText;
    public TMP_Text streakText;
    public Image streakTimerBar; // Image Type: Filled, Fill Method: Horizontal

    void OnEnable()
    {
        scoreManager.OnScoreChanged += UpdateScore;
        scoreManager.OnStreakChanged += UpdateStreak;
    }

    void OnDisable()
    {
        scoreManager.OnScoreChanged -= UpdateScore;
        scoreManager.OnStreakChanged -= UpdateStreak;
    }

    void UpdateScore(int score) => scoreText.text = score.ToString("N0");

    void UpdateStreak(int streak)
    {
        streakText.gameObject.SetActive(streak > 0);
        streakText.text = $"x{streak}";
        if (streak > 0) StartCoroutine(PunchScale(streakText.transform));
    }

    void Update()
    {
        if (streakTimerBar == null) return;
        float t = scoreManager.Streak > 0 ? scoreManager.StreakTimeRemaining / scoreManager.streakGraceTime : 0f;
        streakTimerBar.fillAmount = Mathf.Clamp01(t);
    }

    IEnumerator PunchScale(Transform target, float amount = 1.3f, float duration = 0.15f)
    {
        Vector3 baseScale = Vector3.one;
        target.localScale = baseScale * amount;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(baseScale * amount, baseScale, t / duration);
            yield return null;
        }
        target.localScale = baseScale;
    }
}