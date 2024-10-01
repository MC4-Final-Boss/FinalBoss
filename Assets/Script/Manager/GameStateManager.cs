using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private GameState currentState;

    private void Start()
    {
        // Start the game in the Play state
        SetGameState(GameState.Play);
    }

    public void SetGameState(GameState newState)
    {
        // Exit the current state
        ExitCurrentState();

        currentState = newState;

        // Enter the new state
        EnterNewState();
    }

    private void ExitCurrentState()
    {
        switch (currentState)
        {
            case GameState.Finish:
                Debug.Log("Exiting Finish State");
                break;

            case GameState.Die:
                Debug.Log("Exiting Die State");
                break;

            case GameState.Logout:
                Debug.Log("Exiting Logout State");
                break;

            case GameState.Pause:
                Debug.Log("Exiting Pause State");
                Time.timeScale = 1f; // Resume game time
                break;

            case GameState.Play:
                Debug.Log("Exiting Play State");
                break;
        }
    }

    private void EnterNewState()
    {
        switch (currentState)
        {
            case GameState.Finish:
                Debug.Log("Entering Finish State");
                break;

            case GameState.Die:
                Debug.Log("Entering Die State");
                HandlePlayerRespawn();
                break;

            case GameState.Logout:
                Debug.Log("Entering Logout State");
                break;

            case GameState.Pause:
                Debug.Log("Entering Pause State");
                Time.timeScale = 0f; // Pause game time
                break;

            case GameState.Play:
                Debug.Log("Entering Play State");
                break;
        }
    }

    // Handle player respawn logic when entering Die state
    private void HandlePlayerRespawn()
    {
        PlayerRespawn[] players = FindObjectsOfType<PlayerRespawn>();
        foreach (var player in players)
        {
            player.RespawnPlayer(); // Respawn each player
        }
    }

    // Example methods to change states
    public void OnPlayerDie()
    {
        SetGameState(GameState.Die);
    }

    public void OnPlayerFinish()
    {
        SetGameState(GameState.Finish);
    }

    public void OnLogout()
    {
        SetGameState(GameState.Logout);
    }

    public void OnPause()
    {
        SetGameState(GameState.Pause);
    }

    public void OnResume()
    {
        SetGameState(GameState.Play);
    }
}
