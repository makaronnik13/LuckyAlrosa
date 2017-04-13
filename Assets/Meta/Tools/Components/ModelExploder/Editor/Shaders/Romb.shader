Shader "Meta/Tools/Romb" {
     Properties {
        _Color ("Color", Color) = (1,1,1,1)
    }
   
             SubShader {
        
		Offset -3000, -3000
        
      CGPROGRAM
      #pragma surface surf BlinnPhong

      float4 _Color;
      struct Input {
          float4 color : COLOR;
      };
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = _Color;
          o.Alpha = 1;
      }
      ENDCG
    }
}