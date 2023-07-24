﻿using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomPalettes
{
    internal static class ImgHandler
    {
        public static Sprite LoadSpriteFromAssets(string fileName)
        {
            Texture2D texture2D = LoadTex2DFromAssets(fileName);

            return CreateSprite(texture2D, fileName);
        }

        public static Sprite LoadSprite(string path, string name)
        {
            Texture2D texture2D = LoadTex2D(path, true);

            return CreateSprite(texture2D, name);
        }

        private static Sprite CreateSprite(Texture2D texture2D, string name)
        {
            texture2D.name = name;
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
            Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
            Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
            sprite.name = name;
            return sprite;
        }

        public static Texture2D LoadTex2DFromAssets(string fileName, bool pointFilter = false)
        {
            string fullFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"Assets/{fileName}");

            return LoadTex2D(fullFilePath, pointFilter);
        }

        public static Texture2D LoadTex2D(string fullFilePath, bool pointFilter = false)
        {
            Texture2D texture2D = ImgHandler.LoadPNG(fullFilePath, pointFilter);
            if (pointFilter)
            {
                texture2D.filterMode = FilterMode.Point;
                texture2D.Apply();
            }
            return texture2D;
        }

        public static Texture2D LoadPNG(string assemblyDir, string folderName, string fileName)
        {
            string fullFilePath = Path.Combine(assemblyDir, folderName);
            fullFilePath = Path.Combine(fullFilePath, fileName);

            return LoadPNG(fullFilePath);
        }
        public static Texture2D LoadPNG(string filePath, bool pointFilter = false)
        {
            Texture2D texture2D = new Texture2D(2, 2);
            byte[] data = File.ReadAllBytes(filePath);
            texture2D.LoadImage(data);
            texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
            texture2D.Apply();

            return texture2D;
        }

    }
}