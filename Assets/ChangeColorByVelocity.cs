using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorByVelocity : MonoBehaviour
{
    [GradientUsage(true)]
    public Gradient gradient;
    public float minVelocity;
    public float maxVelocity;

    private PhysicsObject _physObj;
    private Material _material;

    private void Start()
    {
        _physObj = this.gameObject.GetComponent<PhysicsObject>();
        _material = this.gameObject.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        float vel = _physObj.vel.magnitude;
        var col = gradient.Evaluate((vel - minVelocity) / (maxVelocity - minVelocity));
        _material.SetColor("_EmissionColor", col);
    }
}
