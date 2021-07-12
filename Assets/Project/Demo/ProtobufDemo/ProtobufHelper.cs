using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace InteractionFramework.Runtime.Demo
{
    public static class ProtobufHelper
    {
        public static byte[] Serialize<T>(T instance)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, instance);
                bytes = new byte[ms.Position];
                var fullBytes = ms.GetBuffer();
                Array.Copy(fullBytes, bytes, bytes.Length);
            }
            return bytes;
        }

        public static T Deserialize<T>(byte[] obj)
        {
            byte[] bytes = obj;
            using (var ms = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }





        //将对象序列化为字符串  
        public static string SerializerToString<T>(T t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, t);
                return ASCIIEncoding.UTF8.GetString(ms.ToArray());
            }
        }

        //将对象序列化到文件  
        public static void SerializerToFile<T>(T t, string filePath)
        {
            using (var file = File.Create(filePath))
            {
                Serializer.Serialize(file, t);
            }
        }

        //将字符串转化为对象  
        public static T DederializerFromString<T>(string str)
        {
            //using作为语句，用于定义一个范围，在此范围的末尾将释放对象  
            //将字符串转化为内存流  
            using (MemoryStream ms = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(str)))
            {
                T obj = Serializer.Deserialize<T>(ms);
                return obj;
            }
        }

        //将文件数据转化为对象  
        public static T DederializerFromFile<T>(string filePath)
        {
            using (var file = File.OpenRead(filePath))
            {
                T obj = Serializer.Deserialize<T>(file);
                return obj;
            }
        }
    }
}