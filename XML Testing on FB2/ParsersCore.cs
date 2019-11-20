using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace XML_Testing_on_FB2
{

    abstract class ParsersCore
    {
        protected static string  checkImageFileExtention(string base64ImageSubstring)
        {
            switch (base64ImageSubstring.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
        protected static void decodeBinaryImage(string base64ImageString, string realName, string path = @"../../XML/pictures")
        {

            string[] preName = realName.Split('.');
            string imageName = "";
            for (int i = 0; i < preName.Length - 2; i++)
            {
                imageName += preName[i] + '.';
            }
            imageName += preName[preName.Length - 2];

            try
            {
                string substring = base64ImageString.Substring(0, 5);
                string extension = checkImageFileExtention(substring);
                byte[] base64EncodedBytes = Convert.FromBase64String(base64ImageString);
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                File.WriteAllBytes($"{path}/{imageName}.{extension}", base64EncodedBytes);
            }
            catch
            {
                return;
            }
        }
        // Idk but without statik it isn't working
    }
}
