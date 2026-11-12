Shader "Hidden/Fluffy/PawTransitionMask"
{
    Properties
    {
        _Opening ("Opening", Float) = 0
        _Aspect ("Aspect", Float) = 1
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _Opening;
            float _Aspect;

            float ellipse(float2 position, float2 center, float2 radius, float edge)
            {
                float distance = length((position - center) / radius);
                return 1.0 - smoothstep(1.0 - edge * 3.0, 1.0 + edge * 3.0, distance);
            }

            fixed4 frag(v2f_img input) : SV_Target
            {
                if (_Opening <= 0.0001) return fixed4(0, 0, 0, 1);
                float2 position = (input.uv - 0.5) * float2(_Aspect, 1.0) / _Opening;
                float edge = 0.006 / _Opening;
                float paw = ellipse(position, float2(0, -0.12), float2(0.34, 0.27), edge);
                paw = max(paw, ellipse(position, float2(-0.30, 0.19), float2(0.105, 0.13), edge));
                paw = max(paw, ellipse(position, float2(-0.11, 0.31), float2(0.105, 0.13), edge));
                paw = max(paw, ellipse(position, float2(0.11, 0.31), float2(0.105, 0.13), edge));
                paw = max(paw, ellipse(position, float2(0.30, 0.19), float2(0.105, 0.13), edge));
                return fixed4(0, 0, 0, saturate(1.0 - paw));
            }
            ENDCG
        }
    }
}
