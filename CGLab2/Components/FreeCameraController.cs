using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class FreeCameraController : Component, IUpdatable
{
    [EditorField] public float MovementSpeed = 1.0f;
    [EditorField] public float Sensitivity = 2000f;
    [EditorField] public float Acceleration = 8.0f;

    private bool _firstMove = true;
    private Vector2 _lastPos;

    private Vector3 _currentVelocity = Vector3.Zero;
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
        if (_keyboardState.IsKeyPressed(Keys.Escape))
        {
            if (Game.Instance.CursorState == OpenTK.Windowing.Common.CursorState.Normal)
                Game.Instance.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;
            else
                Game.Instance.CursorState = OpenTK.Windowing.Common.CursorState.Normal;

            _firstMove = true;
        }


        if (!_mouseState.IsButtonDown(MouseButton.Right))
            return;

        Vector3 forward = Entity.Transform.Forward;
        Vector3 right = Entity.Transform.Right;
        Vector3 up = Entity.Transform.Up;


        Vector3 targetVelocity = Vector3.Zero;
        if (_keyboardState.IsKeyDown(Keys.A))
        { 
            targetVelocity -= right;
        }

        if (_keyboardState.IsKeyDown(Keys.D))
        {
            targetVelocity += right;
        }

        if (_keyboardState.IsKeyDown(Keys.W))
        {
            targetVelocity -= forward;
        }

        if (_keyboardState.IsKeyDown(Keys.S))
        {
            targetVelocity += forward;
        }

        if (_keyboardState.IsKeyDown(Keys.Space))
        {
            targetVelocity += up;
        }

        if (_keyboardState.IsKeyDown(Keys.LeftShift))
        {
            targetVelocity -= up;
        }

        if (targetVelocity.LengthSquared > 0.1f)
        {
            targetVelocity.Normalize();
        }

        float speed = MovementSpeed;
        if (_keyboardState.IsKeyDown(Keys.LeftControl)) speed *= 2.0f;

        _currentVelocity = Vector3Extensions.MoveTowards(_currentVelocity, targetVelocity * speed, Acceleration * deltaTime);
        Transform.LocalPosition += _currentVelocity * deltaTime;

        if (_firstMove)
        {
            _firstMove = false;
            _lastPos = new Vector2(_mouseState.X, _mouseState.Y);
        }
        else
        {
            float height = Screen.Height;

            if (height == 0) return;

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