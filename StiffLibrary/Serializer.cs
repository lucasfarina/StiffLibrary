using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace StiffLibrary
{
    public static class Serializer
    {
        public static BinaryFormatter binFor = new BinaryFormatter();

        public static string Serialize(object obj)
        {
            string tmp = "";
            MemoryStream ms = new MemoryStream();
            binFor.Serialize(ms, obj);
            tmp = System.Convert.ToBase64String(ms.ToArray());
            return tmp;
        }

        public static object Deserialize(string str)
        {
            MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(str));
            return binFor.Deserialize(ms);
        }

        //--------------------------Bytes to Base64 --------Base64 to Bytes
        public static string BlobToBase64(byte[] blob)
        {
            return Convert.ToBase64String(blob, 0, blob.Length);
        }

        public static byte[] Base64ToBlob(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        //--------------------------Main image functions
        public static System.Drawing.Image BlobToImage(byte[] blob)
        {
            MemoryStream ms = new MemoryStream(blob, 0, blob.Length);
            ms.Write(blob, 0, blob.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }

        public static byte[] ImageToBlob(string imagePath)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    return imageBytes;
                }
            }
        }

        public static byte[] ImageToBlob(System.Drawing.Image image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();
                return imageBytes;
            }
        }


        //-------------------------Delegated Image Facilitators

        public static string ImageToBase64(string imagePath)
        {
            return BlobToBase64(ImageToBlob(imagePath));
        }

        public static System.Drawing.Image Base64ToImage(string base64String)
        {
            return BlobToImage(Base64ToBlob(base64String));
        } 

        
    } 
}
