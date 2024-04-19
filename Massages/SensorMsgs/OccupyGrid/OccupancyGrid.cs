using System;
using System.Collections;
using System.Collections.Generic;
using ROS2;
using std_msgs.msg;
using UnityEngine;

[RequireComponent(typeof(ROS2UnityComponent))]
public class OccupancyGrid : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    private IPublisher<nav_msgs.msg.OccupancyGrid> publisher;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    public Lidar lidar;
    float[,] map = new float[61, 61];
    sbyte[] smap = new sbyte[61 * 61];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        publisher = ros2Node.CreatePublisher<nav_msgs.msg.OccupancyGrid>(TopicName);
        _time = 1 / fps;
        time = _time;
    }

    float _time;
    float time;
    void Update()
    {
        _time += Time.deltaTime;
        if (time > _time)
            return;
        _time = 0;
        PointCloudToMap();
        Publisher();
    }

    void PointCloudToMap()
    {
        var p = lidar.Point;
        for (int i = 0; i < 61; i++)
            for (int j = 0; j < 61; j++)
                map[i, j] = float.MinValue;
        foreach (var i in p)
        {
            if (i.y > 1)
                continue;
            else if (Mathf.Abs(i.x) > 3 || Mathf.Abs(i.z) > 3)
                continue;

            // Debug.DrawLine(i + lidar.transform.position + Vector3.forward * 0.01f, i + lidar.transform.position, Color.red, time);

            var x = (int)Mathf.Round((i.x + 3) * 10);
            var y = (int)Mathf.Round((i.z + 3) * 10);
            if (Mathf.Abs(i.x) < 0.2f && Mathf.Abs(i.z) < 0.2f)
                map[x, y] = 0;
            else
                map[x, y] = Mathf.Max(map[x, y], i.y + 0.5f);
        }
        float[,] temp = (float[,])map.Clone();

        for (int i = 2; i < 59; i++)
        {
            for (int j = 2; j < 59; j++)
            {
                if (map[i, j] == float.MinValue)
                    map[i, j] = 0;
            }
        }
        for (int i = 2; i < 59; i++)
        {
            for (int j = 2; j < 59; j++)
            {
                smap[i * 61 + j] = 100;
                if (map[i, j] < 0.05 || map[i, j] == float.MinValue)
                    smap[i * 61 + j] = 0;
                if (map[i, j] < 0.05
                && map[i + 1, j] < 0.05
                && map[i - 1, j] < 0.05
                && map[i, j + 1] < 0.05
                && map[i, j - 1] < 0.05)
                    smap[i * 61 + j] = 0;

                if (Mathf.Abs(temp[i + 2, j] - temp[i - 2, j]) / 2 > 0.1f)
                    continue;
                if (Mathf.Abs(temp[i, j + 2] - temp[i, j - 2]) / 2 > 0.1f)
                    continue;

            }
        }
        // foreach (var i in p)
        // {
        //     if (i.y > 0)
        //         continue;
        //     else if (Mathf.Abs(i.x) > 3 || Mathf.Abs(i.z) > 3)
        //         continue;
        //     var x = (int)Mathf.Round((i.x + 3) * 10);
        //     var y = (int)Mathf.Round((i.z + 3) * 10);
        //     if (map[x, y] != 0)
        //         Debug.DrawLine(i + lidar.transform.position + Vector3.forward * 0.01f, i + lidar.transform.position, Color.red, time);
        // }
    }
    void Publisher()
    {
        if (!ros2Unity.Ok())
            return;
        nav_msgs.msg.OccupancyGrid msg = new();
        msg.Info.Height = (uint)map.GetLength(0);
        msg.Info.Width = (uint)map.GetLength(1);
        msg.Header = new Header() { Frame_id = FrameId };
        msg.Data = smap;
        msg.Info.Resolution = 0.1f;
        msg.Info.Origin = new geometry_msgs.msg.Pose()
        {
            Position = new geometry_msgs.msg.Point() { X = -3, Y = 3, Z = 0 },
            Orientation = new() { X = 1, Y = 0, Z = 0, W = 0 }
        };
        publisher.Publish(msg);
        msg.Dispose();

    }
}
