Shader "Unlit/GLVertexColor"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZWrite On
            ZTest LEqual
            Cull Off
            Lighting Off
            BindChannels {
                Bind "Color", color
                Bind "Vertex", vertex
            }
        }
    }
}
