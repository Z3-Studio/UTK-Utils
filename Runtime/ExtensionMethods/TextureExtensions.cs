using System;
using UnityEngine;
//using AmazingAssets.ResizePro;

namespace Z3.Utils.ExtensionMethods
{
    public static class TextureExtensions
    {
        public static Texture2D ToTexture(this byte[] bytes)
        {
            // Opção A: Classe Lib.
            // Opção B: https://docs.unity3d.com/ScriptReference/ImageConversion.EncodeArrayToPNG.html
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            //texture.LoadRawTextureData(bytes); // Opção C
            //texture.Apply();
            return texture;
        }

        public static Texture2D Compress(this Texture2D texture2D)
        {
            Texture2D uncompressedTexture = new Texture2D(texture2D.width, texture2D.height, TextureFormat.RGBA32, false);
            RenderTexture renderTexture = new RenderTexture(texture2D.width, texture2D.height, 32);

            Graphics.Blit(texture2D, renderTexture);
            RenderTexture.active = renderTexture;

            uncompressedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            uncompressedTexture.Apply();
            return uncompressedTexture;
        }

        public static Sprite ToSprite(this byte[] texture)
        {
            return texture.ToTexture().ToSprite();
        }

        public static Sprite ToSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        public static string ToBase64String(this Texture2D texture2D)
        {
            byte[] bytes = texture2D.EncodeToPNG();
            return Convert.ToBase64String(bytes);
        }

        public static Texture2D ToTextureBase64(this string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            return bytes.ToTexture();
        }

        public static Texture2D CopyTexture(this Texture2D texture)
        {
            Color[] sourcePixels = texture.GetPixels();

            Texture2D copiedTexture = new Texture2D(texture.width, texture.height);

            copiedTexture.SetPixels(sourcePixels);

            copiedTexture.Apply();

            return copiedTexture;
        }
    }
}