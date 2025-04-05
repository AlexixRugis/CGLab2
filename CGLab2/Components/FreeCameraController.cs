using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class FreeCameraController : Component, IUpdatable
{
    public float MovementSpeed = 1.0f;
    public float Sensitivity = 2000f;

    private bool _firstMove = true;
    private Vector2 _lastPos;

    private float _pitch = 0.0f;
    private float _yaw = 0.0f;

    private KeyboardState _keyboardState;
    private MouseState _mouseState;

    public override Component Clone()
    {
        return new FreeCameraController()
        {
            MovementSpeed = MovementSpeed,
            Sensitivity = Sensitivity,
        };
    }

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

            _firstMove = true;
        }

        if (_keyboardState.IsKeyDown(Keys.A))
        {
            Entity.Transform.LocalPosition -= right * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.D))
        {
            Entity.Transform.LocalPosition += right * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.W))
        {
            Entity.Transform.LocalPosition -= forward * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.S))
        {
            Entity.Transform.LocalPosition += forward * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.Space))
        {
            Entity.Transform.LocalPosition += up * deltaTime * MovementSpeed;
        }

        if (_keyboardState.IsKeyDown(Keys.LeftShift))
        {
            Entity.Transform.LocalPosition -= up * deltaTime * MovementSpeed;
        }

        if (_firstMove)
        {
            _firstMove = false;
            _lastPos = new Vector2(_mouseState.X, _mouseState.Y);
        }
        else
        {
            float height = Screen.Height;

            var deltaX = (_mouseState.X - _lastPos.X) / height;
            var deltaY = (_mouseState.Y - _lastPos.Y) / height;

            _lastPos = new Vector2(_mouseState.X, _mouseState.Y);

            _pitch -= deltaY * Sensitivity * deltaTime;
            if (_pitch > 89.0f)
            {
                _pitch = 89.0f;
            }
            else if (_pitch < -89.0f)
            {
                _pitch = -89.0f;
            }

            _yaw -= deltaX * Sensitivity * deltaTime;

            if (_yaw > 360.0f || _yaw < -360.0f) _yaw %= 360.0f;

            Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegToRad * _yaw);
            q *= Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegToRad * _pitch);

            Entity.Transform.LocalRotation = q;

            if (float.IsNaN(_yaw)) _yaw = 0;
            if (float.IsNaN(_pitch)) _pitch = 0;
        }

    }
}