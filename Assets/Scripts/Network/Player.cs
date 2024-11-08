using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public int ID { get; set; }
    Vector3 moveDir;
    public Vector3 MoveDir { set { moveDir = value.normalized; } }
    NetworkManager _network;
    System.Random _rand = new();
    // Start is called before the first frame update
    void Start()
    {
        _network = FindObjectOfType<NetworkManager>();
        StartCoroutine(CoSendPacket());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDir * Time.deltaTime * 5;
    }

    IEnumerator CoSendPacket()
    {

        while (true)
        {
            yield return new WaitForSeconds(2);
            C_Move p = new C_Move();
            p.posX = _rand.Next(-50, 50);
            p.posY = 0;
            p.posZ = _rand.Next(-50, 50);

            ArraySegment<byte> segment = p.Write();
            _network.Send(segment);
        }
    }


}
