using Dear_ImGui_Sample;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class ImGuiEditor
{
    private readonly Game _game;
    private World _world;

    private ImGuiController _controller;
    private bool _showImGui = false;
    private ulong _selectedEntity = 0;
    private bool _vsync = false;

    public ImGuiEditor(Game game)
    {
        _game = game;
        _world = game.World;
        _controller = new ImGuiController(game.ClientSize.X, game.ClientSize.Y);
    }

    public void Update(float deltaTime)
    {
        _controller.Update(_game, deltaTime);
        if (_game.KeyboardState.IsKeyPressed(Keys.F5))
        {
            _showImGui = !_showImGui;
        }
    }

    public void WindowResized(int x, int y)
    {
        _controller.WindowResized(x, y);
    }

    public void Render(float deltaTime)
    {
        if (!_showImGui) return;
        
        ImGui.Begin("Controls");
        ImGui.Text($"FPS: {(int)(1.0f / deltaTime)}");
        ImGui.Checkbox("VSync", ref _vsync);
        if (_vsync && _game.VSync != VSyncMode.On) _game.VSync = VSyncMode.On;
        else if (!_vsync && _game.VSync != VSyncMode.Off) _game.VSync = VSyncMode.Off;

        if (ImGui.Button("fff"))
        {
            Console.WriteLine("fff");
        }

        ImGui.End();

        Entity? selected = null;
        ImGui.Begin("Objects");
        foreach (var e in _world.Entities)
        {
            if (e.Id == _selectedEntity)
            {
                ImGui.TextColored(new System.Numerics.Vector4(0.0f, 1.0f, 1.0f, 1.0f), $"{e.Id} {e.Name}");
                selected = e;
            }
            else
            {
                if (ImGui.Button($"{e.Id} {e.Name}"))
                {
                    _selectedEntity = e.Id;
                }
            }
        }
        ImGui.End();

        if (selected != null)
        {

            ImGui.Begin("Details");

            if (ImGui.SmallButton("X")) _selectedEntity = 0;
            ImGui.Text($"Id: {selected.Id}");
            ImGui.Text($"Id: {selected.Name}");
            ImGui.Separator();
            ImGui.Text("Transform");

            System.Numerics.Vector3 pos = (System.Numerics.Vector3)selected.Transform.Position;
            if (ImGui.InputFloat3("Position", ref pos))
                selected.Transform.Position = (Vector3)pos;

            System.Numerics.Vector3 rot = (System.Numerics.Vector3)selected.Transform.Rotation.ToEulerAngles();
            if (ImGui.InputFloat3("Rotation", ref rot))
                selected.Transform.Rotation = Quaternion.FromEulerAngles((Vector3)rot);

            System.Numerics.Vector3 scale = (System.Numerics.Vector3)selected.Transform.Scale;
            if (ImGui.InputFloat3("Scale", ref scale))
                selected.Transform.Scale = (Vector3)scale;

            ImGui.Text("COMPONENTS");
            ImGui.Separator();

            foreach (var c in selected.Components)
            {
                ImGui.Text(c.GetType().Name);
            }

            ImGui.End();
        }

        ImGui.ShowDemoWindow();

        _controller.Render();

        ImGuiController.CheckGLError("End of frame");
    }

    internal void PressChar(char unicode)
    {
        _controller.PressChar(unicode);
    }

    internal void MouseScroll(Vector2 offset)
    {
        _controller.MouseScroll(offset);
    }
}
