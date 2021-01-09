using Guu.Utils;
using UnityEngine;

// ReSharper disable once CheckNamespace

/// <summary>Contains extension methods for Colors</summary>
public static class ColorExt
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