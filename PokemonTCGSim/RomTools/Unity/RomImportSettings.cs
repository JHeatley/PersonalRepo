#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Import settings for graphics extracted from GBA/DS games (RomTools' PNGs): put this file in an Editor folder of your
/// Unity project, and any PNG under a folder named "RomImport" (Assets/RomImport/..., at any depth) imports as crisp pixel
/// art: a sprite, point filtering (no blur), no compression (exact colours), no mipmaps, and PixelsPerUnit pixels to one
/// Unity unit. Tile sheets come in as Multiple sprites: open the Sprite Editor and Slice → Grid By Cell Size (8x8 for
/// tiles, 16x16, 32x32 or 64x64 for characters), then drag the slices into a Tile Palette (Window → 2D → Tile Palette)
/// to paint maps on a Tilemap.
/// </summary>
public class RomImportSettings : AssetPostprocessor
{
    /// <summary>Pixels per Unity unit: 16 suits 16x16 GBA/DS characters on an 8-pixel tile grid; change to taste.</summary>
    public const int PixelsPerUnit = 16;

    private bool InRomImportFolder
    {
        get { return assetPath.Replace('\\', '/').Contains("/RomImport/"); }
    }

    private void OnPreprocessTexture()
    {
        if (!InRomImportFolder) return;
        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        // Tile sheets and screens are several pictures in one; a palette swatch or a single sprite is one.
        bool sheet = !assetPath.EndsWith("_palette.png");
        importer.spriteImportMode = sheet ? SpriteImportMode.Multiple : SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.isReadable = true; // so editor tools can read the pixels (slicing, palette swaps)
    }
}
#endif
