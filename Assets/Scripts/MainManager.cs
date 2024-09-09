using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Rigidbody Ball;

    [Header("Gun")]
    [SerializeField] private float reloadTime;
    [SerializeField] private Slider reloadSlider;
    [SerializeField] private float gunRotationSpeed;
    [SerializeField] private Transform gunHolder;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private Rigidbody bullet;
    [SerializeField] private AudioClip shootSound;
    private Gun gun;
    private bool isReloading;

    [Header("Brick")]
    [SerializeField] private Brick BrickPrefab;
    [SerializeField] private GameObject brickHolder;
    [SerializeField] private BricksConfig bricksConfig;
    [SerializeField] private int holderWidth = 4;
    [SerializeField] private int holderHeight = 3;
    [SerializeField] private float brickPartition = 0.1f;
    [SerializeField] private bool isRowAmountRandom;
    [SerializeField] private int rowAmount = 6;
    [SerializeField] private bool isColumnAmountRandom;
    [SerializeField] private int columnAmount = 8;
    [SerializeField] private AudioClip breakSound;
    private List<Brick> bricks = new();

    [Header("Paddle")]
    [SerializeField] private GameObject paddlePrefab;
    [SerializeField] private float paddleSpeed;
    [SerializeField] private float paddleMaxMovement;
    private Paddle paddle;

    [Header("UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private GameObject highScoreObject;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompletedScreen;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private AudioSource soundMaker;

    [Header("Other")]
    [SerializeField] private DeathZone deathZone;

    private bool isOnPlatform = true;

    private bool isPaused = false;
    private bool IsPaused
    {
        get { return isPaused; }
        set 
        { 
            isPaused = value;
            Time.timeScale = isPaused ? 0f : gameSpeed;
            pauseUI.SetActive(isPaused);
        }
    }

    private int points;
    public int Points
    {
        get { return points; }
        set 
        { 
            points = value;
            scoreText.text = "Score: " + points;
        }
    }

    [SerializeField] private int lives;
    public int Lives
    {
        get { return lives; }
        set 
        { 
            lives = value;
            livesText.text = "Lives: " + lives;
            if (lives == 0) GameOver(false);
        }
    }
    private float gameSpeed;
    private bool isGameOver = false;

    void Start()
    {
        if (DataManager.Instance != null)
            if (DataManager.Instance.IsGunMode)
            {
                gun = new(gunRotationSpeed, gunHolder, gunPoint, bullet);
                gunHolder.gameObject.SetActive(true);
            }
                
        paddle = new(paddlePrefab.transform, paddleSpeed, paddleMaxMovement);

        Points = DataManager.Instance == null ? 0 : DataManager.Instance.Score;

        gameSpeed = DataManager.Instance == null ? 1 : DataManager.Instance.SettingsData.gameSpeed;
        Time.timeScale = gameSpeed;

        deathZone.onEntered.AddListener(FallenBall);

        GenerateBricks();
    }

    private void Update()
    {
        gun?.Update();
        paddle.Update();
    }

    private void GenerateBricks()
    {
        int currentRowAmount = isRowAmountRandom ? Random.Range(6, 9) : rowAmount;
        int currentColumnAmount = isColumnAmountRandom ? Random.Range(6, 13) : columnAmount;

        Dictionary<int, int> bricksAmounts = GetBricksAmounts(currentRowAmount * currentColumnAmount, bricksConfig.bricks.Count);

        float widthStep = (float)holderWidth / currentColumnAmount, heightStep = BrickPrefab.transform.localScale.y + brickPartition;
        float brickWidth = widthStep - brickPartition, brickHeight = BrickPrefab.transform.localScale.y;
        float rowStart = (-holderWidth / 2f) + widthStep / 2f, columnStart = brickHolder.transform.position.y;

        List<int> keyBag = new(); bricksAmounts.Keys.ToList().ForEach(x => keyBag.Add(x));
        for (int i = 0; i < currentRowAmount;  i++)
        {
            for (int j = 0; j < currentColumnAmount; j++)
            {
                int selectedBrickIndex = keyBag[BrickPercetangeSelect(bricksAmounts, keyBag)];
                bricksAmounts[selectedBrickIndex]--;
                if (bricksAmounts[selectedBrickIndex] == 0) keyBag.Remove(selectedBrickIndex);

                Vector3 point = new(rowStart + widthStep*j, columnStart + heightStep*i);
                Brick newBrick = Instantiate(BrickPrefab, point, Quaternion.identity);
                newBrick.transform.SetParent(brickHolder.transform);
                newBrick.transform.localScale = new(brickWidth, brickHeight, newBrick.transform.localScale.z);
                newBrick.BrickValues = bricksConfig.bricks[selectedBrickIndex];
                newBrick.OnDestroyed.AddListener(DestroyedBrick);
                bricks.Add(newBrick);
            }
        }
    }

    void ThrowBall()
    {
        Ball.isKinematic = false;

        float randomDirection = Random.Range(-1.0f, 1.0f);
        Vector3 forceDir = new(randomDirection, 1, 0);
        forceDir.Normalize();

        Ball.transform.SetParent(null);
        Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
    }

    void DestroyedBrick(int point, Brick brick)
    {
        bricks.Remove(brick);
        Points += point;
        soundMaker.PlayOneShot(breakSound);
        if (bricks.Count == 0) GameOver(true);
    }
    void DestroyBricks()
    {
        List<Brick> deletingBricks = new(); deletingBricks.AddRange(bricks);
        deletingBricks.RemoveAt(deletingBricks.Count - 1); bricks.Clear();
        for(int i = 0; i < deletingBricks.Count; i++)
        {
            Destroy(deletingBricks[i].gameObject);
        }
    }

    private void FallenBall()
    {
        ReturnBall();
        Lives--;
    }
    private void ReturnBall()
    {
        paddle.SetBallOnPaddle(Ball.transform);
        Ball.isKinematic = true;
        isOnPlatform = true;
    }

    private void UpdateScore()
    {
        if (DataManager.Instance != null)
        {
            if (Points > DataManager.Instance.BestScore)
            {
                highScoreObject.SetActive(true);
                DataManager.Instance.BestScore = Points;
                DataManager.Instance.SaveData();
            }
        }
    }
    public void GameOver(bool isCompleted)
    {
        isGameOver = true;
        UpdateScore();
        ReturnBall();
        if (isCompleted)
        {
            levelCompletedScreen.SetActive(true);
        }
        else gameOverScreen.SetActive(true);

        if (DataManager.Instance != null) DataManager.Instance.Score = isCompleted ? Points : 0;
    }

    public void Continue() => IsPaused = false;

    public void ExitToMenu()
    {
        if (DataManager.Instance != null) DataManager.Instance.Score = 0;
        SceneManager.LoadSceneAsync("menu");
    }

    private void OnGUI()
    {
        if(Event.current.type == EventType.KeyDown)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.Escape:
                    if (!isGameOver) IsPaused = !IsPaused;
                    else ExitToMenu();
                    break;
                case KeyCode.Space:
                    if (isOnPlatform)
                    {
                        isOnPlatform = false;
                        ThrowBall();
                    }
                    if (isGameOver) SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                    break;
#if UNITY_EDITOR
                case KeyCode.R:
                    DestroyBricks();
                    break;
#endif
            }
        }
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (gun != null && !isPaused && !isGameOver && !isOnPlatform && !isReloading)
            {
                gun.Shoot();
                soundMaker.PlayOneShot(shootSound);
                StartCoroutine(Reload());
            }
        }
            
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        reloadSlider.gameObject.SetActive(true);

        reloadSlider.value = 0;

        while(reloadSlider.value < 1f)
        {
            yield return new WaitForEndOfFrame();
            reloadSlider.value += Time.deltaTime/reloadTime;
        }

        isReloading = false;
        reloadSlider.gameObject.SetActive(false);
    }

    Dictionary<int,int> GetBricksAmounts(int wholeAmount, int brickKindsAmount)
    {
        Dictionary<int, int> bricksAmounts = new();
        int startAmount = wholeAmount / brickKindsAmount; int modulo = wholeAmount % brickKindsAmount;
        for(int i = 0; i < brickKindsAmount; i++) bricksAmounts.Add(i, i == 0 ? startAmount + modulo : startAmount);

        int amountCheck = brickKindsAmount;
        for(int i = bricksAmounts.Count - 1; i > 0; i--)
        {
            while (bricksAmounts[i] > amountCheck)
            {
                for (int j = 0; j < i; j++)
                {
                    bricksAmounts[j]++;
                    bricksAmounts[i]--;
                }
            }
            amountCheck += bricksAmounts[i];
        }

        return bricksAmounts;
    }
    int BrickPercetangeSelect(Dictionary<int, int> bricksAmounts, List<int> keyBag)
    {
        List<int> tKeyBag = new(); keyBag.ForEach (k =>  tKeyBag.Add(k));
        Shuffle(tKeyBag);

        int sum = bricksAmounts.Sum(i => i.Value);
        int select = Random.Range(0, tKeyBag.Count);
        float random = Random.Range(0f, 1f);

        for(int i = 0; i < tKeyBag.Count; i++)
        {
            if (((float)bricksAmounts[tKeyBag[i]]/sum) < random) return i;
        }
        return 0;
    }
    public static void Shuffle<T>(List<T> list)
    {
        System.Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
    /*    void OldGenerateBricks()
        {
            const float step = 0.6f;
            int perLine = Mathf.FloorToInt(4.0f / step);

            int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
            for (int i = 0; i < LineCount; ++i)
            {
                for (int x = 0; x < perLine; ++x)
                {
                    Vector3 position = new(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brick.transform.SetParent(brickHolder.transform);
                    brick.BrickValues = bricksConfig.bricks.Find(x => x.value == pointCountArray[i]);
                    brick.OnDestroyed.AddListener(DestroyedBrick);
                }
            }
        }*/
}
