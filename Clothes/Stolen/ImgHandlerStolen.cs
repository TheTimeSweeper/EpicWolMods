using System.IO;
using System.Reflection;
using UnityEngine;

namespace Clothes
{
    //stolen from tournamentEdition
    public static class ImgHandlerStolen
    {
        public static Sprite LoadSpriteFromAssets(string path)
        {
            Texture2D texture2D = LoadTex2D(path, true);
            texture2D.name = path;
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
            Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
            Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
            sprite.name = path;
            return sprite;
        }

        public static Texture2D LoadTex2D(string spriteFileName, bool pointFilter = false, Texture2D T2D = null, bool overrideFullPath = false)
        {
            string path = Path.Combine("Assets", spriteFileName);
            if (overrideFullPath)
            {
                path = spriteFileName;
            }
            string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
            Texture2D texture2D = T2D == null ? ImgHandlerStolen.LoadPNG(text, pointFilter) : T2D;
            if (pointFilter)
            {
                texture2D.filterMode = FilterMode.Point;
                texture2D.Apply();
            }
            return texture2D;
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
