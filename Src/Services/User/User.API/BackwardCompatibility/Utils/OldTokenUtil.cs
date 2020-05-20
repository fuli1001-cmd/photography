using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UtilLib.Util;

namespace Photography.Services.User.API.BackwardCompatibility.Utils
{
    public class OldTokenUtil
    {
        private const string ENCRYPT_KEY = "Ars!1&90";

        public static string GetTokenString(int userId, string password)
        {
            string encryptedPassword = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                encryptedPassword = Encryptor.GetMd5Hash(md5Hash, password);
            }

            var content = userId + "_" + encryptedPassword + "_" + CommonUtil.GetTimestamp(DateTime.Now);
            return Encryptor.EncryptDES(content, ENCRYPT_KEY);
        }
    }
}
