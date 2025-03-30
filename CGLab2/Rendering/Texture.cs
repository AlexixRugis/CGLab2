using System.Drawing.Imaging;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

public class Texture : IDisposable
{
    public Texture(Bitmap bitmap, bool generateMipmaps)
    {
        GenerateMipmaps = generateMipmaps;
        Bitmap = bitmap;

        if (Bitmap == null) throw new NullReferenceException(nameof(Bitmap));

        TextureID = GenerateTexture(Bitmap, GenerateMipmaps);
    }

    public bool GenerateMipmaps;
    public int TextureID { get; private set; }
    public Bitmap Bitmap { get; private set; }
    public bool IsValid { get; private set; }

    private static int GenerateTexture(Bitmap image, bool generateMipmaps)
    {
        int textureID = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, textureID);

        BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        image.UnlockBits(data);

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