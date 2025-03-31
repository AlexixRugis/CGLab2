using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class FreeCameraController : Component, IUpdatable
{
    public float MovementSpeed = 1.0f;
    public float Sensitivity = 0.4f;

    private bool _firstMove = true;
    private Vector2 _lastPos;

    private KeyboardState _keyboardState;
    private MouseState _mouseState;

    public override void OnStart()
    {
        _keyboardState = Game.Instance.KeyboardState;
        _mouseState = Game.Instance.MouseState;
    }

    public void Update(float deltaTime)
    {
        Vector3 forward = Entity.Transform.Forward;
        Vector3 right = Entity.Transform.Right;
        Vector3 up = Entity.Transform.Up;

        if (_keyboardState.IsKeyPressed(Keys.Escape))
        {
            if (Game.Instance.CursorState == OpenTK.Windowing.Common.CursorState.Normal)
                Game.Instance.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;
            else
                Game.Instance.CursorState = OpenTK.Windowing.Common.CursorState.Normal;
        }

        if (_keyboardState.IsKeyDown(Keys.A))
        {
            Entity.Transform.Position -= right * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.D))
        {
            Entity.Transform.Position += right * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.W))
        {
            Entity.Transform.Position -= forward * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.S))
        {
            Entity.Transform.Position += forward * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.Space))
        {
            Entity.Transform.Position += up * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.LeftShift))
        {
            Entity.Transform.Position -= up * deltaTime * MovementSpeed;
        }

        if (_firstMove)
        {
            _firstMove = false;
            _lastPos = new Vector2(_mouseState.X, _mouseState.Y);
        }
        else
        {
            var deltaX = _mouseState.X - _lastPos.X;
            var deltaY = _mouseState.Y - _lastPos.Y;

            _lastPos = new Vector2(_mouseState.X, _mouseState.Y);

            Entity.Transform.Rotation *= Quaternion.FromEulerAngles(
                -deltaY * Sensitivity * deltaTime,
                -deltaX * Sensitivity * deltaTime,
                0.0f);
        }

    }
}