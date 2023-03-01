using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProcessTextureAtlas : AssetPostprocessor {

    void OnPreprocessTexture ()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.mipmapEnabled = false;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
 
    }

    public void OnPostprocessTexture (Texture2D texture)
    {
        int colCount = 16;
        int rowCount = 16;
        int sw = texture.width/colCount;
        int sh = texture.height/rowCount;
 
        List<SpriteMetaData> metas = new List<SpriteMetaData>();
 
        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                SpriteMetaData meta = new SpriteMetaData();
                meta.rect = new Rect(c * sw, r * sh, sw, sh);
                float uv1x = (c*sw)/(float)texture.width;
                float uv1y = (r*sh)/(float)texture.height;
                float uv2x = (c*sw+sw)/(float)texture.width;
                float uv2y = (r*sh)/(float)texture.height;
                float uv3x = (c*sw)/(float)texture.width;
                float uv3y = (r*sh+sh)/(float)texture.height;
                float uv4x = (c*sw+sw)/(float)texture.width;
                float uv4y = (r*sh+sh)/(float)texture.height;
                meta.name = uv1x + "," + uv1y + "|" + 
                			uv2x + "," + uv2y + "|" +
                			uv3x + "," + uv3y + "|" +
                			uv4x + "," + uv4y;
                metas.Add(meta);
            }
        }

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.spritesheet = metas.ToArray();
    }
 
}
