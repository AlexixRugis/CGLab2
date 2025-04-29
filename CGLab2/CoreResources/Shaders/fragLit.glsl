#version 410 core

uniform sampler2D texture0;

uniform vec3 AmbientColor;
uniform vec3 LightColor;
uniform vec3 LightPos;
uniform vec3 ViewPos;

uniform vec4 Color;

out vec4 FragColor;
in vec3 normal;
in vec3 fragPos;
in vec2 texCoord;

void main()
{
    vec4 c = Color * texture(texture0, texCoord);

    if (c.a < 1.0f)
    {
        discard;
    }

    // diffuse 
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(LightPos - fragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = LightColor * diff;

    // specular
    vec3 specular = vec3(0.0);
    if (dot(lightDir, norm) > 0.0)
    {
        vec3 viewDir = normalize(ViewPos - fragPos);
        vec3 halfwayDir = normalize(lightDir + viewDir);
        float spec = pow(max(dot(halfwayDir, norm), 0.0), 15);
        specular = LightColor * spec;
    }

    c = vec4(AmbientColor + diffuse + specular, 0.0) * c;
    c.a = 1.0;
    FragColor = c;
}