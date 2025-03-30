public static class Screen
{
    /// <summary>
    /// Height of the screen
    /// </summary>
    public static int Height => Game.Instance.ClientRectangle.Size.Y;
    /// <summary>
    /// Width of the screen
    /// </summary>
    public static int Width => Game.Instance.ClientRectangle.Size.X;
    /// <summary>
    /// Aspect of the screen (Width / Height)
    /// </summary>
    public static float Aspect => (float)Width / Height;
}