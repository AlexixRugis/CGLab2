#version 410 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUv;

uniform mat4 _Model;
uniform mat4 _Projection;
uniform mat4 _View;

out vec2 texCoord;

void main()
{
    texCoord = aUv;
    gl_Position = _Projection * _View * _Model *  vec4(aPosition, 1.0);
}