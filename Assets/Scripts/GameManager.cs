using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager GM;
    public GameObject Player;

    //Player
    public int lives = 3;
    public int DeathTime = 5;
    private bool isRunning = false;

    //Meteors
    public List<Transform> SpawnPoint = new List<Transform>();
    public GameObject MeteorHolder;
    public Vector2 AsteroidSpeed = new Vector2();
    public int OnScreen = 3;
    public List<GameObject> Wave = new List<GameObject>();
    private List<GameObject> MeteorsActive = new List<GameObject>();
    private List<GameObject> AdditionalMeteors = new List<GameObject>();

    //UI
    public Text HealthText;
    public Text GameOverText;

    //Misc
    public Camera camera;
    public Vector3 CamSize = new Vector3();

    private void Awake(){
        //Makes this the only instance of the GameManager
        if (GM == null){
            GM = this;
            DontDestroyOnLoad(gameObject);
        }else
            Destroy(gameObject);

        Player = GameObject.FindGameObjectWithTag("Player");
        UpdateHealthCount();
        SpawnAsteroids();
        CamSize = new Vector3((camera.orthographicSize * 2) * camera.aspect,camera.orthographicSize * 2, 0);
    }

    public void UpdateHealthCount()
    {
        HealthText.text = "Lives: " + lives;
    }

    private void SpawnAsteroids()
    {
        for (int i = 0; i < OnScreen; i++)
        {
            Vector3 newPos = SpawnPoint[Random.Range(0, SpawnPoint.Count - 1)].position;
            GameObject rock = Instantiate(Resources.Load<GameObject>("Meteor_Big"), newPos, Quaternion.identity, MeteorHolder.transform);
            rock.GetComponent<AsteroidMovement>().SpawnAestroid(1 * 0.7f, 3, Player.transform.position);
        }
    }

    public void PlayerLosesLife() {
        if (!isRunning)
            StartCoroutine(ReturnPlayerToLife());
    }
    IEnumerator ReturnPlayerToLife() {
        isRunning = true;
        Player.SetActive(false);
        lives -= 1;
        UpdateHealthCount();
        yield return new WaitForSeconds(DeathTime);
        if (lives < 0) {
            GameOverText.text = "Game Over.";
        }
        Player.SetActive(true);
        Player.transform.position = Vector2.zero;
        isRunning = false;
    }
}
