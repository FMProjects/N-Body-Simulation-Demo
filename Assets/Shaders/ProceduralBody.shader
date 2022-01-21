Shader "Unlit/ProceduralBody"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque"}

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct Body
            {
                float3 pos;//position
                float3 vel;//velocity
                float3 acc;//acceleration
                float mass;
            };


            struct v2f
            {
                uint id : SV_INSTANCEID;
                float4 vertex : SV_POSITION;
            };


            float3 faceCamera(float3 position, float3 centerPosition)
            {                
                //get direction to face
                float3 norm = normalize(_WorldSpaceCameraPos - centerPosition);

                //get axis to rotate on
                //axis to rotate on is orthogonal to original facing direction and new direction to face
                float3 axis = normalize(cross(norm, float3(0, 0, 1)));

                //since both vector are normalized, no need to divide by magnitude
                // a . b = |A|*|B|*cos(angle)
                float angle = acos(dot(norm, float3(0, 0, -1)));


                //rotate position of vertex
                //https://en.wikipedia.org/wiki/Rotation_matrix#Rotation_matrix_from_axis_and_angle
                float3 rotatedPos = axis * (dot(axis, position)) + 
                                    cross(cos(angle) * (cross(axis, position)), axis) + 
                                    sin(angle) * (cross(axis, position));
                
                return rotatedPos;
            }

            float4 _Color;
            StructuredBuffer<float3> quadVertices;
            StructuredBuffer<Body> bodies;

            //vertex_id-> id of vertex. We have six vertex so this will range from 0 to 5.
            //instance_id-> id of instance. The number of instances is the number of bodies we have. This will range from 0 to numberOfBodies-1.            
            v2f vert(uint vertex_id: SV_VertexID, uint instance_id : SV_InstanceID)
            {
                v2f o;

                //get vertex position
                float3 position = quadVertices[vertex_id];

                //make quad face camera
                position = faceCamera(position, bodies[instance_id].pos);


                //add body position
                position += bodies[instance_id].pos;

                //convert the vertex position from world space to clip space
                o.vertex = UnityObjectToClipPos(float4(position.xyz, 1));
                o.id = instance_id;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //modify color depending on acceleration and mass
                fixed accCol = length(bodies[i.id].acc)*1.2f;
                fixed massCol = bodies[i.id].mass*1.25f;
                //return _Color + fixed4(0, accCol, accCol, 1);
                return _Color + fixed4(0, accCol, massCol, 1);
            }

            ENDCG
        }
    }
}