using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PhysicsManager;

public class PhysicsObject : MonoBehaviour
{
    public float mass = 1;
    public Vector3 pos = Vector3.zero;
    public Vector3 vel = Vector3.zero;

    public float minMass = 1;
    public float maxMass = 1;

    public Vector3 homePos = Vector3.zero;
    [Range(0, 1)]
    public float homingFactor = 0;
    public float homingStrength = 0;

    private void Start()
    {
        this.mass = Random.Range(minMass, maxMass);
    }

    public void UpdatePhysics(PhysicsData data)
    {
        this.pos = data.pos;
        this.vel = data.vel;

        this.transform.position = this.pos;
    }
}
