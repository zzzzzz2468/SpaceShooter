using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float Speed = 10;
    public float RotationSpeed = 10;
    private Rigidbody2D rb2d;
    private bool Activated = false;

    //Initializes everything
    public void StartSettings(float speed, float rotationSpeed) {
        Speed = speed;
        RotationSpeed = rotationSpeed;
        rb2d = GetComponent<Rigidbody2D>();
        Activated = true;
    }
    void Update()
    {
        rb2d.velocity = transform.up * Speed;
        float step = RotationSpeed * Time.deltaTime;
        Vector3 newPos = new Vector3(GameManager.GM.Player.transform.position.x - transform.position.x,
            GameManager.GM.Player.transform.position.y - transform.position.y, 0);
        transform.up = Vector3.Slerp(transform.up, newPos, RotationSpeed / 10);
    }
    //Returns if it has been spawned or not
    public bool Active() {
        return Activated;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Shot")) {
            Destroy(other.gameObject);
            GameManager.GM.MeteorDestroyed(gameObject);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.GM.PlayerLosesLife();

        }
    }
}
