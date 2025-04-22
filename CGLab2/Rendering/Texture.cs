using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System.IO;

public class Texture : IDisposable
{
    public Texture(int width, int height, bool generateMipmaps)
    {
        GenerateMipmaps = generateMipmaps;

        TextureID = CreateBlank(width, height, GenerateMipmaps);
    }

    public Texture(string path, bool generateMipmaps)
    {
        GenerateMipmaps = generateMipmaps;

        TextureID = GenerateTexture(path, GenerateMipmaps);
    }

    public bool GenerateMipmaps;
    public int TextureID { get; private set; }

    private static int GenerateTexture(string path, bool generateMipmaps)
    {
        int textureID = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, textureID);

        FileStream stream = File.OpenRead(path);
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        stream.Close();

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        if (generateMipmaps)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        return textureID;
    }

    private int CreateBlank(int width, int height, bool generateMipmaps)
    {
        int textureID = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, textureID);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        if (generateMipmaps)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        return textureID;
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, TextureID);
    }

    public void Unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose()
    {
        GL.DeleteTexture(TextureID);
    }
}