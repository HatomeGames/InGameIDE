using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] float movePower;
    [SerializeField] float rotatePower;

    Transform tr;
    Rigidbody rb;
    Vector3 initPos;
    Quaternion initRot;

    void Awake()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
        initPos = tr.position;
        initRot = tr.rotation;
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(tr.rotation * force * movePower);
    }

    public void AddTorque(Vector3 torque)
    {
        rb.AddTorque(tr.rotation * torque * rotatePower);
    }

    public void ResetPosture()
    {
        tr.position = initPos;
        tr.rotation = initRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
