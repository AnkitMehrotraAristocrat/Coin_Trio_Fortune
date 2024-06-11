// Universal Render Pipeline Color correction shader by AtomicJoe
// feel free to use it as you like, just don't sell this in the assetstore
// version 1.02

	uniform half3 _TonemappedLitColorFilter;
	uniform float _TonemappedLitGamma;
	uniform float _TonemappedLitContrast;
	uniform float _TonemappedLitBrightness;
	uniform float _TonemappedLitSaturation;
	uniform float _TonemappedLitHueShift;

	inline float3 TonemappedLitRgbToHsv(float3 c)
{
	float4 K = float4(0.0, -0.33333333333333333333333333333334, 0.66666666666666666666666666666667, -1.0); //half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-4;
	return float3(abs(q.z + (q.w - q.y) * rcp(6.0 * d + e)), d * rcp(q.x + e), q.x);
}

inline float3 TonemappedLitHsvToRgb(float3 c)
{
	float4 K = float4(1.0, 0.66666666666666666666666666666667, 0.33333333333333333333333333333334, 3.0); //half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0)
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return (c.z * lerp(K.xxx, saturate(p - K.xxx), c.y));
}



	inline half4 TonemappedLitColorCorrection(half4 col)
	{
		float3 hsv = TonemappedLitRgbToHsv(col);
		hsv.x = frac(hsv.x + _TonemappedLitHueShift);
		hsv.z = (pow(hsv.z, 1.0 + _TonemappedLitGamma) * (1.0 + _TonemappedLitContrast)) + _TonemappedLitBrightness;
		hsv.y *= (1.0 + _TonemappedLitSaturation);
		col.rgb = TonemappedLitHsvToRgb(hsv) * (_TonemappedLitColorFilter + 1.0);
		col.rgb > 0.0 ? col.rgb : 0.0;
		return col;
	}
