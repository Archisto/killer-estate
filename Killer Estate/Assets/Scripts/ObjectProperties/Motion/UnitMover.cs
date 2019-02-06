using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    private float moveSpeed;
    private float turnSpeed;

    public void Init(float moveSpeed, float turnSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.turnSpeed = turnSpeed;
    }
}
