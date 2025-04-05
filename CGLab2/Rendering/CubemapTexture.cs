using System.Drawing.Imaging;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

public class CubemapTexture : IDisposable
{
    public CubemapTexture(Bitmap[] bitmaps, bool generateMipmaps)
    {
        if (bitmaps == null) throw new NullReferenceException(nameof(Bitmap));
        if (bitmaps.Length != 6) throw new ArgumentException("There must be 6 textures.");

        GenerateMipmaps = generateMipmaps;
        Bitmaps = bitmaps;

        TextureID = GenerateTexture(Bitmaps, GenerateMipmaps);
    }

    public bool GenerateMipmaps;
    public int TextureID { get; private set; }
    public Bitmap[] Bitmaps { get; private set; }
    public bool IsValid { get; private set; }

    private static int GenerateTexture(Bitmap[] images, bool generateMipmaps)
    {
        int textureID = GL.GenTexture();

        GL.BindTexture(TextureTarget.TextureCubeMap, textureID);

        for (int i = 0; i < 6; i++)
        {
            BitmapData data = images[i].LockBits(new Rectangle(0, 0, images[i].Width, images[i].Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);


            images[i].UnlockBits(data);
        }

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        if (generateMipmaps)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        }

        return textureID;
    }

    public void Bind()
    {
        GL.BindTexture(TextureTarget.TextureCubeMap, TextureID);
    }

    public void Unbind()
    {
        GL.BindTexture(TextureTarget.TextureCubeMap, 0);
    }

    public void Dispose()
    {
        GL.DeleteTexture(TextureID);
    }
}