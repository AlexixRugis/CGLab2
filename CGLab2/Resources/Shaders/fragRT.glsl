#version 430 core

struct Vertex
{
    vec3 position;
};

struct Ray
{
    vec3 origin;
    vec3 direction;
};

struct Material
{
    vec3 color;
    vec3 emissionColor;
    float emissionStrength;
    float smoothness;
    float metallic;
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

struct MeshInfo
{
    int startIndex;
    int indexCount;

    mat4 transform;

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

layout(std430, binding = 1) buffer VertexBuffer
{
    Vertex _Vertices[];
};

layout(std430, binding = 2) buffer IndexBuffer
{
    uint _Indices[];
};

layout(std430, binding = 3) buffer MeshBuffer
{
    MeshInfo _Meshes[];
};

uniform sampler2D prevFrame;
uniform samplerCube _Skybox;
uniform mat4 _CameraToWorld;
uniform mat4 _CameraInverseProjection;
uniform vec2 _ScreenSize;
uniform uint _Frame;
uniform int _SpheresCount;
uniform int _MeshesCount;

uint wang_hash(inout uint seed)
{
    seed = uint(seed ^ uint(61)) ^ uint(seed >> uint(16));
    seed *= uint(9);
    seed = seed ^ (seed >> 4);
    seed *= uint(0x27d4eb2d);
    seed = seed ^ (seed >> 15);
    return seed;
}

float randomFloat(inout uint state)
{
    return float(wang_hash(state)) / 4294967296.0;
}

vec3 randomDirection(inout uint state)
{
    float z = randomFloat(state) * 2.0f - 1.0f;
    float a = randomFloat(state) * 6.2831853071;
    float r = sqrt(1.0f - z * z);
    float x = r * cos(a);
    float y = r * sin(a);
    return vec3(x, y, z);
}

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

    float r2 = radius * radius;
    vec3 oc = ray.origin - center;
    vec3 cr = cross(ray.direction, oc);
    if (dot(cr, cr) > r2)
    {
        return hit;
    }

    float b = dot(oc, ray.direction);
    float c = dot(oc, oc) - r2;
    float discriminant = b * b - c;

    float dst = -b - sqrt(discriminant);
        
    hit.d = dst;
    hit.p = ray.origin + ray.direction * dst;
    hit.n = normalize(hit.p - center);

    return hit;
}

Hit hitTriangle(
    Ray ray,
    vec3 v0, vec3 v1, vec3 v2
)
{
    Hit hit;
    hit.d = -1.0;

    vec3 e1 = v1 - v0;
    vec3 e2 = v2 - v0;
    vec3 pvec = cross(ray.direction, e2);
    float det = dot(e1, pvec);

    if (abs(det) < 1e-8) return hit;

    float invDet = 1.0 / det;
    vec3 tvec = ray.origin - v0;
    vec2 uv;
    uv.x = dot(tvec, pvec) * invDet;
    if (uv.x < 0.0 || uv.x > 1.0) return hit;

    vec3 qvec = cross(tvec, e1);
    uv.y = dot(ray.direction, qvec) * invDet;
    if (uv.y < 0.0 || uv.x + uv.y > 1.0) return hit;

    hit.d = dot(e2, qvec) * invDet;
    hit.p = ray.origin + ray.direction * (hit.d - 0.01);
    hit.n = normalize(cross(e1, e2));

    return hit;
}

Hit calculateCollision(Ray ray)
{
    Hit closest;
    closest.d = 1e10;
    
    for (int i = 0; i < _SpheresCount; i++)
    {
        Hit h = hitSphere(ray, _Spheres[i].position, _Spheres[i].radius);

        if (h.d >= 0.0 && h.d < closest.d)
        {
            closest = h;
            closest.mat = _Spheres[i].mat;
        }
    }

    for (int i = 0; i < _MeshesCount; i++)
    {
        for (int j = _Meshes[i].startIndex; j < _Meshes[i].startIndex + _Meshes[i].indexCount; j+=3)
        {
            (_Meshes[i].transform * vec4(_Vertices[_Indices[j]].position, 1.0)).xyz;

            Hit h = hitTriangle(ray,
                (_Meshes[i].transform * vec4(_Vertices[_Indices[j]].position, 1.0)).xyz,
                (_Meshes[i].transform * vec4(_Vertices[_Indices[j + 1]].position, 1.0)).xyz,
                (_Meshes[i].transform * vec4(_Vertices[_Indices[j + 2]].position, 1.0)).xyz);

            if (h.d >= 0.0 && h.d < closest.d)
            {
                closest = h;
                closest.mat = _Meshes[i].mat;
            }
        }
    }

    return closest;
}

vec3 traceRay(Ray ray, inout uint state)
{
    vec3 incomingLight = vec3(0.0);
    vec3 color = vec3(1.0);

    for (int i = 0; i < 8; i++)
    {
        Hit hit = calculateCollision(ray);
        if (hit.d < 1e9)
        {
            ray.origin = hit.p;

            vec3 dirDiffuse = normalize(hit.n + randomDirection(state));
            vec3 dirSpecular = reflect(ray.direction, hit.n);

            vec3 emittedLight = hit.mat.emissionColor * hit.mat.emissionStrength;
            incomingLight += emittedLight * color;

            float isSpecular = float(randomFloat(state) < 0.04 + 0.96 * hit.mat.metallic);
            ray.direction = mix(dirDiffuse, dirSpecular, hit.mat.smoothness * isSpecular);
            color *= mix(vec3(0.04), hit.mat.color, max(1.0 - isSpecular, hit.mat.metallic));
            
        } else
        {
            incomingLight += texture(_Skybox, ray.direction).rgb * color;
            break;
        }
    }

    return incomingLight;
}

out vec4 FragColor;
in vec3 glPosition;

void main()
{
    vec2 uv = glPosition.xy;
    vec2 texCoords = (vec2(0.5) + glPosition.xy * 0.5);
    ivec2 pixelCoords = ivec2(texCoords * _ScreenSize);

    uint state = uint(uint(pixelCoords.x) * uint(1973) + uint(pixelCoords.y) * uint(9277) + uint(_Frame) * uint(26699)) | uint(1);

    Ray ray = createCameraRay(uv);
    
    vec3 light = vec3(0.0f);
    for (int rayIndex = 0; rayIndex < 2; rayIndex++)
    {
        state += 1456;
        light += traceRay(ray, state);
    }
    light = light / 2.0;

    vec4 previous = texture(prevFrame, texCoords);
    FragColor = mix(previous, vec4(light, 1.0), 1.0 / float(_Frame + 1));
}