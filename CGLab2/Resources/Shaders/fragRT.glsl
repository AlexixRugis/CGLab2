#version 410 core

struct Ray
{
    vec3 origin;
    vec3 direction;
};

Ray createRay(vec3 origin, vec3 direction)
{
    Ray r;
    r.origin = origin;
    r.direction = direction;
    return r;
}

uniform mat4 _CameraToWorld;
uniform mat4 _CameraInverseProjection;

Ray createCameraRay(vec2 uv)
{
    vec3 origin = (_CameraToWorld * vec4(0.0, 0.0, 0.0, 1.0)).xyz;

    vec3 direction = (_CameraInverseProjection * vec4(uv, 0.0, 1.0)).xyz;
    direction = (_CameraToWorld * vec4(direction, 0.0)).xyz;
    direction = normalize(direction);

    return createRay(origin, direction);
}


out vec4 FragColor;
in vec3 glPosition;

void main()
{
    vec2 uv = glPosition.xy;
    Ray ray = createCameraRay(uv);

    FragColor = vec4(ray.direction * 0.5 + 0.5, 1.0);
}