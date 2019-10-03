Shader "ProtoTools/UnlitAlpha"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans. (Alpha)", 2D) = "white" { }
    }

    Category
    {
        ZWrite On
        //Alphatest Greater 0.5
		Blend SrcAlpha OneMinusSrcAlpha
        SubShader
        {
        Tags { "Queue" = "Transparent" }
            Pass
            {
            	ZWrite Off
   			    Cull Off
                Lighting Off
				SetTexture [_MainTex]
				{
					constantColor [_Color]
					Combine texture * constant, texture * constant 
				} 
                
                
//				// Use texture alpha to blend up to white (= full illumination)
//				SetTexture [_MainTex] {
//					// Pull the color property into this blender
//					constantColor [_Color]
//					// And use the texture's alpha to blend between it and
//					// vertex color
//					combine constant lerp(texture) previous
//				}
//				// Multiply in texture
//				SetTexture [_MainTex] {
//					combine previous * texture
//				}
                
                
            }
        } 
    }
}
