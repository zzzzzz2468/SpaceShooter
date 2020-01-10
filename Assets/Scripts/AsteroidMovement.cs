using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    //How many times it could be destroyed before it is considered dead
    private int AsteroidLevel = 0;
    private float Speed = 0;
    private bool isRunning = false;
    private float BoundPadding = 1;
    private bool Activated;

    //Gets the bullet moving
    public void SpawnAestroid(float speed, int level, Vector3 plyrTrans, float pad, bool direction = true)
    {
        if(direction)
            GetComponent<Rigidbody2D>().velocity = (plyrTrans - transform.position).normalized * speed;
        else
            GetComponent<Rigidbody2D>().velocity = plyrTrans * speed;

        AsteroidLevel = level;
        Speed = speed;
        BoundPadding = pad;
        Activated = true;
    }

    //Doesn't need all the other details if it already has them.
    public void Respawn(Vector3 plyrTrans, float speed = 0) {
        if (speed == 0)
            speed = Speed;
        GetComponent<Rigidbody2D>().velocity = (plyrTrans - transform.position).normalized * speed;
    }

    //Checks wether the gameObject exists already or not
    public bool Active() {
        return Activated;
    }

    private void Update()
    {
        //Detects if the Meteor is out of bounds
        Vector3 zeroPoint = GameManager.GM.camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 edgePoint = GameManager.GM.camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if (transform.position.x < zeroPoint.x - BoundPadding || transform.position.x > edgePoint.x + BoundPadding || 
            transform.position.y < zeroPoint.y - BoundPadding|| transform.position.y > edgePoint.y + BoundPadding) {
            GameManager.GM.MeteorOutOfBounds(gameObject);
        }

    }

    //If the asteroid runs into the player than do some damage
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            GameManager.GM.PlayerLosesLife();
        }
        if (other.gameObject.CompareTag("Shot")) {
            Destroy(other.gameObject);
            if (!isRunning){
                isRunning = true;
                SpawnInLittleOne();
            }
            GameManager.GM.MeteorDestroyed(gameObject);
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
            GameObject rock = Instantiate(littleOnes, new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle, Quaternion.identity, transform.parent);
            rock.GetComponent<AsteroidMovement>().SpawnAestroid(Speed * Random.Range(0.5f, 2f), AsteroidLevel - 1, 
                new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),BoundPadding, false);
            GameManager.GM.AddMeteor(rock);
        }
        Destroy(gameObject);
    }
}
