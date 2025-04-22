using OpenTK.Graphics.OpenGL4;

public class Framebuffer : IDisposable
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public int FboID { get; private set; }
    public int DepthbufferID { get; private set; }
    public Texture ColorTexture { get; private set; }

    public Framebuffer(int width, int height)
    {
        Width = width;
        Height = height;

        FboID = GL.GenFramebuffer();
        Bind();

        ColorTexture = new Texture(width, height, false);
        ColorTexture.Bind();

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D, ColorTexture.TextureID, 0);

        DepthbufferID = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, DepthbufferID);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0,
            PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
            TextureTarget.Texture2D, DepthbufferID, 0);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
            throw new Exception("Framebuffer is not complete");
        }

        GL.BindTexture(TextureTarget.Texture2D, 0);
        Unbind();
    }

    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboID);
        GL.Viewport(0, 0, Width, Height);
    }

    public void Unbind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, Game.Instance.ClientSize.X, Game.Instance.ClientSize.Y);
    }

    public void Dispose()
    {
        GL.DeleteFramebuffer(FboID);
        ColorTexture.Dispose();
        GL.DeleteTexture(DepthbufferID);
    }
}