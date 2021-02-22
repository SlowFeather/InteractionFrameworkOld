using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InteractionFramework.Runtime
{
    public class TextureKit:MonoSingletion<TextureKit>
    {
        /// <summary>
        /// 将RenderTexture保存成一张png图片  
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="contents"></param>
        /// <param name="pngName"></param>
        /// <returns></returns>
        public bool SaveRenderTextureToPNG(RenderTexture rt, string contents, string pngName)
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();
            if (!Directory.Exists(contents))
                Directory.CreateDirectory(contents);
            using (FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.OpenOrCreate))
            {
                BinaryWriter writer = new BinaryWriter(file);
                writer.Write(bytes);
                file.Close();
            }
                
            Texture2D.DestroyImmediate(png);
            png = null;
            RenderTexture.active = prev;
            return true;
        }

        /// <summary>
        /// 将Texture写入文件
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="fileName"></param>
        public void SaveTextureToFile(Texture2D texture, string fileName)
        {
            var bytes = texture.EncodeToPNG();
            using (FileStream file = File.Open(Application.dataPath + "/" + fileName, FileMode.OpenOrCreate))
            {
                var binary = new BinaryWriter(file);
                binary.Write(bytes);
                file.Close();
            }
        }

        /// <summary>
        /// 本地图片转成Base64
        /// </summary>
        /// <param name="datapath">本地图片路径</param>
        /// <returns></returns>
        public string GetImgBase64String(string datapath)
        {
            FileInfo file = new FileInfo(datapath);
            using (FileStream stream = file.OpenRead()) {
                byte[] buffer = new byte[file.Length];
                //读取图片字节流
                stream.Read(buffer, 0, Convert.ToInt32(file.Length));
                //base64字符串
                string imageBase64 = Convert.ToBase64String(buffer);
                stream.Close();
                //Debug.Log(imageBase64);
                return imageBase64;
            }
        }

        /// <summary>
        /// Base64转Texture2D
        /// </summary>
        /// <param name="Base64STR"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Texture2D Base64ToTexter2d(string Base64STR, float width, float height)
        {
            Texture2D pic = new Texture2D((int)width, (int)height);
            byte[] data = System.Convert.FromBase64String(Base64STR);
            pic.LoadImage(data);
            return pic;
        }

        /// <summary>
        /// 运行模式下Texture转换成Texture2D
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public Texture2D TextureToTexture2D(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }

        /// <summary>
        /// 将texture2d转换为Sprite
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        public Sprite Texture2DToSprite(Texture2D tex)
        {
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            return sprite;
        }

        /// <summary>
        /// Base64直接保存成本地png
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="pic_fileName"></param>
        public void Image64SaveLocal(string base64, string pic_fileName)
        {
            string path = null;
#if UNITY_ANDROID
        path=Application.persistentDataPath+"/"+pic_fileName;
#elif UNITY_IPHONE
        path=Application.persistentDataPath+"/"+pic_fileName;
#elif UNITY_EDITOR
            path = Application.streamingAssetsPath + "/" + pic_fileName + ".png";
#endif
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(base64);
            using (FileStream newFs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
                newFs.Write(byteArray, 0, byteArray.Length);
                newFs.Close();
            }
        }

        /// <summary>
        /// Texture图片保存
        /// </summary>
        /// <param name="tex">Tex.</param>
        public void TextureSaveLocal(Texture tex, string pic_fileName)
        {
            string path = null;
#if UNITY_ANDROID
        path=Application.persistentDataPath+"/"+pic_fileName;
#elif UNITY_IPHONE
        path=Application.persistentDataPath+"/"+pic_fileName;
#elif UNITY_EDITOR
            path = Application.streamingAssetsPath + "/" + pic_fileName + ".jpg";
#endif
            Texture2D saveImageTex = tex as Texture2D;
            using (FileStream newFs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] bytes = saveImageTex.EncodeToJPG();
                newFs.Write(bytes, 0, bytes.Length);
                newFs.Close();
            }
        }

        /// <summary>
        /// 本地图片加载
        /// </summary>
        public static IEnumerator LoadTexture(string path, string textureName, Action<Texture> action = null)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(path))
            {
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError)
                {
                    Debug.LogError(uwr.error);
                }
                else
                {
                    Texture texture = DownloadHandlerTexture.GetContent(uwr);
                    texture.name = textureName;
                    if (action != null)
                    {
                        action.Invoke(texture);
                    }
                }
            }
        }


    }
}
