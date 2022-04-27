using System;
using System.Collections;
using Scripts;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

/// <summary>
/// Controlling the game states
/// Switching between menu, play, endgame scenes
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject menuScene;
    public GameObject playScene;
    public GameObject gameoverScene;
    public GameObject levelCompletedScene;
    public GameObject nextLevelButton;
    public GameObject[] bricksLevelPrefabs;
    public GameObject deathPrefab;
    public GameObject pedalPrefab;
    public GameObject stagePrefab;
    private GameObject currentLevelBricks;

    public static GameManager Instance { get; private set; }

    /// <summary>
    /// List of available states
    /// </summary>
    public enum State
    {
        MENU,
        PLAY,
        LEVELCOMPLETED,
        GAMEOVER
    }

    private State state;
    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //Begin with menu scene
        SwitchState(State.MENU);
    }

    /// <summary>
    /// This is method is for switching game state
    /// </summary>
    public void SwitchState(State newState)
    {
        EndState();
        state = newState;
        BeginState(newState);
    }

    /// <summary>
    /// Back to Menu Scene
    /// </summary>
    public void PlayAgain()
    {
        SwitchState(State.MENU);
    }

    /// <summary>
    /// Start a level by the given level input
    /// 0 is a test level
    /// </summary>
    public void StartLevel(int level)
    {
        SwitchState(State.PLAY);
        currentLevel = level;
        currentLevelBricks = Instantiate(bricksLevelPrefabs[currentLevel], playScene.transform);
    }

    /// <summary>
    /// Go to the next level
    /// </summary>
    public void NextLevel()
    {
        if (currentLevel < bricksLevelPrefabs.Length)
        {
            currentLevel++;
            StartLevel(currentLevel);    
        }
    }

    /// <summary>
    /// This method will be called when we switch to a new state
    /// </summary>
    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MENU:
                //cleaning up assets from the previous play
                foreach (Transform child in playScene.transform)
                {
                    Destroy(child.gameObject);
                }
                menuScene.SetActive(true);
                break;
            case State.PLAY:
                Instantiate(deathPrefab, playScene.transform);
                Instantiate(pedalPrefab, playScene.transform);
                Instantiate(stagePrefab, playScene.transform);
                playScene.SetActive(true);
                break;
            case State.LEVELCOMPLETED:
                levelCompletedScene.SetActive(true);
                //If we have already finished the last level, the next level button will not be displayed
                if (currentLevel >= bricksLevelPrefabs.Length)
                {
                    nextLevelButton.SetActive(false);
                }
                break;
            case State.GAMEOVER:
                gameoverScene.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.MENU:
                break;
            case State.PLAY:
                //Checking the winning condition
                if (currentLevelBricks.transform.childCount == 0)
                {
                    SwitchState(State.LEVELCOMPLETED);
                }
                break;
            case State.LEVELCOMPLETED:
                break;
            case State.GAMEOVER:
                break;
        }
    }

    /// <summary>
    /// This method will be called when a state is ended
    /// </summary>
    void EndState()
    {
        switch (state)
        {
            case State.MENU:
                menuScene.SetActive(false);
                break;
            case State.PLAY:
                //Cleaning up prefabs
                foreach (Transform child in playScene.transform)
                {
                    Destroy(child.gameObject);
                }
                
                playScene.SetActive(false);
                break;
            case State.LEVELCOMPLETED:
                nextLevelButton.SetActive(true);
                levelCompletedScene.SetActive(false);
                break;
            case State.GAMEOVER:
                gameoverScene.SetActive(false);
                break;
        }
    }
}