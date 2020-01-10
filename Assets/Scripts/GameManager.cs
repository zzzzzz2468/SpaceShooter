using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game Manager / Public Info
    public static GameManager GM;

    [Header("Misc.")]
    public GameObject Player;
    public Camera camera;

    //Player
    [Header("Player")]
    public int lives = 3;
    public int DeathTime = 5;
    private bool isRunning = false;

    //Enemy
    [Header("Enemy")]
    public float EnemySpeed = 1;
    public float EnemyRotationSpeed = 0.2f;

    //Meteors
    [Header("Meteors")]
    public GameObject MeteorHolder;
    public int OnScreen = 3;
    public float BoundPadding = 2;
    public Vector2 AsteroidSpeed = new Vector2();

    //Waves
    public List<Transform> SpawnPoint = new List<Transform>();
    public List<GameObject> WaveOne = new List<GameObject>();
    public List<GameObject> AdditionalMeteors = new List<GameObject>();
    public Vector2 MeteorRate = new Vector2();
    private List<GameObject> ActiveAdditional = new List<GameObject>();
    private List<GameObject> MeteorsActive = new List<GameObject>();

    //UI
    [Header("UI")]
    public Text HealthText;
    public Text MarkerText;


    private void Awake(){
        //Makes this the only instance of the GameManager
        if (GM == null){
            GM = this;
            DontDestroyOnLoad(gameObject);
        }else
            Destroy(gameObject);

        Player = GameObject.FindGameObjectWithTag("Player");
        UpdateHealthCount();
        WaveManager();
        MarkerText.text = "";
        StartCoroutine(WaveManager());
    }

    void Update()
    {
        if (MeteorsActive.Count == 0 && ActiveAdditional.Count == 0 && WaveOne.Count == 0 && AdditionalMeteors.Count == 0) {
            MarkerText.text = "You Win.";
        }

    }

    //Spawns in the Waves
    private IEnumerator WaveManager()
    {
        while (!isRunning) {
            if (MeteorsActive.Count <= OnScreen - 1 && WaveOne.Count > 0) {
                Vector3 newPos = SpawnPoint[Random.Range(0, SpawnPoint.Count)].position;
                int meteorNumber = Random.Range(0, WaveOne.Count);
                
                GameObject rock = null;

                if ((WaveOne[meteorNumber].GetComponent<AsteroidMovement>() != null && !WaveOne[meteorNumber].GetComponent<AsteroidMovement>().Active()) ||
                    (WaveOne[meteorNumber].GetComponent<EnemyController>() != null && !WaveOne[meteorNumber].GetComponent<EnemyController>().Active())) {
                    rock = Instantiate(Resources.Load<GameObject>(WaveOne[meteorNumber].name), newPos, Quaternion.identity, MeteorHolder.transform);
                }else {
                    rock = WaveOne[meteorNumber];
                    rock.transform.position = newPos;
                    rock.SetActive(true);
                }

                //Determines if it's an enemy ship or a meteor
                if (rock.GetComponent<AsteroidMovement>() != null)
                    rock.GetComponent<AsteroidMovement>().SpawnAestroid(Random.Range(AsteroidSpeed.x, AsteroidSpeed.y), 3, Player.transform.position, BoundPadding);
                else
                    rock.GetComponent<EnemyController>().StartSettings(EnemySpeed, EnemyRotationSpeed);

                WaveOne.Remove(WaveOne[meteorNumber]);
                MeteorsActive.Add(rock);
            }
            if (AdditionalMeteors.Count != 0) {
                int count = Random.Range(1, 4);
                for (int i = 0; i < AdditionalMeteors.Count; i++) {
                    Vector3 newPos = SpawnPoint[Random.Range(0, SpawnPoint.Count)].position;
                    GameObject rock = AdditionalMeteors[Random.Range(0, AdditionalMeteors.Count - 1)];
                    rock.transform.position = newPos;
                    rock.SetActive(true);

                    //Determines if it's an Asteroid or an enemy ship
                    if(rock.GetComponent<AsteroidMovement>() != null)
                        rock.GetComponent<AsteroidMovement>().Respawn(Player.transform.position);
                    else
                        rock.GetComponent<EnemyController>().StartSettings(EnemySpeed, EnemyRotationSpeed);

                    AdditionalMeteors.Remove(rock);
                    ActiveAdditional.Add(rock);

                    if (i == count) break;
                }
            }
            yield return new WaitForSeconds(Random.Range(MeteorRate.x, MeteorRate.y));
        }
    }

    //Removes a Meteor if it goes out of bounds
    public void MeteorOutOfBounds(GameObject other) {
        if (MeteorsActive.Contains(other)) {
            WaveOne.Add(other);
            MeteorsActive.Remove(other);
            other.SetActive(false);
            return;
        }
        AdditionalMeteors.Add(other);
        ActiveAdditional.Remove(other);
        other.SetActive(false);
    }

    //Adds created meteors to the additional meteors list
    public void AddMeteor(GameObject other) {
        ActiveAdditional.Add(other);
    }

    //Takes the Meteors off of the list so that they aren't referenced anymore
    public void MeteorDestroyed(GameObject other) {
        if (MeteorsActive.Contains(other))
            MeteorsActive.Remove(other);
        else
            ActiveAdditional.Remove(other);
    }

    //When player dies
    public void PlayerLosesLife() {
        if (!isRunning)
            StartCoroutine(ReturnPlayerToLife());
    }
    //Re-spawns the player
    private IEnumerator ReturnPlayerToLife() {
        //Death
        isRunning = true;
        Player.SetActive(false);
        lives -= 1;
        CollectMeteors();
        //Detects if player is out of lives
        if (lives < 0) {
            MarkerText.text = "Game Over.";
            isRunning = false;
            yield break;
        }
        UpdateHealthCount();
        //Re-spawning
        for (int i = 0; i < DeathTime; i++){
            MarkerText.text = "Revive in..." + (DeathTime - i);
            yield return new WaitForSeconds(1);
        }

        MarkerText.text = "";
        Player.SetActive(true);
        Player.transform.position = Vector2.zero;
        isRunning = false;
        StartCoroutine(WaveManager());

    }
    //Updates the health number
    public void UpdateHealthCount(){
        HealthText.text = "Lives: " + lives;
    }

    //Removes all active meteors from the scene and gets ready to launch them again
    private void CollectMeteors() {
        int count = MeteorsActive.Count;

        //Removes Main meteors from the field
        for (int i = 0; i < count; i++) {
            MeteorsActive[0].SetActive(false);
            WaveOne.Add(MeteorsActive[0]);
            MeteorsActive.Remove(MeteorsActive[0]);
        }

        count = ActiveAdditional.Count;
        //Removes extras from the field
        for (int i = 0; i < count; i++){
            ActiveAdditional[0].SetActive(false);
            AdditionalMeteors.Add(ActiveAdditional[0]);
            ActiveAdditional.Remove(ActiveAdditional[0]);
        }
    }
}
