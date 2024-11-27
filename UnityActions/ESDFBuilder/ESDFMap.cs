
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

[Serializable]
public class ESDFMap
{
    public float MaxLength = 1f;
    public int SizeX;
    public int SizeY;
    public float Resolution;
    public sbyte[] StaticMapForSave;
    [NonSerialized] public sbyte[,] StaticMap;
    [NonSerialized] public sbyte[,] Map;
    [NonSerialized] bool[,] Colord;
    [NonSerialized] public int[,,] StaticObs;
    [NonSerialized] int[,,] Obs;
    public sbyte[,] OutputMap => Map;

    public sbyte this[int x, int y]
    {
        get => Map == null ? (sbyte)-1 : Map[x, y];
        set
        {
            if (Map == null)
            {
                Map = new sbyte[SizeX, SizeY];
                StaticObs = new int[SizeX, SizeY, 2];
                Colord = new bool[SizeX, SizeY];
            }
            Map[x, y] = value;
        }
    }

    public Vector2 XY2Vector2(int x, int y) => new Vector2(-y * Resolution + (SizeY - 1) * Resolution / 2.0f, -x * Resolution + (SizeX - 1) * Resolution / 2.0f);
    public (int x, int y) Vector22XY(Vector2 pos)
    {
        return new((int)Math.Round(-(pos.Y - ((SizeX - 1) * Resolution / 2.0f)) / Resolution),
         (int)Math.Round(-(pos.X - ((SizeY - 1) * Resolution / 2.0f)) / Resolution));
    }

    public void Build()
    {
        Queue<(int x, int y)> openList = new Queue<(int x, int y)>();
        if (StaticMap == null)
        {
            StaticMap = new sbyte[SizeX, SizeY];
            Map = new sbyte[SizeX, SizeY];
            StaticObs = new int[SizeX, SizeY, 2];
            Obs = new int[SizeX, SizeY, 2];
            Colord = new bool[SizeX, SizeY];
        }
        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                StaticMap[i, j] = StaticMapForSave[i * SizeY + j];
            }
        Buffer.BlockCopy(StaticMap, 0, Map, 0, Map.Length * sizeof(sbyte));
        for (int i = 1; i < SizeX - 1; i++)
            for (int j = 1; j < SizeY - 1; j++)
            {
                var t = Map[i + 1, j + 1] + Map[i + 1, j] + Map[i + 1, j - 1] +
                Map[i, j + 1] + Map[i, j - 1] +
                Map[i - 1, j + 1] + Map[i - 1, j] + Map[i - 1, j - 1];
                if (Map[i, j] == 0 && t != 0)
                {
                    StaticObs[i, j, 0] = i;
                    StaticObs[i, j, 1] = j;
                    Colord[i, j] = true;
                    openList.Enqueue(new(i, j));
                }
                else
                {
                    StaticObs[i, j, 0] = -1;
                    StaticObs[i, j, 1] = -1;
                    if (Map[i, j] != 0)
                        StaticMap[i, j] = 100;
                    else
                        StaticMap[i, j] = -100;
                }
            }

