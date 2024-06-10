using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class LocalGridMapBuilder : MonoBehaviour
{
    public float BoxSize = 6;
    public float Resolution = 0.1f;

    public float ESDF_MaxDistance = 2f;

    float halfBoxsize;

    public bool ShowBin = false;
    public bool ShowESDF = false;
    sbyte[,] map_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        map_ = new sbyte[(int)Mathf.Ceil(BoxSize / Resolution) + 1, (int)Mathf.Ceil(BoxSize / Resolution) + 1];
        halfBoxsize = BoxSize / 2;
    }

    // Update is called once per frame
    public void Build(out sbyte[,] map)
    {
        if (map_ == null)
        {
            map = new sbyte[(int)Mathf.Ceil(BoxSize / Resolution) + 1, (int)Mathf.Ceil(BoxSize / Resolution) + 1];
            return;
        }
        for (int x = 0, k = (int)Mathf.Ceil(BoxSize / Resolution) + 1; x < k; x += 1)
        {
            for (int y = 0; y < k; y += 1)
            {
                Vector3 p1 = transform.position + transform.right * (x * Resolution - halfBoxsize) + Vector3.up * 2 + transform.forward * (-y * Resolution + halfBoxsize);
                // Vector3 p1 = transform.position + new Vector3(x * Resolution - halfBoxsize, 2, y * Resolution - halfBoxsize);

                if (!Physics.Raycast(p1, Vector3.down, out var hitInfo, 5))
                    map_[x, y] = 0;
                else if (!NavMesh.SamplePosition(hitInfo.point, out var hit, 0.3f, NavMesh.AllAreas))
                    map_[x, y] = 0;
                else
                    map_[x, y] = -1;

                if (ShowBin)
                    Debug.DrawLine(p1, p1 + Vector3.forward * Resolution, new Color(
                    1 + map_[x, y],
                    -map_[x, y],
                    0), 0.5f);

            }
        }
        map = map_;
    }

    public void MakeESDF(ref sbyte[,] map)
    {
        Queue<(int x, int y)> queue = new();

        for (float x = Resolution; x < BoxSize; x += Resolution)
        {
            for (float y = Resolution; y < BoxSize; y += Resolution)
            {
                (int x, int y) pos = ((int)(x / Resolution), (int)(y / Resolution));
                if (map[pos.x, pos.y] == 0)
                {
                    if (map[pos.x + 1, pos.y] != 0)
                    {
                        map[pos.x + 1, pos.y] = Math.Min(map[pos.x + 1, pos.y], (sbyte)(Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x + 1, pos.y));
                    }
                    if (map[pos.x - 1, pos.y] != 0)
                    {
                        map[pos.x - 1, pos.y] = Math.Min(map[pos.x - 1, pos.y], (sbyte)(Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x - 1, pos.y));
                    }
                    if (map[pos.x, pos.y + 1] != 0)
                    {
                        map[pos.x, pos.y + 1] = Math.Min(map[pos.x, pos.y + 1], (sbyte)(Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x, pos.y + 1));
                    }
                    if (map[pos.x, pos.y - 1] != 0)
                    {
                        map[pos.x, pos.y - 1] = Math.Min(map[pos.x, pos.y - 1], (sbyte)(Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x, pos.y - 1));
                    }
                    if (map[pos.x + 1, pos.y + 1] != 0)
                    {
                        map[pos.x + 1, pos.y + 1] = Math.Min(map[pos.x + 1, pos.y + 1], (sbyte)(math.SQRT2 * Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x + 1, pos.y + 1));
                    }
                    if (map[pos.x - 1, pos.y - 1] != 0)
                    {
                        map[pos.x - 1, pos.y - 1] = Math.Min(map[pos.x - 1, pos.y - 1], (sbyte)(math.SQRT2 * Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x - 1, pos.y - 1));
                    }
                    if (map[pos.x - 1, pos.y + 1] != 0)
                    {
                        map[pos.x - 1, pos.y + 1] = Math.Min(map[pos.x - 1, pos.y + 1], (sbyte)(math.SQRT2 * Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x - 1, pos.y + 1));
                    }
                    if (map[pos.x + 1, pos.y - 1] != 0)
                    {
                        map[pos.x + 1, pos.y - 1] = Math.Min(map[pos.x + 1, pos.y - 1], (sbyte)(math.SQRT2 * Resolution / ESDF_MaxDistance * 100));
                        queue.Enqueue((pos.x + 1, pos.y - 1));
                    }
                }
            }
        }

        while (queue.Count > 0)
        {
            (int x, int y) pos = queue.Dequeue();
            if (map[pos.x, pos.y] != 100)
            {
                if (pos.x + 1 < (int)Mathf.Ceil(BoxSize / Resolution) && map[pos.x + 1, pos.y] == 100)
                {
                    uint temp = (uint)map[pos.x, pos.y] + (uint)(Resolution / ESDF_MaxDistance * 100);
                    map[pos.x + 1, pos.y] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x + 1, pos.y));
                }
                if (pos.x - 1 >= 0 && map[pos.x - 1, pos.y] == 100)
                {
                    uint temp = (uint)map[pos.x, pos.y] + (uint)(Resolution / ESDF_MaxDistance * 100);
                    map[pos.x - 1, pos.y] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x - 1, pos.y));
                }
                if (pos.y + 1 < (int)Mathf.Ceil(BoxSize / Resolution) && map[pos.x, pos.y + 1] == 100)
                {
                    uint temp = (uint)map[pos.x, pos.y] + (uint)(Resolution / ESDF_MaxDistance * 100);
                    map[pos.x, pos.y + 1] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x, pos.y + 1));
                }
                if (pos.y - 1 >= 0 && map[pos.x, pos.y - 1] == 100)
                {
                    uint temp = (uint)map[pos.x, pos.y] + (uint)(Resolution / ESDF_MaxDistance * 100);
                    map[pos.x, pos.y - 1] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x, pos.y - 1));
                }
                if (pos.x + 1 < (int)Mathf.Ceil(BoxSize / Resolution) && pos.y + 1 < (int)Mathf.Ceil(BoxSize / Resolution) && map[pos.x + 1, pos.y + 1] == 100)
                {
                    uint temp = (uint)map[pos.x, pos.y] + (uint)Mathf.Ceil(math.SQRT2 * Resolution / ESDF_MaxDistance * 100);
                    map[pos.x + 1, pos.y + 1] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x + 1, pos.y + 1));
                }
                if (pos.x - 1 >= 0 && pos.y - 1 >= 0 && map[pos.x - 1, pos.y - 1] == 100)
                {
                    uint temp = (uint)map[pos.x, pos.y] + (uint)Mathf.Ceil(math.SQRT2 * Resolution / ESDF_MaxDistance * 100);
                    map[pos.x - 1, pos.y - 1] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x - 1, pos.y - 1));
                }
                if (pos.y + 1 < (int)Mathf.Ceil(BoxSize / Resolution) && pos.x - 1 >= 0 && map[pos.x - 1, pos.y + 1] == 100)
                {
                    uint temp = (uint)map[pos.x, pos.y] + (uint)Mathf.Ceil(math.SQRT2 * Resolution / ESDF_MaxDistance * 100);
                    map[pos.x - 1, pos.y + 1] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x - 1, pos.y + 1));
                }
                if (pos.x + 1 < (int)Mathf.Ceil(BoxSize / Resolution) && pos.y - 1 >= 0 && map[pos.x + 1, pos.y - 1] == 100)
                {
                    uint temp = (uint)(map[pos.x, pos.y] + (uint)Mathf.Ceil(math.SQRT2 * Resolution / ESDF_MaxDistance * 100));
                    map[pos.x + 1, pos.y - 1] = (sbyte)Math.Clamp(temp, 0, 100);
                    queue.Enqueue((pos.x + 1, pos.y - 1));
                }
            }
        }

        if (ShowESDF)

            for (float x = 0; x <= BoxSize; x += Resolution)
            {
                for (float y = 0; y <= BoxSize; y += Resolution)
                {
                    Vector3 p1 = transform.position + new Vector3(x - halfBoxsize, 2, y - halfBoxsize);
                    Debug.DrawLine(p1, p1 + Vector3.forward * 0.1f, new Color(
                                    1 - map[(int)(x / Resolution), (int)(y / Resolution)] / 100.0f,
                    map[(int)(x / Resolution), (int)(y / Resolution)] / 100.0f,
                                    0), 0.5f);
                }
            }
    }
}
