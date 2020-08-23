using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CannonBall : MonoBehaviour
{
    public float TimeToLiveSeconds = 5;
    public float Age = 0;
    public Ship PlayerShip;

    private Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        PlayerShip = transform.parent.gameObject.GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        Age += Time.deltaTime;
        if(Age > TimeToLiveSeconds)
        {
            Destroy(gameObject); //TODO: replace this with a pool of objects to optimise resource usage
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug-draw all contact points and normals
        Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Color.white);

        if(collision.gameObject.tag == "Shippy")
        {
            bool dead = collision.gameObject.GetComponent<Wander>().Shot();
            if (dead)
            {
                PlayerShip.Player.PiecesOfEight += Random.Range(100, 1000);
                PlayerShip.Player.Dubloons += Random.value > 0.5 ? 1 : 0;
                PlayerShip.ApplyCurse();
                Destroy(collision.gameObject);
            }
        }

        // Play a sound if the colliding objects had a big impact.
        body.velocity = -body.velocity * 2;
    }
}
