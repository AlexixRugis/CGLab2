#version 410 core

layout(location = 0) in vec3 aPosition;

out vec3 texCoord;

uniform mat4 _Projection;
uniform mat4 _View;

void main()
{
    texCoord = aPosition;
    vec4 pos = _Projection * _View * vec4(aPosition, 1.0);
    gl_Position = pos.xyww;
}