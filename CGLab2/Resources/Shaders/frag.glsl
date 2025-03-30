#version 410 core

uniform sampler2D texture0;

out vec4 FragColor;
in vec2 texCoord;

void main()
{
    FragColor = texture(texture0, texCoord);
}