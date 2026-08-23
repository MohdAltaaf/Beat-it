using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EndGameHandler : MonoBehaviour
{
    public GameState gameState;
    public cameraFollow cameraFollowScript;
    public BikeController bikeController;
    public Rigidbody bikeRigidbody;
    public GameObject winPanel;
    public GameObject losePanel;
    

    public ScoreManager scoreManager;
    public TMP_Text winScoreText;
    public TMP_Text loseScoreText;

    public float winCameraHoldTime = 3f;
    public float loseCameraHoldTime = 2.5f;
    public float crashSpinForce = 5f;
    public float crashPopForce = 3f;

    void OnEnable()
    {
        gameState.OnWin += HandleWin;
        gameState.OnGameOver += HandleLose;
    }

    void OnDisable()
    {
        gameState.OnWin -= HandleWin;
        gameState.OnGameOver -= HandleLose;
    }

    void HandleWin() => StartCoroutine(WinSequence());
    void HandleLose() => StartCoroutine(LoseSequence());

    IEnumerator WinSequence()
    {
        cameraFollowScript.followEnabled = false;
        yield return new WaitForSeconds(winCameraHoldTime);
        if (winScoreText != null) winScoreText.text = $"Final Score: {scoreManager.Score:N0}";
        winPanel.SetActive(true);
    }

    IEnumerator LoseSequence()
    {
        bikeController.enabled = false;
        bikeRigidbody.isKinematic = false;
        bikeRigidbody.AddTorque(Random.insideUnitSphere * crashSpinForce, ForceMode.Impulse);
        bikeRigidbody.AddForce((Vector3.up + Random.insideUnitSphere * 0.3f) * crashPopForce, ForceMode.Impulse);

        yield return new WaitForSeconds(loseCameraHoldTime);
        if (loseScoreText != null) loseScoreText.text = $"Final Score: {scoreManager.Score:N0}";
        losePanel.SetActive(true);
    }

    void Update()
    {
        if (!winPanel.activeSelf && !losePanel.activeSelf) return;
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            SceneManager.LoadScene("MainMenu");
    }
}