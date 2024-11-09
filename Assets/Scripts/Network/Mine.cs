using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Mine : Player
{
    System.Random _rand = new();

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CoSendPacket());
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
