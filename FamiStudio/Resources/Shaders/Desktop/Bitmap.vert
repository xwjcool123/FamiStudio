attribute vec2  inPosition;
attribute vec4  inColor;
attribute vec2  inTexCoords;
attribute float inDepth;

uniform vec4 screenScaleBias;

noperspective out vec4 colorInterp; 
noperspective out vec2 texCoordsInterp;

void main()
{
    gl_Position = vec4(inPosition.xy * screenScaleBias.xy + screenScaleBias.zw, inDepth, 1);
    colorInterp = inColor;
    texCoordsInterp = inTexCoords;
}
