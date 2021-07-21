using UnityEngine;

// ReSharper disable once CheckNamespace
/// <summary>Contains extension methods for Colors (SR Version)</summary>
public static class SRColorExt
{
	/// <summary>
	/// Converts a color array to a Color Palette
	/// </summary>
	/// <param name="color">The color array to convert</param>
	public static SlimeAppearance.Palette ToPalette(this Color[] color)
	{
		return new SlimeAppearance.Palette()
		{
			Ammo = color[3],
			Bottom = color[0],
			Middle = color[1],
			Top = color[2]
		};
	}
}