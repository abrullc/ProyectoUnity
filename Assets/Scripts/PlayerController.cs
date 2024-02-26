using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    private float zRange = 7.5f;
    public float speed = 15.0f;
    private float impulseForce = 10.0f;
    private bool hasPowerup = false;
    private int powerupTime = 5;
    private GameManager gameManager;
    public AudioSource playerAudio;
    public AudioClip moneySound;
    public AudioClip crashSound;
    public Camera mainCamera;
    public Camera upCamera;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {    
        if (gameManager.isGameActive)
        {
            // If player tries to surpass left limit, it will be stoped
            if (transform.position.z < -zRange)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -zRange);
            }

            // If player tries to surpass right limit, it will be stoped
            if (transform.position.z > zRange)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, zRange);
            }

            // Movement of the player
            // The player can move in the Z axis
            horizontalInput = Input.GetAxis("Horizontal");

            transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);

            if(Input.GetKeyDown(KeyCode.C))
            {
                mainCamera.enabled = !mainCamera.enabled;
                upCamera.enabled = !upCamera.enabled;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
        Vector3 awayFromPlayer =  other.gameObject.transform.position - transform.position;

        if (!hasPowerup) {
            if (other.gameObject.CompareTag("Obstacle"))
            {
                playerAudio.PlayOneShot(crashSound, 1.0f);
                otherRigidbody.AddForce(awayFromPlayer * impulseForce, ForceMode.Impulse);
                gameManager.UpdateDamage(3);
            } else if (other.gameObject.CompareTag("Enemy"))
            {
                gameManager.UpdateDamage(100);
            }
        } else {
            otherRigidbody.AddForce(awayFromPlayer * impulseForce*2, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if player collides with money play sound and destroy money object
        if (other.gameObject.CompareTag("Money"))
        {
            playerAudio.PlayOneShot(moneySound, 1.0f);
            gameManager.UpdateScore(1);
            Destroy(other.gameObject);

        } else if (other.gameObject.CompareTag("Powerup") && !hasPowerup)
        {
            playerAudio.PlayOneShot(moneySound, 1.0f);
            Destroy(other.gameObject);
            StartCoroutine(PowerupCorroutine());

        }
    }

    IEnumerator PowerupCorroutine()
    {
        hasPowerup = true;
        Time.timeScale = 2;
        int timeLeft = powerupTime;
        gameManager.powerupTimeText.gameObject.SetActive(true);

        while (timeLeft > 0)
        {
            gameManager.powerupTimeText.text = "Powerup ends in: "+timeLeft;
            yield return new WaitForSeconds(2);
            timeLeft--;
        }

        Time.timeScale = 1;
        gameManager.powerupTimeText.gameObject.SetActive(false);
        hasPowerup = false;
    }
}