        while (openList.Count > 0)
        {
            var c = openList.Dequeue();
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    var ci = c.x + i;
                    var cj = c.y + j;
                    if (i == 0 && j == 0)
                        continue;
                    if (ci < 0 || ci >= SizeX)
                        continue;
                    if (cj < 0 || cj >= SizeY)
                        continue;

                    // for calculate
                    (int x, int y) cObs = (StaticObs[ci, cj, 0], StaticObs[ci, cj, 1]);
                    if (cObs.x != -1)
                    {
                        var m = (sbyte)(Math.Clamp(Math.Round(Math.Sqrt(Math.Pow(cObs.x - c.x, 2) + Math.Pow(cObs.y - c.y, 2))) * Resolution / MaxLength, 0, 1) * 100);
                        if (Math.Abs(StaticMap[c.x, c.y]) > m)
                        {
                            StaticMap[c.x, c.y] = (sbyte)(m * (StaticMap[c.x, c.y] <= 0 ? -1 : 1));
                            StaticObs[c.x, c.y, 0] = cObs.x;
                            StaticObs[c.x, c.y, 1] = cObs.y;
                        }
                    }
                    // for enqueue
                    if (Colord[ci, cj])
                        continue;

                    Colord[ci, cj] = true;
                    openList.Enqueue((ci, cj));
                }
        }
        Buffer.BlockCopy(StaticMap, 0, Map, 0, Map.Length * sizeof(sbyte));
    }

    public void Update(int offsetX, int offsetY, sbyte[,] dynamicMap)
    {
        Buffer.BlockCopy(StaticMap, 0, Map, 0, Map.Length * sizeof(sbyte));
        Buffer.BlockCopy(StaticObs, 0, Obs, 0, Obs.Length * sizeof(int));
        Colord = new bool[SizeX, SizeY];

        Queue<(int x, int y)> openList = new Queue<(int x, int y)>();


        for (int i = 1, k = dynamicMap.GetLength(0); i <= k; i++)
            for (int j = 1; j <= k; j++)
            {
                var x = -i + offsetX + k / 2;
                var y = -j + offsetY + k / 2;
                if (x < 0 || x >= SizeX)
                    continue;
                if (y < 0 || y >= SizeY)
                    continue;
                if (dynamicMap[k - i, k - j] != 0 && StaticMap[x, y] != 0)
                    continue;
                Map[x, y] = 0;
                Obs[x, y, 0] = x;
                Obs[x, y, 1] = y;
                Colord[x, y] = true;
                openList.Enqueue(new(x, y));
            }

        while (openList.Count > 0)
        {
            var c = openList.Dequeue();
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    var ci = c.x + i;
                    var cj = c.y + j;
                    if (i == 0 && j == 0)
                        continue;
                    if (ci < 0 || ci >= SizeX)
                        continue;
                    if (cj < 0 || cj >= SizeY)
                        continue;

                    // for calculate
                    var cObsx = Obs[ci, cj, 0];
                    var cObsy = Obs[ci, cj, 1];
                    if (cObsx != -1)
                    {
                        var m = (sbyte)(Math.Clamp(Math.Round(Math.Sqrt(Math.Pow(cObsx - c.x, 2) + Math.Pow(cObsy - c.y, 2))) * Resolution / MaxLength, 0, 1) * 100);
                        if (Map[c.x, c.y] > m)
                        {
                            Map[c.x, c.y] = m;
                            Obs[c.x, c.y, 0] = cObsx;
                            Obs[c.x, c.y, 1] = cObsy;
                        }

                    }
                    // for enqueue
                    if (Colord[ci, cj])
                        continue;
                    if (Obs[ci, cj, 0] == ci && Obs[ci, cj, 0] == cj)
                        continue;
                    Colord[ci, cj] = true;
                    openList.Enqueue((ci, cj));
                }
        }
    }

    public void RelocatedToNoneCollision(in Vector2 Point1, ref Vector2 Point2)
    {
        Vector2 formTo = Point2 - Point1;
        for (float i = 0, l = Resolution / formTo.Length(); i < 1; i += l)
        {
            var XY = Vector22XY(formTo * i + Point1);
            if (XY.x < 0 || XY.y < 0 || XY.x >= SizeX || XY.y >= SizeY || this[XY.x, XY.y] != 0)
                continue;
            else
            {
                Relocate(XY, ref Point2);
            }
        }
    }
    private void Relocate((int x, int y) begin, ref Vector2 Point2)
    {
        var tar = Vector22XY(Point2);
        var min = float.MaxValue;

        SortedList<float, (int x, int y)> values = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                if (begin.x + i < 0 || begin.y + j < 0 || begin.x + i >= SizeX || begin.y + j >= SizeY)
                    continue;
                if (this[begin.x + i, begin.y + j] != 0)
                {
                    float len = (float)(Math.Pow(begin.x + i - tar.x, 2) + Math.Pow(begin.y + j - tar.y, 2));
                    if (!values.ContainsKey(len))
                        values.Add(len, (begin.x + i, begin.x + j));
                }
            }
        }

        while (values.Count > 0)
        {
            var head = values.Keys[0];
            if (min <= head)
            {
                Point2 = XY2Vector2(values.Values[0].x, values.Values[0].y);
                return;
            }
            var b = values.Values[0];
            min = head;
            values.RemoveAt(0);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (b.x + i < 0 || b.y + j < 0 || b.x + i >= SizeX || b.y + j >= SizeY)
                        continue;
                    if (this[b.x + i, b.y + j] != 0)
                    {
                        float len = (float)(Math.Pow(b.x + i - tar.x, 2) + Math.Pow(b.y + j - tar.y, 2));
                        if (!values.ContainsKey(len))
                            values.Add(len, (b.x + i, b.x + j));
                    }
                }
            }
        }
    }

    public (float dist, Vector3 grad) CalCostWithGrad(Vector3 Point, float time)
    {
        var P = new Vector2(Point.X, Point.Y);
        var t = 2 * Resolution;
        var p = Vector22XY(P);
        if (p.x < 5 || p.x >= SizeX - 5 || p.y < 5 || p.y >= SizeY - 5)
            return (-100, Resolution * new Vector3(Math.Min(5 - p.x, p.x - SizeX - 5), Math.Min(5 - p.y, p.y - SizeY - 5), 0));
        var p1 = Vector22XY(P + new Vector2(t, 0f));
        var p2 = Vector22XY(P - new Vector2(t, 0f));
        var p3 = Vector22XY(P + new Vector2(t, 0.1f));
        var p4 = Vector22XY(P - new Vector2(t, 0.1f));
        var diff = new Vector2(
            (this[p2.x, p2.y] - this[p1.x, p1.y]) * Resolution,
            (this[p4.x, p4.y] - this[p3.x, p3.y]) * Resolution
        );

        return (this[p.x, p.y], new(diff, 0));
    }
}