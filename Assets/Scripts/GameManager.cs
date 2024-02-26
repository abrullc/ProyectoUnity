using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Numerics;

public class GameManager : MonoBehaviour
{
    public List<GameObject> objects;
    public List<GameObject> enemies;
    public GameObject powerup;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI powerupTimeText;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public GameObject pauseScreen;
    private AudioSource music;
    public AudioClip boomSound;
    private PlayerController playerController;
    public bool isGameActive;
    private int score;
    private float spawnOjectRate = 1.2f;
    private float spawnEnemyRate = 6.0f;
    private float spawnPowerupRate = 1.0f;
    private int damage = 0;
    private float spawnRangeZ = 7.5f;
    private float spawnPosX = 130.0f;
    private float spawnPosY = 1.0f;
    private bool paused;

    // Start is called before the first frame update
    void Start()
    {
        music = GameObject.Find("GameManager").GetComponent<AudioSource>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (damage == 100)
        {
            playerController.playerAudio.PlayOneShot(boomSound, 1.0f);
            GameOver();
        }

        //Check if the user has pressed the P key
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangePaused();
        }
    }

    IEnumerator SpawnItem()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnOjectRate);
            int index = Random.Range(0, objects.Count);
            UnityEngine.Vector3 spawnPos = new UnityEngine.Vector3(spawnPosX, spawnPosY, Random.Range(-spawnRangeZ, spawnRangeZ));
            Instantiate(objects[index], spawnPos, objects[index].transform.rotation);
        }
    }

    IEnumerator SpawnEnemy()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnEnemyRate);
            int index = Random.Range(0, enemies.Count);
            UnityEngine.Vector3 spawnPos = new UnityEngine.Vector3(spawnPosX, spawnPosY, Random.Range(-spawnRangeZ, spawnRangeZ));
            Instantiate(enemies[index], spawnPos, enemies[index].transform.rotation);
        }
    }

    IEnumerator SpawnPowerup()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnPowerupRate);
            int chance = Random.Range(1, 6);
            if (chance == 5)
            {
                UnityEngine.Vector3 spawnPos = new UnityEngine.Vector3(spawnPosX, spawnPosY, Random.Range(-spawnRangeZ, spawnRangeZ));
                Instantiate(powerup, spawnPos, powerup.transform.rotation);
            }
        }
    }

    public void StartGame(int difficulty)
    {
        score = 0;
        damage = 0;
        spawnOjectRate /= difficulty;
        spawnEnemyRate /= difficulty;

        UpdateScore(score);
        damageText.text = "Damage: " + damage;
        
        isGameActive = true;
        StartCoroutine(SpawnItem());
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());

        titleScreen.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        damageText.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
        music.Stop();
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ChangePaused()
        {
        if (!paused)
        {
            paused = true;
            pauseScreen.SetActive(true);
            music.Pause();
            Time.timeScale = 0;
        }
        else
        {
            paused = false;
            pauseScreen.SetActive(false);
            music.UnPause();
            Time.timeScale = 1;
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void UpdateDamage(int newDamage)
    {
        if (damage < 100 && damage+newDamage <= 100)
        {
            damage += newDamage;
        } else {
            damage = 100;
        }

        damageText.text = "Damage: " + damage;
    }
}
