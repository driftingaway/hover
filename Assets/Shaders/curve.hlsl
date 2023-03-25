// this file is meant to be used in conjunction with a Custom Function node
// in ShaderGraph. You can find out more about how to use it at
// https://docs.unity3d.com/Packages/com.unity.shadergraph@13.1/manual/Custom-Function-Node.html

// all you need is the Position node (Object Space), connect its input to VertexPosOS and optionally
// provide an origin to determine where the rendering looks straight (and not curved)

#ifndef CURVER_INCLUDED
#define CURVER_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

uniform float _CURVER_STRAIGHT_RENDER_DISTANCE;
uniform float _CURVER_HORIZONTAL_CURVATURE;
uniform float _CURVER_VERTICAL_CURVATURE;

#define CURVER_CURVATURE_POWER 2

void GetModifiedObjectSpace_float(const float3 OriginWS, const float3 VertexPosOS, out float3 ModifiedVertexPosOS)
{
    const float3 VertexPosWS = TransformObjectToWorld(VertexPosOS);

    const float DistanceToCamera = distance(OriginWS, VertexPosWS);

    const float StraighteningMask = step(_CURVER_STRAIGHT_RENDER_DISTANCE, DistanceToCamera);

    const float2 Curvature = pow(DistanceToCamera - _CURVER_STRAIGHT_RENDER_DISTANCE, CURVER_CURVATURE_POWER)
        * float2(_CURVER_HORIZONTAL_CURVATURE, _CURVER_VERTICAL_CURVATURE) * pow(0.1f, CURVER_CURVATURE_POWER);

    const float3 ModifiedVertexPosWS = VertexPosWS + (StraighteningMask * float3(Curvature.x, Curvature.y, 0));

    ModifiedVertexPosOS = TransformWorldToObject(ModifiedVertexPosWS);
}

void GetModifiedObjectSpace_half(const half3 OriginWS, const half3 VertexPosOS, out half3 ModifiedVertexPosOS)
{
    const half3 VertexPosWS = TransformObjectToWorld(VertexPosOS);

    const half DistanceToCamera = distance(OriginWS, VertexPosWS);

    const half StraighteningMask = step(_CURVER_STRAIGHT_RENDER_DISTANCE, DistanceToCamera);

    const half2 Curvature = pow(DistanceToCamera - _CURVER_STRAIGHT_RENDER_DISTANCE, CURVER_CURVATURE_POWER)
        * half2(_CURVER_HORIZONTAL_CURVATURE, _CURVER_VERTICAL_CURVATURE) * pow(0.1f, CURVER_CURVATURE_POWER);

    const half3 ModifiedVertexPosWS = VertexPosWS + (StraighteningMask * half3(Curvature.x, Curvature.y, 0));

    ModifiedVertexPosOS = TransformWorldToObject(ModifiedVertexPosWS);
}

#undef CURVER_CURVATURE_POWER

#endif