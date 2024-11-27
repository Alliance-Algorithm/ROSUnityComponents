using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LocalGridMapBuilder))]
class ESDFBuilder : MonoBehaviour
{
    int SizeX = 28;
    int SizeY = 16;
    public string FilePath;
    public float Resolution = 0.3f;
    public Transform Sentry;
    public ESDFMap map_;
    ESDFMapData mapData;
    public bool Show = false;
    public void Bake()
    {
        int SizeX = 28;
        int SizeY = 16;
        SizeX = Mathf.RoundToInt(SizeX / Resolution);
        SizeY = Mathf.RoundToInt(SizeY / Resolution);
        var halfBoxsizeX = SizeX / 2.0f;
        var halfBoxsizeY = SizeY / 2.0f;
        ESDFMap map = new ESDFMap
        {
            SizeX = SizeX + 1,
            SizeY = SizeY + 1,
            Resolution = Resolution
        };
        for (int x = 0, k = SizeX + 1; x < k; x += 1)
        {
            for (int y = 0, l = SizeY + 1; y < l; y += 1)
            {
                Vector3 p1 = new Vector3(x * Resolution - 14f, 2.5f, y * Resolution - 8f);
                // Vector3 p1 = transform.position + transform.right * (x * Resolution - halfBoxsizeX) + Vector3.up * 2 + transform.forward * (-y * Resolution + halfBoxsizeY);
                // Vector3 p1 = transform.position + new Vector3(x * Resolution - halfBoxsize, 2, y * Resolution - halfBoxsize);

                if (!Physics.Raycast(p1, Vector3.down, out var hitInfo, 5))
                    map[x, y] = 0;
                else if (!NavMesh.SamplePosition(hitInfo.point, out var hit, 0.03f, NavMesh.AllAreas))
                    map[x, y] = 0;
                else
                    map[x, y] = -1;
            }
        }
        map.StaticMapForSave = new sbyte[(SizeX + 1) * (SizeY + 1)];
        for (int i = 0; i < SizeX + 1; i++)
            for (int j = 0; j < SizeY + 1; j++)
            {
                map.StaticMapForSave[i * SizeY + i + j] = map[i, j];
            }
        try
        {
            File.Delete(FilePath);
        }
        catch { }
        map.Build();
        ESDFMapData mapdata = new()
        {
            _size_x = map.SizeX,
            _size_y = map.SizeY,
            _staticMap = map.Map,
            _staticObs = map.StaticObs,
            _resolution = map.Resolution
        };
        var json = JsonConvert.SerializeObject(mapdata);
        File.WriteAllText(FilePath, json);

        map_ = map;
    }

    public void Test()
    {
        var json = File.ReadAllText(FilePath);
        mapData = JsonConvert.DeserializeObject<ESDFMapData>(json);
        var a = Sentry.GetComponent<LocalGridMapBuilder>();
        // map_.Update(b.x, b.y, m);
    }
    public System.Numerics.Vector2 XY2Vector2(int x, int y) => new System.Numerics.Vector2(
        x * mapData.Resolution - (mapData.SizeX - 1) * mapData.Resolution / 2.0f,
        y * mapData.Resolution - (mapData.SizeY - 1) * mapData.Resolution / 2.0f);
    public (int x, int y) Vector2ToXY(System.Numerics.Vector2 pos) => new(
        (int)Math.Round((pos.X - ((mapData.SizeX - 1) * mapData.Resolution / 2.0f)) / mapData.Resolution),
        (int)Math.Round((pos.Y - ((mapData.SizeY - 1) * mapData.Resolution / 2.0f)) / mapData.Resolution));


    void OnDrawGizmosSelected()
    {
        if (!Show)
            return;
        if (mapData == null)
            return;
        for (int i = 0; i < mapData.SizeX; i++)
        {
            for (int j = 0; j < mapData.SizeY; j++)
            {
                var p = XY2Vector2(i, j);
                var p1 = new Vector3(p.X, 5, p.Y);
                if (mapData._staticMap[i, j] > 0)
                    Gizmos.color = UnityEngine.Color.blue + new UnityEngine.Color(0, 1, -1, 0) * mapData._staticMap[i, j] / 100.0f;
                else
                    Gizmos.color = UnityEngine.Color.blue + new UnityEngine.Color(1, 0, -1, 0) * -mapData._staticMap[i, j] / 100.0f;
                Gizmos.DrawCube(p1, mapData.Resolution * Vector3.one);
            }
        }
    }
}