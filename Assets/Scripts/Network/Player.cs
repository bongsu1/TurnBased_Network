using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public int ID { get; set; }
    protected Vector3 moveDir;
    public Vector3 MoveDir { set { moveDir = value.normalized; } }
    protected NetworkManager _network;
    // Start is called before the first frame update
    protected virtual void  Start()
    {
        _network = FindObjectOfType<NetworkManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.position += moveDir * Time.deltaTime * 5;
    }

    

}
