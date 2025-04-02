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
    private bool _vsync = false;

    private WeakReference<Entity> _selected = null;

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

        ImGui.Begin("Objects");
        foreach (var e in _world.Entities)
        {
            if (e.Transform.Parent == null)
                DrawEntityTree(e);

        }
        ImGui.End();

        if (_selected != null && _selected.TryGetTarget(out Entity selected))
        {

            ImGui.Begin("Details");

            if (ImGui.SmallButton("X")) _selected = null;
            ImGui.Text($"Id: {selected.Id}");
            ImGui.Text($"Id: {selected.Name}");
            ImGui.Separator();
            ImGui.Text("Transform");

            System.Numerics.Vector3 pos = (System.Numerics.Vector3)selected.Transform.LocalPosition;
            if (ImGui.InputFloat3("Position", ref pos))
                selected.Transform.LocalPosition = (Vector3)pos;

            System.Numerics.Vector3 rot = (System.Numerics.Vector3)selected.Transform.LocalRotation.ToEulerAngles();
            if (ImGui.InputFloat3("Rotation", ref rot))
                selected.Transform.LocalRotation = Quaternion.FromEulerAngles((Vector3)rot);

            System.Numerics.Vector3 scale = (System.Numerics.Vector3)selected.Transform.LocalScale;
            if (ImGui.InputFloat3("Scale", ref scale))
                selected.Transform.LocalScale = (Vector3)scale;

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

    public void PressChar(char unicode)
    {
        _controller.PressChar(unicode);
    }

    public void MouseScroll(Vector2 offset)
    {
        _controller.MouseScroll(offset);
    }

    private void DrawEntityTree(Entity e)
    {
        bool opened;
        if (e.Transform.Children.Count == 0)
            opened = ImGui.TreeNodeEx($"{e.Id} {e.Name}", ImGuiTreeNodeFlags.Leaf);
        else
            opened = ImGui.TreeNode($"{e.Id} {e.Name}");

        ImGui.SameLine();
        if (ImGui.Button($"Details {e.Id}"))
        {
            _selected = new WeakReference<Entity>(e);
        }

        if (opened)
        {
            foreach (var c in e.Transform.Children)
            {
                DrawEntityTree(c.Entity);
            }
            ImGui.TreePop();
        }

    }
}
