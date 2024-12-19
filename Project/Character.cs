using System;
using Godot;

public partial class Character : Node2D
{	
    public static float ViewportScale() => 0.07f;

    public static Vector2 ScreenToChar(Vector2 InScreen)
    {
        return InScreen / ViewportScale();
    }
    
    public static Vector2 CharToScreen(Vector2 InPos)
    {
        return InPos * ViewportScale();
    }

    public static Vector2 MousePos()
    {
        return Vector2.Zero;
    }

    public static Vector2 WindowPos()
    {
        return Vector2.Zero;
    }
}
