#version 430 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUv;

out vec3 glPosition;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    glPosition = aPosition;
}