using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    //How many times it could be destroyed before it is considered dead
    private int AsteroidLevel = 0;
    private float Speed = 0;
    private bool isRunning = false;

    //Gets the bullet moving
    public void SpawnAestroid(float speed, int Level, Vector3 PlyrTrans, bool direction = true)
    {
        if(direction)
            GetComponent<Rigidbody2D>().velocity = (PlyrTrans - transform.position).normalized * speed;
        else
            GetComponent<Rigidbody2D>().velocity = PlyrTrans * speed;

        AsteroidLevel = Level;
        Speed = speed;
    }

    private void Update()
    {
        Vector3 zeroPoint = GameManager.GM.camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 edgePoint = GameManager.GM.camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        print(zeroPoint);
        print(edgePoint);

        if (transform.position.x < zeroPoint.x || transform.position.x > edgePoint.x || transform.position.y > zeroPoint.y || transform.position.y < edgePoint.y) {
            print("Bro");
        }

    }

    //If the asteroid runs into the player than do some damage
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            GameManager.GM.PlayerLosesLife();
        }
        if (other.gameObject.CompareTag("Shot")) {
            if (!isRunning){
                isRunning = true;
                SpawnInLittleOne();
            }
            Destroy(other.gameObject);
        }
    }

    //This creates little asteroids if it destroys this ship
    //Also randomizes their direction so that it can go anywhere
    private void SpawnInLittleOne() {
        GameObject littleOnes;
        if (AsteroidLevel == 3){
            littleOnes = Resources.Load<GameObject>("Meteor_Medium");
        }else if (AsteroidLevel == 2){
            littleOnes = Resources.Load<GameObject>("Meteor_Small");
        }else{
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < Random.Range(2, 5); i++) {
            GameObject rock = Instantiate(littleOnes, transform.position, Quaternion.identity, transform.parent);
            rock.GetComponent<AsteroidMovement>().SpawnAestroid(Speed * Random.Range(0.5f, 3f), AsteroidLevel - 1, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), false);
        }
        Destroy(gameObject);
    }
}
