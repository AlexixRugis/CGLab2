using Dear_ImGui_Sample;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class ImGuiEditor
{
    private readonly Game _game;

    private ImGuiController _controller;
    private bool _showImGui = false;
    private bool _vsync = false;

    private WeakReference<Entity> _selected = null;

    public ImGuiEditor(Game game)
    {
        _game = game;
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

        World world = _game.World;
        
        ImGui.Begin("Controls");
        ImGui.Text($"FPS: {(int)(1.0f / deltaTime)}");
        ImGui.Text($"Draw calls: {Game.Instance.Renderer.DrawCalls}");
        ImGui.Checkbox("VSync", ref _vsync);
        if (_vsync && _game.VSync != VSyncMode.On) _game.VSync = VSyncMode.On;
        else if (!_vsync && _game.VSync != VSyncMode.Off) _game.VSync = VSyncMode.Off;

        ImGui.Separator();

        Color4 ambC = world.AmbientColor;
        System.Numerics.Vector3 col = new System.Numerics.Vector3(ambC.R, ambC.G, ambC.B);
        if (ImGui.ColorPicker3("Ambient", ref col))
        {
            world.AmbientColor = new Color4(col.X, col.Y, col.Z, 255);
        }

        ImGui.End();

        ImGui.Begin("Objects");
        foreach (var e in world.RootEntites)
        {
            DrawEntityTree(e);
        }
        ImGui.End();

        if (_selected != null && _selected.TryGetTarget(out Entity selected))
        {

            ImGui.Begin("Details");

            if (ImGui.SmallButton("X")) _selected = null;
            ImGui.Text($"Id: {selected.Id}");
            ImGui.Text($"Name: {selected.Name}");
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
                DrawComponent(c);
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

    private void DrawComponent(Component c)
    {
        Type type = c.GetType();

        if (ImGui.CollapsingHeader(type.Name))
        {
            var fields = FieldCollector.GetEditorFields(type);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(float))
                {
                    float val = (float)field.GetValue(c);
                    if (ImGui.InputFloat(field.Name, ref val))
                    {
                        field.SetValue(c, val);
                    }
                }
                else if (field.FieldType == typeof(double))
                {
                    double val = (double)field.GetValue(c);
                    if (ImGui.InputDouble(field.Name, ref val))
                    {
                        field.SetValue(c, val);
                    }
                }
                else if (field.FieldType == typeof(int))
                {
                    int val = (int)field.GetValue(c);
                    if (ImGui.InputInt(field.Name, ref val))
                    {
                        field.SetValue(c, val);
                    }
                }
                else if (field.FieldType == typeof(bool))
                {
                    bool val = (bool)field.GetValue(c);
                    if (ImGui.Checkbox(field.Name, ref val))
                    {
                        field.SetValue(c, val);
                    }
                }
                else if (field.FieldType == typeof(OpenTK.Mathematics.Vector3))
                {
                    System.Numerics.Vector3 val = (System.Numerics.Vector3)(OpenTK.Mathematics.Vector3)field.GetValue(c);
                    if (ImGui.InputFloat3(field.Name, ref val))
                    {
                        field.SetValue(c, (OpenTK.Mathematics.Vector3)val);
                    }
                }
                else if (field.FieldType == typeof(OpenTK.Mathematics.Color4))
                {
                    Color4 col = (OpenTK.Mathematics.Color4)field.GetValue(c);
                    System.Numerics.Vector4 val = new System.Numerics.Vector4(col.R, col.G, col.B, col.A);
                    if (ImGui.ColorPicker4(field.Name, ref val))
                    {
                        field.SetValue(c, new Color4(val.X, val.Y, val.Z, val.W));
                    }
                }
            }
        }
    }
}
