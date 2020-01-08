using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private float Speed = 10;
    private float RotationSpeed = 10;
    private Rigidbody2D rb2d;

    void Start()
    {
        StartSettings(1, 10);
    }

    //Initializes everything
    public void StartSettings(float speed, float rotationSpeed) {
        Speed = speed;
        RotationSpeed = rotationSpeed;
        rb2d = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        rb2d.velocity = transform.up * Speed;
        float step = RotationSpeed * Time.deltaTime;
        Vector3 newRot = Vector3.RotateTowards(transform.right, GameManager.GM.Player.transform.position, Speed, 0);
        transform.rotation = Quaternion.LookRotation(newRot);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Shot"))
        {
            Destroy(gameObject);
        }
    }
}
