
public class AssetLoader
{
    private Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();
    private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
    private Dictionary<string, Mesh> _meshes = new Dictionary<string, Mesh>();

    public void LoadShader(string name, string vertexPath, string fragmentPath)
    {
        if (_shaders.ContainsKey(name)) throw new ArgumentException($"Shader {name} already exists.");

        Shader shader = new Shader(
            File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), vertexPath)),
            File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fragmentPath))
            );

        _shaders.Add(name, shader);
    }

    public Shader GetShader(string name)
    {
        if (_shaders.TryGetValue (name, out Shader? shader)) return shader;
        throw new InvalidOperationException();
    }

    public void LoadTexture(string name, string path, bool generateMipmaps)
    {
        if (_textures.ContainsKey(name)) throw new ArgumentException($"Texture {name} already exists.");

        Texture tex = new Texture(new System.Drawing.Bitmap(path), generateMipmaps);

        _textures.Add(name, tex);
    }

    public Texture GetTexture(string name)
    {
        if (_textures.TryGetValue(name, out Texture? tex)) return tex;
        throw new InvalidOperationException();
    }

    public void LoadMesh(string name, Vertex[] vertices, uint[] indices, Mesh.SubMeshInfo[] subMeshes)
    {
        if (_meshes.ContainsKey(name)) throw new ArgumentException($"Mesh {name} already exists.");

        Mesh mesh = new Mesh(vertices, indices, subMeshes);
        _meshes.Add(name, mesh);
    }

    public void LoadMesh(string name, string path)
    {
        if (_meshes.ContainsKey(name)) throw new ArgumentException($"Mesh {name} already exists.");

        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
        //AssimpLoader loader = new AssimpLoader();

        //Mesh mesh = loader.Load(fullPath, "");
        //_meshes.Add(name, mesh);
    }

    public Mesh GetMesh(string name)
    {
        if (_meshes.TryGetValue(name, out Mesh? mesh)) return mesh;
        throw new InvalidOperationException();
    }

    public void UnloadAll()
    {
        foreach (var kv in _shaders)
        {
            kv.Value.Dispose();
        }
        _shaders.Clear();

        foreach (var kv in _textures)
        {
            kv.Value.Dispose();
        }
        _shaders.Clear();
    }
}