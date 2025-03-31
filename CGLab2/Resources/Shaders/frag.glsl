#version 410 core

uniform sampler2D texture0;

out vec4 FragColor;
in vec2 texCoord;

void main()
{
    vec4 color = texture(texture0, texCoord);
    if (color.a < 1.0f)
    {
        discard;
    }

    FragColor = color;
}