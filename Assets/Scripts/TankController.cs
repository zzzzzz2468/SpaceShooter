using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    //Player Movement
    public float SpeedAcc = 10;
    public float MaxSpeed = 10;
    public float StopRate = 1;
    public float TurnSpeed = 10;
    public bool CanMove = true;

    //Other Stats
    public float BulletSpeed = 20;
    public float BulletLifeSpan = 3;
    public float FireRate = 3;
    private float Timer = 0;
    public float BulletForce = 3;

    //References
    private Rigidbody2D rb2d;
    public GameObject FireRight, FireLeft, FireCenter;
    public Transform ShotHolder;
    public GameObject BulletRegular;

    private void Awake(){
        rb2d = GetComponent<Rigidbody2D>();
        FireRight.SetActive(false);
        FireLeft.SetActive(false);
        FireCenter.SetActive(false);
    }

    private void Update() {
        if (CanMove){
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Movement(horizontal, vertical);

            FireCenter.SetActive(vertical > 0.1f);
            FireLeft.SetActive(horizontal > 0.1f);
            FireRight.SetActive(horizontal < -0.1f);
        }

        //Shoots if the player presses the space bar it shoots
        if (Input.GetButton("Jump") && Time.time > Timer) {
            ShootShot();
            Timer = Time.time + FireRate;
        }
    }

    private void ShootShot()
    {
        GameObject shot = Instantiate(BulletRegular, transform.position, Quaternion.identity, ShotHolder);
        shot.transform.rotation = transform.rotation;
        shot.GetComponent<ShotController>().SpawnShot(BulletSpeed, BulletLifeSpan);
        rb2d.AddForce(transform.up * -BulletForce, ForceMode2D.Impulse);
    }

    //Moves the player by rotating and forwards and back
    private void Movement(float hor, float ver) {

        //Moves the player
        rb2d.AddForce(transform.up * SpeedAcc * Mathf.Clamp(ver, 0 , 1));

        //Speed Caps the player
        if (rb2d.velocity.magnitude >= MaxSpeed)
            rb2d.AddForce(-transform.up * SpeedAcc * Mathf.Clamp(ver, 0, 1));

        //Slows the Player down if the player isn't pressing anything and they're still moving
        if (ver < 0.1f && Mathf.Abs(rb2d.velocity.magnitude) > 0.1f)
            rb2d.AddForce(-rb2d.velocity.normalized * StopRate);


        transform.Rotate(transform.forward * TurnSpeed * -hor);
    }
}
