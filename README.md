# Quat32
Compressing Quaternion to 32-bit in C# with Unity

Based on [muQuat32](https://github.com/unity3d-jp/MeshSync/blob/9d29838eb21249b3057613e0a0a23042a4bfd4ab/Plugin~/Src/MeshUtils/muQuat32.h) in [unity3d-jp/MeshSync](https://github.com/unity3d-jp/MeshSync), I attempted to write code using C#'s BitVector32 to compress Quaternion to 32 bits.

## Usage
```
Vector3 axis;
float angle;
Quaternion quaternion = Quaternion.AngleAxis(angle, axis);

// Compress (Quaternion -> Quat32 -> int)
Quat32 quat32 = quaternion;
int compressed = quat32;

// Decompress (int -> Quat32 -> Quaternion)
quat32 = compressed;
Quaternion decompressed = quat32;
```
