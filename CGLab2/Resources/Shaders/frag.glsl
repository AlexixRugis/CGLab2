#version 410 core

uniform sampler2D texture0;
uniform vec4 Color;

out vec4 FragColor;

in vec2 texCoord;

void main()
{
    vec4 c = Color * texture(texture0, texCoord);
    if (c.a < 1.0f)
    {
        discard;
    }

    FragColor = c;
}