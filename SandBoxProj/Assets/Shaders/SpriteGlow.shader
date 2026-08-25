Shader "Custom/InnerGlow_Optimized"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Factor("Factor", Range(0, 10)) = 1
        _SampleRange("Sample Range", Range(0, 10)) = 7
        _SampleInterval("Sample Interval", vector) = (1,1,0,0)
        _TexSize("Texture Size", vector) = (256,256,0,0)
    }

    SubShader
    {
        LOD 200
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Cull Back
            Lighting Off
            ZWrite Off
            Offset -1, -1
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half4 _Color;
            half _Factor;
            half _SampleRange;
            half2 _TexSize;
            half2 _SampleInterval;

            struct appdata_t
            {
                float4 vertex : POSITION;
                half2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
    
            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                half2 radius : TEXCOORD1; // 将半径计算移至顶点阶段
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                // 在顶点着色器中预计算半径，避免在片元中重复计算
                o.radius = _SampleInterval / _TexSize;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                // 获取当前像素颜色（只需采样一次中心点）
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                half range = _SampleRange;
                half inner = 0;
                half outter = 0;
                
                // 预计算 count 的倒数，将除法转为乘法
                half count = (range * 2 + 1) * (range * 2 + 1);
                half invCount = 1.0h / count;

                // 强制展开循环，消除动态分支开销
                [unroll(15)]
                for (half k = -range; k <= range; ++k)
                {
                    [unroll(15)]
                    for (half j = -range; j <= range; ++j)
                    {
                        // 跳过中心点，避免重复采样当前像素
                        if (k == 0 && j == 0) continue; 
                        
                        half2 offset = half2(k, j) * i.radius;
                        fixed4 m = tex2D(_MainTex, i.uv + offset);
                        outter += 1 - m.a;
                        inner += m.a;
                    }
                }
                
                // 加上中心点本身的值
                inner += col.a;
                outter += 1 - col.a;

                // 使用乘法代替除法
                inner *= invCount;
                outter *= invCount;
                
                half out_alpha = max(col.a, inner);
                half in_alpha = min(out_alpha, outter);
                
                // 叠加内发光颜色
                col.rgb += in_alpha * _Factor * _Color.a * _Color.rgb;
                col.a = out_alpha;
                
                return col;
            }
            ENDCG
        }
    }
}