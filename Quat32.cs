using System;
using System.Collections.Specialized;
using UnityEngine;

struct Quat32
{
    private const float SR2 = 1.41421356237f;
    private const float RSR2 = 1.0f / 1.41421356237f;
    private const float C = (float)0x3ff;
    private const float R = 1.0f / (float)0x3ff;

    private static UInt32 Pack(float a)
    {
        return (UInt32)((a * SR2 + 1.0f) * 0.5f * C);
    }

    private static float Unpack(UInt32 a)
    {
        return ((a * R) * 2.0f - 1.0f) * RSR2;
    }

    private static float Square(float a)
    {
        return a * a;
    }

    private static int DropMax(float a, float b, float c, float d)
    {
        if (a > b && a > c && a > d) return 0;
        if (b > c && b > d) return 1;
        if (c > d) return 2;
        return 3;
    }

    private static readonly BitVector32.Section X0Section = BitVector32.CreateSection(0x3FF);
    private static readonly BitVector32.Section X1Section = BitVector32.CreateSection(0x3FF, X0Section);
    private static readonly BitVector32.Section X2Section = BitVector32.CreateSection(0x3FF, X1Section);
    private static readonly BitVector32.Section DropSection = BitVector32.CreateSection(0x3, X2Section);

    public BitVector32 Value;

    public static implicit operator Quat32(Quaternion q)
    {
        q.Normalize();

        var q32 = new Quat32();

        float a0, a1, a2;
        q32.Value[DropSection] = DropMax(Square(q.x), Square(q.y), Square(q.z), Square(q.w));
        if (q32.Value[DropSection] == 0)
        {
            float s = Mathf.Sign(q.x);
            a0 = q.y * s;
            a1 = q.z * s;
            a2 = q.w * s;
        }
        else if (q32.Value[DropSection] == 1)
        {
            float s = Mathf.Sign(q.y);
            a0 = q.x * s;
            a1 = q.z * s;
            a2 = q.w * s;
        }
        else if (q32.Value[DropSection] == 2)
        {
            float s = Mathf.Sign(q.z);
            a0 = q.x * s;
            a1 = q.y * s;
            a2 = q.w * s;
        }
        else
        {
            float s = Mathf.Sign(q.w);
            a0 = q.x * s;
            a1 = q.y * s;
            a2 = q.z * s;
        }

        q32.Value[X0Section] = (int)Pack(a0);
        q32.Value[X1Section] = (int)Pack(a1);
        q32.Value[X2Section] = (int)Pack(a2);

        return q32;
    }

    public static implicit operator Quat32(int q)
    {
        var q32 = new Quat32();
        q32.Value = new BitVector32(q);
        return q32;
    }

    public static implicit operator Quaternion(Quat32 q)
    {
        float a0 = Unpack((uint)q.Value[X0Section]);
        float a1 = Unpack((uint)q.Value[X1Section]);
        float a2 = Unpack((uint)q.Value[X2Section]);
        float iss = Mathf.Sqrt(1.0f - (Square(a0) + Square(a1) + Square(a2)));

        switch (q.Value[DropSection])
        {
            case 0: return new Quaternion(iss, a0, a1, a2);
            case 1: return new Quaternion(a0, iss, a1, a2);
            case 2: return new Quaternion(a0, a1, iss, a2);
            default: return new Quaternion(a0, a1, a2, iss);
        }
    }

    public static implicit operator int(Quat32 q)
    {
        return q.Value.Data;
    }
}
