#version 430 core

struct Ray
{
    vec3 origin;
    vec3 direction;
};

struct Material
{
    vec4 color;
};

struct Hit
{
    float d;
    vec3 p;
    vec3 n;
    Material mat;
};


struct Sphere
{
    vec3 position;
    float radius;
    Material mat;
};

Ray createRay(vec3 origin, vec3 direction)
{
    Ray r;
    r.origin = origin;
    r.direction = direction;
    return r;
}

layout(std430, binding = 0) buffer SphereBuffer
{
    Sphere _Spheres[];
};

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

Hit hitSphere(Ray ray, vec3 center, float radius)
{
    Hit hit;
    hit.d = -1.0;

    vec3 oc = ray.origin - center;
    float a = dot(ray.direction, ray.direction);
    float b = 2.0 * dot(oc, ray.direction);
    float c = dot(oc, oc) - radius * radius;
    float discriminant = b * b - 4.0 * a * c;

    if (discriminant >= 0.0)
    {
        float dst = (-b - sqrt(discriminant)) / (2.0 * a);
        
        if (dst >= 0.0)
        {
            hit.d = dst;
            hit.p = ray.origin + ray.direction * dst;
            hit.n = normalize(hit.p - center);
        }
    }

    return hit;
}

Hit calculateCollision(Ray ray)
{
    Hit closest;
    closest.d = 1e10;
    
    for (int i = 0; i < 3; i++)
    {
        Sphere s = _Spheres[i];
        Hit h = hitSphere(ray, s.position, s.radius);

        if (h.d >= 0.0 && h.d < closest.d)
        {
            closest = h;
            closest.mat = s.mat;
        }
    }

    return closest;
}

out vec4 FragColor;
in vec3 glPosition;

void main()
{
    vec2 uv = glPosition.xy;
    Ray ray = createCameraRay(uv);
    Hit hit = calculateCollision(ray);

    if (hit.d < 1e9)
    {
        FragColor = hit.mat.color;
    }
    else
    {
        FragColor = vec4(ray.direction * 0.5 + 0.5, 1.0);
    }
}