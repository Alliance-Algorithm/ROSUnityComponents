using System;
using System.Collections;
using System.Collections.Generic;
using ROS2;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class OccupancyGrid : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    ROSConnection ros;
    public Lidar lidar;
    float[,] map = new float[61, 61];
    sbyte[] smap = new sbyte[61 * 61];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.Nav.OccupancyGridMsg>(TopicName);
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
    }
    void Publisher()
    {
        RosMessageTypes.Nav.OccupancyGridMsg msg = new();
        msg.info.height = (uint)map.GetLength(0);
        msg.info.width = (uint)map.GetLength(1);
        msg.header = new() { frame_id = FrameId };
        msg.data = smap;
        msg.info.resolution = 0.1f;
        msg.info.origin = new()
        {
            position = new() { x = -3, y = 3, z = 0 },
            orientation = new() { x = 1, y = 0, z = 0, w = 0 }
        };
        ros.Publish(TopicName, msg);
    }
}
