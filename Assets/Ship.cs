using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Player Player;

    [Header("Base values")]
    public float FudgeFactor = 100;
    public float Speed = 20;
    public float SteeringSpeed = 20;
    public float FlowSpeed = 5;
    public float SwaySpeed = 5;
    public float MaxSwayAngle = 15;
    public float CannonBallSpeed = 10;
    public GameObject SteeringPivot;
    public GameObject CannonBallPrefab;
    public Animation TextAnimation;
    public GameObject SailPivot;
    public GameObject CannonSpawnPoint;
    public List<GameObject> CannonSpawnPoints;

    [Header("Private")]
    public int Health = 100;
    public int MaxHealth = 100;
    public int Bilge = 0;
    public int MaxBilge = 100;
    public bool Turning;
    private Rigidbody rigi;
    
    [Header("Curses")]
    public float MisfireCurse = 0.1f;
    public int SailCurse = 0;
    public int NavCurse = 0;
    public int GunningCurse = 0;
    public int SwayCurse = 0;
    public bool CannonballSizeCurse = false;
    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        if (SteeringPivot == null)
        {
            //default to local transform position
            SteeringPivot = new GameObject("SteeringPoint(Gen)");
            SteeringPivot.transform.parent = this.transform;
            SteeringPivot.transform.position = Vector3.zero;
        }
        try
        {
            if (CannonBallPrefab == null)
            {
                //try to load from resources
                UnityEngine.Object cPrefab = Resources.Load("Assets/Prefabs/cannonball.prefab");
                
                GameObject pNewObject = (GameObject)Instantiate(cPrefab, Vector3.zero, Quaternion.identity);
                //pNewObject.transform.parent = CannonSpawnPoint.transform;
                //pNewObject.transform.position = Vector3.zero;
                var x = pNewObject.GetComponent<Rigidbody>();
                x.velocity = transform.TransformDirection(Vector3.forward * 10);
            }
        }catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    void ResetCurses()
    {
        SailCurse = 0;
        NavCurse = 0;
        GunningCurse = 0;
        SwayCurse = 0;
        CannonballSizeCurse = false;
        MisfireCurse = 0.1f;
    }

    public void ApplyCurse()
    {
        TextAnimation.Play();
        int r = UnityEngine.Random.Range(0, 6);
        float r2 = UnityEngine.Random.Range(0f, 1f);
        bool ff = UnityEngine.Random.Range(0, 2) == 1;
        int r3 = UnityEngine.Random.Range(0, 4);
        int r4 = ff ? -r3 : r3;

        if (r3 > 2)
        {
            ResetCurses();
        }
        
        switch (r)
        {
            case 0:
                SwayCurse = r4;
                break;
            case 1:
                SailCurse = r4;
                break;
            case 2:
                MisfireCurse = r2;
                break;
            case 3:
                NavCurse = r4;
                break;
            case 4:
                GunningCurse = r4;
                break;
            case 5:
                CannonballSizeCurse = true;
                break;
        }
    }

    void Shoot()
    {
        foreach (GameObject spawn in CannonSpawnPoints)
        {
            if (UnityEngine.Random.value < MisfireCurse)
            {
                continue;
            }
            var ball = Instantiate(CannonBallPrefab, spawn.transform.position, spawn.transform.rotation);
            if (CannonballSizeCurse)
            {
                ball.transform.localScale *= UnityEngine.Random.Range(.5f, 6f);
            }
            ball.GetComponent<Rigidbody>().velocity = spawn.transform.TransformDirection(Vector3.forward * CannonBallSpeed * (Player.Gunning + GunningCurse));
            ball.transform.parent = transform;
        }
    }

    private float _d = 0f;
    void Update()
    {
        //hack to apply curses every x seconds
        _d += Time.deltaTime;
        if((int)_d % 10 == 9)
        {
            _d = 0;
            //Debug.Log("Cursed!");
            //ApplyCurse();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        var speed = (Player.Sailing + SailCurse) * Speed * Time.deltaTime;
        var steeringSpeed = (Player.Navigating + NavCurse) * SteeringSpeed * Time.deltaTime;
        var flowSpeed = FlowSpeed * Time.deltaTime;
        Turning = false;
        int directionBias = 0;

        if (Input.GetKey("a"))
        {
            Turning = true;
            directionBias = 1;
        }

        if (Input.GetKey("d"))
        {
            Turning = true;
            directionBias = -1;
        }

        if (Turning)
        {
            //transform.position += Vector3.right * flowSpeed * directionBias;
            transform.RotateAround(SteeringPivot.transform.position, Vector3.up, -steeringSpeed * directionBias);
            //rigi.AddRelativeTorque(Vector3.forward * 1000);
            //rigi.AddRelativeTorque(Vector3.right * steeringSpeed * directionBias * FudgeFactor);
            //rigi.MoveRotation(rigi.rotation * Quaternion.Euler(Vector3.up * 100 * steeringSpeed * directionBias));

            //Make ship sway
            Vector3 rot = transform.eulerAngles + (Vector3.forward * (Player.Sailing + SwayCurse) * SwaySpeed * Time.deltaTime * directionBias);
            rot.z = ClampAngle(rot.z, -MaxSwayAngle * Mathf.Max(SwayCurse, 1), MaxSwayAngle * Mathf.Max(SwayCurse, 1));
            transform.localRotation = Quaternion.Euler(rot);

            //Make sails rotate
            if (SailPivot)
            {
                Vector3 rota = SailPivot.transform.localEulerAngles + (Vector3.up * FlowSpeed * 10 * Time.deltaTime * directionBias);
                rota.y = ClampAngle(rota.y, -45, 45);
                SailPivot.transform.localRotation = Quaternion.Euler(rota);
            }
        }
        else 
        { 
            //Rebalance ship
            Quaternion goalRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, goalRotation, Time.deltaTime);
            //Reangle sails
            if (SailPivot)
            {
                //Vector3 rota = SailPivot.transform.localEulerAngles + (Vector3.up * FlowSpeed * 10 * Time.deltaTime * directionBias);
                //rota.y = ClampAngle(rota.y, -45, 45);
                Quaternion resetSailRotation = Quaternion.Euler(SailPivot.transform.localEulerAngles.x, 0, SailPivot.transform.localEulerAngles.z);
                SailPivot.transform.localRotation = Quaternion.Slerp(SailPivot.transform.localRotation, resetSailRotation, Time.deltaTime);
            }
        }

        //Move forward/back
        if (Input.GetKey("w"))
        {
            //transform.position += transform.forward * speed * (Player.Navigating + SailCurse);
            rigi.velocity += transform.forward * speed;
        }
        //hack to try and always have the ship drift forwards when steering
        float distance = Vector3.Distance(rigi.velocity.normalized, transform.forward.normalized);
        //Debug.Log(distance);
        if(distance >= 0.3)
        {
            rigi.velocity += transform.forward * distance;
        }
        Debug.DrawLine(transform.position, transform.position + rigi.velocity, Color.green);

        if (Input.GetKey("s"))
        {
            transform.position -= transform.forward * speed * 0.2f;
        }

        if(transform.localEulerAngles.x != 0)
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    float getMaxElement(Vector3 v3)
    {
        return Mathf.Max(Mathf.Max(v3.x, v3.y), v3.z);
    }
}
