﻿using System;
using System.IO;
using Memoria.FFPR.IL2CPP;
using UnityEngine;

namespace Memoria.FFPR.Core
{
    public static class TextureHelper
    {
        public static Texture2D GetFragment(Texture2D texture, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            if (texture == null)
                return null;

            Texture2D result = new Texture2D(width, height, texture.format, false);
            Color[] colors = texture.GetPixels(x, y, width, height);
            result.SetPixels(colors);
            result.Apply();
            return result;
        }

        public static Texture2D CopyAsReadable(Texture texture)
        {
            if (texture == null)
                return null;

            RenderTexture oldTarget = Camera.main.targetTexture;
            RenderTexture oldActive = RenderTexture.active;

            Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
            try
            {
                Camera.main.targetTexture = rt;
                //Camera.main.Render();
                Graphics.Blit(texture, rt);

                RenderTexture.active = rt;
                result.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            }
            finally
            {
                RenderTexture.active = oldActive;
                Camera.main.targetTexture = oldTarget;
                RenderTexture.ReleaseTemporary(rt);
            }

            return result;
        }

        public static void WriteTextureToFile(Texture2D texture, String outputPath)
        {
            Byte[] data;
            String extension = Path.GetExtension(outputPath);
            switch (extension)
            {
                case ".png":
                    data = ImageConversion.EncodeToPNG(texture);
                    break;
                case ".jpg":
                    data = ImageConversion.EncodeToJPG(texture);
                    break;
                case ".tga":
                    data = ImageConversion.EncodeToTGA(texture);
                    break;
                default:
                    throw new NotSupportedException($"Not supported type [{extension}] of texture [{texture.name}]. Path: [{outputPath}]");
            }

            File.WriteAllBytes(outputPath, data);
        }

        public static Texture2D ReadTextureFromFile(String fullPath)
        {
            Byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            if (!ImageConversion.LoadImage(texture, bytes))
                throw new NotSupportedException($"Failed to load texture from file [{fullPath}]");
            return texture;
        }
    }
}