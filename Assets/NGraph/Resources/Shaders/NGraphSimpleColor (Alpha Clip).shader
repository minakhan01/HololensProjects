Shader "NGraph/SimpleColor Plot With Alpha Clip"
{
   Properties
   {
      _TintColor ("Tint Color", Color) = (1,1,1,1)
      _Clipping ("Clipping", Vector) = (-1, -1, 1, 1)
   }
   
   SubShader
   {
      Lighting Off
      ZWrite Off
      Cull Back
      Blend SrcAlpha OneMinusSrcAlpha
      Tags {"Queue" = "Transparent"}
      Color[_TintColor]
      Pass
      {
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
         
         struct appdata_t
         {
            float4 vertex : POSITION;
            half4 color : COLOR;
         };
         
         struct v2f
         {
            float4 pos : SV_POSITION;
            float2 worldPos : TEXCOORD1;
            half4 color : COLOR;
         };
         
         fixed4 _TintColor;
         float4 _Clipping;
         
         v2f vert (appdata_t v)
         {
            v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.color = _TintColor;
            o.worldPos = v.vertex;
            return o;
         }
         half4 frag (v2f IN) : COLOR
         {
            half4 col = half4(IN.color);
            
            if(IN.worldPos.x < _Clipping.x) col.a = 0.0;
            else if(IN.worldPos.x > _Clipping.y) col.a = 0.0;
            else if(IN.worldPos.y < _Clipping.z) col.a = 0.0;
            else if(IN.worldPos.y > _Clipping.w) col.a = 0.0;
            
            return col;
         }
         ENDCG
      }
      
   } 
   FallBack "Unlit/Transparent"
}