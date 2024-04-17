using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Lidar : MonoBehaviour
{
    /// <summary>
    /// how many raycast per 1 scan around 360
    /// </summary>
    public int RaycastPrecision = 360;
    bool isRunning = false;
    public float MaxLength = 100;
    public int fps = 10;
    float time = 0;
    float time_ = 0;
    List<Vector3> points = new();
    public List<Vector3> Point => points;
    void Start()
    {
        time = 1.0f / fps;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        time_ += Time.deltaTime;
        if (time_ >= time)
        {
            RaycastAround();
            time_ = 0;
        }

    }

    void RaycastAround()
    {
        isRunning = true;
        var result = new NativeArray<RaycastHit>(RaycastPrecision, Allocator.TempJob);
        var commands = new NativeArray<RaycastCommand>(RaycastPrecision, Allocator.TempJob);

        Vector3 origin = transform.position;

        for (int i = 0; i < RaycastPrecision; i++)
        {
            var phi = Mathf.Acos(UnityEngine.Random.Range(-0.788010753606722f, 0.121869343405147f)) * 2 * (0.5f + UnityEngine.Random.Range(-1, 0));
            var theta = UnityEngine.Random.Range(-2 * Mathf.PI, 2 * Mathf.PI);
            Vector3 direction = MaxLength * new Vector3(Mathf.Sin(phi) * Mathf.Cos(theta), Mathf.Cos(phi), Mathf.Sin(phi) * Mathf.Sin(theta));
            commands[i] = new RaycastCommand(origin, direction, QueryParameters.Default);

        }
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, result, RaycastPrecision, 1);

        handle.Complete();
        var points = new List<Vector3>();
        foreach (var hit in result)
        {
            if (hit.collider != null)
            {
                points.Add(hit.point - transform.position);
                // Debug.DrawLine(transform.position, hit.point, Color.red, time);
            }
        }
        this.points = points;

        result.Dispose();
        commands.Dispose();
    }
}
