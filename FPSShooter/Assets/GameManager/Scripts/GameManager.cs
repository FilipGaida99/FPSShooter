using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public int Wave { get; private set; }
    public float ElapsedTime { 
        get 
        {
            if(state == State.Active)
            {
                return Time.time - startTime;
            }
            else
            {
                return endTime - startTime;
            }
        }
    }

    public State state;
    public float crateDropInterval;
    public MenuScenesManager.Scene endGameScene = MenuScenesManager.Scene.ScoreMenu;
    public Camera mainCamera;

    private float startTime;
    private float endTime;

    private EnemySpawner enemySpawner;
    private CrateSpawner crateSpawner;
    private Coroutine crateSpawnCorotine;
    private int waveEnemiesCount = 0;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainCamera = Camera.main;
    }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public void Reset()
    {
        Wave = 0;
        startTime = 0f;
        endTime = 0f;
    }

    public void StartGame()
    {
        if (state == State.Passive)
        {
            Wave = 0;
            startTime = Time.time;
            endTime = 0f;
            state = State.Active;
            enemySpawner = FindObjectOfType<EnemySpawner>();
            crateSpawner = FindObjectOfType<CrateSpawner>();
            crateSpawnCorotine = StartCoroutine(CrateSpawnRoutine());
            StartNewWave();
        }
    }

    public void EndGame()
    {
        if (state == State.Active)
        {
            endTime = Time.time;
            state = State.Passive;
            if (crateSpawnCorotine != null) 
            {
                StopCoroutine(crateSpawnCorotine);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            MenuScenesManager.Instance.Load(endGameScene);
        }
    }

    public void EnemyKilled()
    {
        if (state == State.Active)
        {
            waveEnemiesCount--;
            InGameUIController.Instance.enemyCountText.SetTextValue(waveEnemiesCount);
            if (waveEnemiesCount <= 0)
            {
                StartNewWave();
            }
        }
    }

    private void StartNewWave()
    {
        if (state == State.Active)
        {
            Wave++;
            waveEnemiesCount = enemySpawner.SpawnEnemies(Wave);
            InGameUIController.Instance.enemyCountText.SetTextValue(waveEnemiesCount);
            InGameUIController.Instance.SetWave(Wave);
        }
    }

    private IEnumerator CrateSpawnRoutine()
    {
        while (true)
        {
            if(state != State.Active)
            {
                yield break;
            }
            if (crateSpawner != null)
            {
                crateSpawner.SpawnCrates();
                yield return new WaitForSeconds(crateDropInterval);
            }
        }
    }

    public enum State { Passive, Active}
}
