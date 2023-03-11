using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using MineFramework;

namespace MineFramework
{
    public class EncryptCls
    {
        // 암호화/복호화 관련
        private static readonly string KEY = "01234567890123456789012345678901";
        // 128bit (16자리)
        private static readonly string KEY_128 = KEY.Substring(0, 128 / 8);
        // 256bit (32자리)
        private static readonly string KEY_256 = KEY;

        #region AES 128비트방식으로 암호화 하기 (encryptAES128)
        /// <summary>
        /// 128 비트 방식으로 암호화 합니다.
        /// </summary>
        /// <param name="plain">암호화할 문자열</param>
        /// <returns></returns>
        public static string encryptAES128(string plain)
        {
            try
            {
                // byte로 변환
                byte[] plainBytes = Encoding.UTF8.GetBytes(plain);

                // 레인달 알고리즘
                RijndaelManaged rm = new RijndaelManaged();
                rm.Mode = CipherMode.CBC;
                rm.Padding = PaddingMode.PKCS7;
                rm.KeySize = 128;

                // 메모리스트림 생성
                MemoryStream memoryStream = new MemoryStream();
                // Key값, iv값 정의
                ICryptoTransform encryptor = rm.CreateEncryptor(Encoding.UTF8.GetBytes(KEY_128), Encoding.UTF8.GetBytes(KEY_128));
                // 크립트스트림을 key와 iv값으로 메모리 스트림을 이용하여 생성
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                // 크립트스트림에 바이트배열을 쓰고 플러쉬
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
                // 메모리스트림에 담겨있는 암호화된 바이트배열을 닫음
                byte[] encryptBytes = memoryStream.ToArray();
                // 베이스64로 변환
                string encryptString = Convert.ToBase64String(encryptBytes);
                // 스트림닫기
                cryptoStream.Close();
                memoryStream.Close();

                return encryptString;
            }
            catch (Exception e)
            {
                LogCls.writeLog("", "PrcLog.log", "AES128 Encrypt Error : " + e.Message);
                return null;
            }
        }
        #endregion

        #region AES 128비트방식으로 복호화 하기 (decryptAES128)
        public static string decryptAES128(string encrypt)
        {
            try
            {
                // base64를 바이트로 변환
                byte[] encryptBytes = Convert.FromBase64String(encrypt);

                // 레인달 알고리즘
                RijndaelManaged rm = new RijndaelManaged();
                rm.Mode = CipherMode.CBC;
                rm.Padding = PaddingMode.PKCS7;
                rm.KeySize = 128;

                // 메모리스트림 생성
                MemoryStream memoryStream = new MemoryStream(encryptBytes);
                // Key값, iv값 정의
                ICryptoTransform decryptor = rm.CreateDecryptor(Encoding.UTF8.GetBytes(KEY_128), Encoding.UTF8.GetBytes(KEY_128));
                // 크립트스트림을 key와 iv값으로 메모리 스트림을 이용하여 생성
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                // 복호화된 데이터를 담을 바이트 배열을 선언한다.
                byte[] plainBytes = new byte[encryptBytes.Length];
                int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);
                // 복호화된 바이트배열을 string으로 변환
                string plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);
                // 스트림닫기
                cryptoStream.Close();
                memoryStream.Close();

                return plainString;
            }
            catch (Exception e)
            {
                LogCls.writeLog("", "PrcLog.log", "AES128 Decrypt Error : " + e.Message);
                return null;
            }
        }
        #endregion

        #region AES 256비트방식으로 암호화 하기 (encryptAES256)
        public static string encryptAES256(string plain)
        {
            try
            {
                RijndaelManaged rm = new RijndaelManaged();
                rm.KeySize = 256;
                rm.BlockSize = 256;
                rm.Mode = CipherMode.CBC;
                rm.Padding = PaddingMode.PKCS7;
                rm.Key = Encoding.UTF8.GetBytes(KEY_256);
                rm.IV = Encoding.UTF8.GetBytes(KEY_256);

                var encrypt = rm.CreateEncryptor(rm.Key, rm.IV);
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(plain);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                string Output = Convert.ToBase64String(xBuff);

                return Output;

            }
            catch (Exception e)
            {
                LogCls.writeLog("", "PrcLog.log", "AES256 Encrypt Error : " + e.Message);
                return "";
            }
        }
        #endregion

        #region AES 256비트방식으로 복호화 하기 (decryptAES256)
        public static string decryptAES256(string plain)
        {
            try
            {
                RijndaelManaged rm = new RijndaelManaged();
                rm.KeySize = 256;
                rm.BlockSize = 256;
                rm.Mode = CipherMode.CBC;
                rm.Padding = PaddingMode.PKCS7;
                rm.Key = Encoding.UTF8.GetBytes(KEY_256);
                rm.IV = Encoding.UTF8.GetBytes(KEY_256);

                var decrypt = rm.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(plain);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                return Encoding.UTF8.GetString(xBuff);
            }
            catch (Exception e)
            {
                LogCls.writeLog("", "PrcLog.log", "AES256 Decrypt Error : " + e.Message);
                return "";
            }
        }
        #endregion
    }
}
