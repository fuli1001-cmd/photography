using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Utilities
{
    public class Encryptor
    {

        /// <summary>  
        /// SHA1加密字符串  
        /// </summary>  
        /// <param name="source">源字符串</param>  
        /// <returns>加密后的字符串</returns>  

        //public string SHA1(string source)
        //{
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "SHA1");
        //}

        ///// <summary>  
        ///// MD5加密字符串  
        ///// </summary>  
        ///// <param name="source">源字符串</param>  
        ///// <returns>加密后的字符串</returns>  
        //public string MD5(string source)
        //{
        //    return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5"); ;
        //}

        //  方法三(MD5不可逆)：  
        //MD5不可逆加密  
        //32位加密  
        public static string GetMD5_32(string s, string _input_charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        //16位加密  
        public static string GetMd5_16(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        //默认密钥向量  
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /**//**//**//// <summary>  
                    /// DES加密字符串  
                    /// </summary>  
                    /// <param name="encryptString">待加密的字符串</param>  
                    /// <param name="encryptKey">加密密钥,要求为8位</param>  
                    /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>  
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbKey), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }

        }

        /**//**//**//// <summary>  
        /// DES解密字符串  
        /// </summary>  
        /// <param name="decryptString">待解密的字符串</param>  
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>  
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>  
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbKey), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }

        }

        public static string EncryptDES(string encryptString, string encryptKey, string iv)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Encoding.UTF8.GetBytes(iv.Substring(0, 8));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbKey), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }

        }

        public static string DecryptDES(string decryptString, string decryptKey, string iv)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Encoding.UTF8.GetBytes(iv.Substring(0, 8));
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbKey), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }

        }

        //  方法六(文件加密)：  
        //加密文件  
        private static void EncryptData(String inName, String outName, byte[] desKey, byte[] desIV)
        {
            //Create the file streams to handle the input and output files.  
            FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.  
            byte[] bin = new byte[100]; //This is intermediate storage for the encryption.  
            long rdlen = 0;              //This is the total number of bytes written.  
            long totlen = fin.Length;    //This is the total length of the input file.  
            int len;                     //This is the number of bytes to be written at a time.  

            DES des = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(fout, des.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);

            //Read from the input file, then encrypt and write to the output file.  
            {
                len = fin.Read(bin, 0, 100);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }
            encStream.Close();
            fout.Close();
            fin.Close();
        }

        //解密文件  
        private static void DecryptData(String inName, String outName, byte[] desKey, byte[] desIV)
        {
            //Create the file streams to handle the input and output files.  
            FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.  
            byte[] bin = new byte[100]; //This is intermediate storage for the encryption.  
            long rdlen = 0;              //This is the total number of bytes written.  
            long totlen = fin.Length;    //This is the total length of the input file.  
            int len;                     //This is the number of bytes to be written at a time.  
            DES des = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(fout, des.CreateDecryptor(desKey, desIV), CryptoStreamMode.Write);
            //Read from the input file, then encrypt and write to the output file.  
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 100);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }

            encStream.Close();
            fout.Close();
            fin.Close();
        }

        //方法     
        //加密方法     
        public string Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中     
            //原来使用的UTF8编码，我改成Unicode编码了，不行     
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            //byte[]  inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt);     
            //建立加密对象的密钥和偏移量     
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法     
            //使得输入密码必须输入英文文本     
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //Write  the  byte  array  into  the  crypto  stream     
            //(It  will  end  up  in  the  memory  stream)     
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //Get  the  data  back  from  the  memory  stream,  and  into  a  string     
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                //Format  as  hex     
                ret.AppendFormat("{0:X2}", b);
            }

            ret.ToString();
            return ret.ToString();
        }


        //解密方法     
        public string Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //Put  the  input  string  into  the  byte  array     
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            //建立加密对象的密钥和偏移量，此值重要，不能修改     
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //Flush  the  data  through  the  crypto  stream  into  the  memory  stream     
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            //Get  the  decrypted  data  back  from  the  memory  stream     
            //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象     
            StringBuilder ret = new StringBuilder();
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
