﻿using OpenTK.Mathematics;

public struct Transform
{
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;

    public Matrix4 LocalToWorld => Matrix4.CreateScale(Scale)
            * Matrix4.CreateFromQuaternion(Rotation)
            * Matrix4.CreateTranslation(Position);

    public Matrix4 WorldToLocal => Matrix4.Invert(LocalToWorld);

    public Transform() { }
}