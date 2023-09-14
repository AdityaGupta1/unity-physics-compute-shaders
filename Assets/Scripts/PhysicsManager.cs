using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    private List<PhysicsObject> _physObjs = new List<PhysicsObject>();
    private PhysicsData[] _physDatas;
    private int _numPhysObjs;

    private ComputeBuffer _physDatasBuffer;

    public ComputeShader physicsCompute;
    public int numSteps = 100;
    public float timeScale = 1;

    public GameObject sunPrefab;
    public GameObject orangeParticlePrefab;

    public struct PhysicsData
    {
        public float mass;
        public Vector3 pos;
        public Vector3 vel;
        public float homingFactor;
        public float homingStrength;
        public Vector3 homePos;
    }

    private void Start()
    {
        SpawnObjects();
        SetupBuffers();
    }

    private void SpawnObjects()
    {
        var sun = GameObject.Instantiate(sunPrefab).GetComponent<PhysicsObject>();
        _physObjs.Add(sun);

        sun.homingFactor = 0.02f;
        sun.homingStrength = 10f;

        //for (int i = 0; i < 5; ++i)
        //{
        //    var extraSun = GameObject.Instantiate(sunPrefab).GetComponent<PhysicsObject>();
        //    _physObjs.Add(extraSun);

        //    extraSun.pos = Random.insideUnitSphere.normalized * Random.Range(10, 15);
        //    extraSun.homePos = extraSun.pos;
        //    extraSun.homingFactor = 0.01f;
        //    extraSun.homingStrength = 0.5f;
        //}

        for (int i = 0; i < 2000; ++i)
        {
            var orangeParticle = GameObject.Instantiate(orangeParticlePrefab).GetComponent<PhysicsObject>();
            _physObjs.Add(orangeParticle);

            var posLength = Random.Range(3, 12);
            orangeParticle.pos = Random.insideUnitSphere.normalized * posLength;
            orangeParticle.vel = Vector3.Cross(orangeParticle.pos, new Vector3(0, 0, 1)).normalized * Mathf.Sqrt(sun.mass / posLength);
        }

        _numPhysObjs = _physObjs.Count;

        _physDatas = new PhysicsData[_numPhysObjs];
        for (int i = 0; i < _numPhysObjs; ++i)
        {
            var physObj = _physObjs[i];

            _physDatas[i] = new PhysicsData
            {
                mass = physObj.mass,
                pos = physObj.pos,
                vel = physObj.vel,
                homingFactor = physObj.homingFactor,
                homingStrength = physObj.homingStrength,
                homePos = physObj.homePos
            };
        }
    }

    private void SetupBuffers()
    {
        _physDatasBuffer = new ComputeBuffer(_numPhysObjs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(PhysicsData)));
    }

    private void Update()
    {
        UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        // COMPUTE VELOCITY
        int kernelIdxVel = physicsCompute.FindKernel("ComputeVelocity");

        _physDatasBuffer.SetData(_physDatas, 0, 0, _numPhysObjs);
        physicsCompute.SetBuffer(kernelIdxVel, "physDatas", _physDatasBuffer);
        physicsCompute.SetInt("numObjs", _numPhysObjs);

        physicsCompute.SetFloat("deltaTime", Time.deltaTime / numSteps * timeScale);
        physicsCompute.SetInt("numSteps", numSteps);

        // COMPUTE POSITION
        int kernelIdxPos = physicsCompute.FindKernel("ComputePosition");

        physicsCompute.SetBuffer(kernelIdxPos, "physDatas", _physDatasBuffer);

        int numBlocks = Mathf.CeilToInt(_numPhysObjs / 64f);
        for (int i = 0; i < numSteps; ++i)
        {
            physicsCompute.Dispatch(kernelIdxVel, numBlocks, 1, 1);
            physicsCompute.Dispatch(kernelIdxPos, numBlocks, 1, 1);
        }

        _physDatasBuffer.GetData(_physDatas);

        for (int i = 0; i < _numPhysObjs; ++i)
        {
            _physObjs[i].UpdatePhysics(_physDatas[i]);
        }
    }

    private void OnDestroy()
    {
        _physDatasBuffer.Release();
    }
}
