using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace AutoArtImportProcessor
{
    public class AutoTextureImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.EndsWith(".png")) return;

            TextureImporter importer = assetImporter as TextureImporter;
            if (importer == null) return;

            // 强制设置为 Sprite
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false; // 2D 关闭 Mipmaps
            bool needAtlas = false;
            // 根据路径关键字区分压缩格式
            if (assetPath.Contains("/UI/"))
            {
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                {
                    name = "Android",
                    overridden = true,
                    format = TextureImporterFormat.ASTC_4x4,
                });

                needAtlas = true;
            }
            else if (assetPath.Contains("/Scene/"))
            {
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                {
                    name = "Android",
                    overridden = true,
                    format = TextureImporterFormat.ASTC_6x6,
                });

                needAtlas = true;
            }
            else if (assetPath.Contains("/Default/"))
            {
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                {
                    name = "Android",
                    overridden = true,
                    format = TextureImporterFormat.RGBA32,
                });

                needAtlas = false;
            }

            if (needAtlas)
            {
                EditorApplication.delayCall += DelayCallFunc;
            }
        }

        private void DelayCallFunc()
        {
            AssetDatabase.Refresh();
            string atlasPath = GetAtlasFilePath(assetPath);
            if (File.Exists(atlasPath))
            {
                Debug.Log("File.Exists atlasPath:" + atlasPath);
                File.Delete(atlasPath);
            }
            else
            {
                Debug.Log("@@@@ File.Exists atlasPath:" + atlasPath);
            }

            SpriteAtlasAsset spriteAtlas = new SpriteAtlasAsset();
            SpriteAtlasAsset.Save(spriteAtlas, atlasPath);


            if (spriteAtlas == null)
            {
                Debug.LogError("            if (spriteAtlas == null)\n");
                return;
            }

            List<Sprite> sprites = new List<Sprite>();
            string[] imageFiles =
                Directory.GetFiles(Path.GetDirectoryName(assetPath), "*.png", SearchOption.AllDirectories);

            foreach (string file in imageFiles)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(file);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            if (sprites.Count == 0)
            {
                EditorUtility.DisplayDialog("No Sprites", "No sprites found in the selected folder.", "OK");
                return;
            }


            // 添加精灵
            foreach (Sprite sprite in sprites)
            {
                spriteAtlas.Add(new[] { sprite });
            }

            EditorUtility.SetDirty(spriteAtlas);
            // 保存
            SpriteAtlasAsset.Save(spriteAtlas, atlasPath);
            AssetDatabase.ImportAsset(GetRaletivePath(atlasPath), ImportAssetOptions.ForceUpdate);
            SpriteAtlas runtimeAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(GetRaletivePath(atlasPath));
            if (runtimeAtlas != null)
            {
                // 对当前BuildTarget重新打包该图集
                SpriteAtlasUtility.PackAtlases(
                    new SpriteAtlas[] { runtimeAtlas },
                    EditorUserBuildSettings.activeBuildTarget,
                    false
                );
            }

            AssetDatabase.Refresh();
            EditorApplication.delayCall -= DelayCallFunc;
        }

        private string GetRaletivePath(string path)
        {
            return path.Replace(Application.dataPath, "Assets/");
        }

        private string GetAtlasFilePath(string path)
        {
            if (path.Contains("/UI/")) return $"{Application.dataPath}/Arts/UI/UIAtlas.spriteatlasv2";
            if (path.Contains("/Scene/")) return $"{Application.dataPath}/Arts/Scene/SceneAtlas.spriteatlasv2";
            return $"{Application.dataPath}/Arts/Default/DefaultAtlas.spriteatlasv2";
        }
    }
}
