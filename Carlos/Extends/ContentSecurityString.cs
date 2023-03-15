using System;
using System.IO;
using System.Text;
using Carlos.Exceptions;
using Carlos.Enumerations;
using System.Security.Cryptography;
namespace Carlos.Extends
{
    /// <summary>
    /// 在加密学概念上保证内容安全的字符串扩展类。
    /// </summary>
    public class ContentSecurityString
    {
        private readonly byte[] mKeys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };//加密或着解密用的IV。
        /// <summary>
        /// 获取当前实例中用于存储文本内容的基础字符串，这个字符串是否经过加密处理，由SecurityStatus属性决定。
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// 获取当前实例是否已经启用了循环加密。
        /// </summary>
        public bool IsCyclicEncrypt { get; private set; }
        /// <summary>
        /// 获取当前实例在启用了循环加密的情况下，其循环加密或解密的次数。
        /// </summary>
        public int Cycle { get; private set; }
        /// <summary>
        /// 获取当前实例的安全状态或者加密状态。
        /// </summary>
        public SecurityStatus SecurityStatus { get; private set; }
        /// <summary>
        /// 构造函数，通过指定的字符串初始化一个ContentSecurityString实例，并初始化这个实例的安全状态。
        /// </summary>
        /// <param name="basicStr">一个指定的字符串。</param>
        public ContentSecurityString(string basicStr)
        {
            Content = basicStr;
            IsCyclicEncrypt = false;
            SecurityStatus = SecurityStatus.Unencrypted;
        }
        /// <summary>
        /// 构造函数，通过一个指定的字符串初始化一个ContentSecurityString实例，并根据isEncryptedStr参数决定这个实例的安全状态。
        /// </summary>
        /// <param name="basicStr">一个指定的字符串。</param>
        /// <param name="isEncryptedStr">用于决定实例安全状态的参数，如果这个参数为true，则会初始化安全状态为“已加密（SecurityStatus.Encrypted）”，否则会初始化安全状态为“未加密（SecurityStatus.Unencrypted）”</param>
        public ContentSecurityString(string basicStr, bool isEncryptedStr)
        {
            Content = basicStr;
            IsCyclicEncrypt = false;
            if (isEncryptedStr) SecurityStatus = SecurityStatus.Encrypted;
            else SecurityStatus = SecurityStatus.Unencrypted;
        }
        /// <summary>
        /// 构造函数，通过一个指定的字符串初始化一个ContentSecurityString实例，与此同时指定该实例是否需要或者经过循环加密，以及循环加密的次数，并根据isEncryptedStr参数决定这个实例的安全状态。
        /// </summary>
        /// <param name="basicStr">一个指定的字符串。</param>
        /// <param name="isEncryptedStr">用于决定实例安全状态的参数，如果这个参数为true，则会初始化安全状态为“已加密（SecurityStatus.Encrypted）”，否则会初始化安全状态为“未加密（SecurityStatus.Unencrypted）”</param>
        /// <param name="isCyclicEncrypt">是否已经启用了循环加密。</param>
        /// <param name="cycle">当前实例在启用了循环加密的情况下，其循环加密或解密的次数。</param>
        public ContentSecurityString(string basicStr, bool isEncryptedStr, bool isCyclicEncrypt, int cycle)
        {
            Content = basicStr;
            IsCyclicEncrypt = isCyclicEncrypt;
            if (isCyclicEncrypt) Cycle = cycle;
            else Cycle = 1;
            if (isEncryptedStr) SecurityStatus = SecurityStatus.Encrypted;
            else SecurityStatus = SecurityStatus.Unencrypted;
        }
        /// <summary>
        /// 指示这个实例是否能够加密。
        /// </summary>
        public virtual bool CanEncrypt => SecurityStatus == SecurityStatus.Unencrypted;
        /// <summary>
        /// 指示这个实例是否能够解密。
        /// </summary>
        public virtual bool CanDecrypt => SecurityStatus == SecurityStatus.Encrypted;
        /// <summary>
        /// 加密当前实例包含的字符串，如果安全状态不允许，则无法加密。
        /// </summary>
        /// <param name="encryptKey">加密需要用到的密钥。</param>
        /// <exception cref="CannotEncryptException">当无法执行加密操作时，则将会抛出这个异常。</exception>
        public virtual void Encrypt(string encryptKey)
        {
            try
            {
                byte[] key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] iv = mKeys;
                byte[] inputByteArr = Encoding.UTF8.GetBytes(Content);
                if (SecurityStatus == SecurityStatus.Unencrypted)
                {
                    DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, provider.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                    cryptoStream.Write(inputByteArr, 0, inputByteArr.Length);
                    cryptoStream.FlushFinalBlock();
                    Content = Convert.ToBase64String(memoryStream.ToArray());
                    SecurityStatus = SecurityStatus.Encrypted;
                }
                else throw new CannotEncryptException();
            }
            catch (Exception catchedException) { throw catchedException; }
        }
        /// <summary>
        /// 解密当前实例包含的字符串，如果安全状态不允许，则无法解密。
        /// </summary>
        /// <param name="decryptKey">解密需要用到的密钥。</param>
        /// <exception cref="CannotDecryptException">当无法执行解密操作时，则将会抛出这个异常。</exception>
        public virtual void Decrypt(string decryptKey)
        {
            if (Content == string.Empty) Content = string.Empty;
            try
            {
                byte[] key = Encoding.UTF8.GetBytes(decryptKey);
                byte[] iv = mKeys;
                byte[] inputByteArr = Convert.FromBase64String(Content);
                if (SecurityStatus == SecurityStatus.Encrypted)
                {
                    DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, provider.CreateDecryptor(key, iv), CryptoStreamMode.Write);
                    cryptoStream.Write(inputByteArr, 0, inputByteArr.Length);
                    cryptoStream.FlushFinalBlock();
                    Content = Encoding.UTF8.GetString(memoryStream.ToArray());
                    SecurityStatus = SecurityStatus.Unencrypted;
                }
                else throw new CannotDecryptException();
            }
            catch (Exception catchedException) { throw catchedException; }
        }
        /// <summary>
        /// 使用相同的密钥循环加密明文字符串。
        /// </summary>
        /// <param name="encryptKey">加密的密钥。</param>
        /// <returns>该操作将会返回一个明文经过循环加密得到的密文字符串。</returns>
        /// <exception cref="CannotEncryptException">当无法执行加密操作时，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当循环加密次数低于2的时候，则将会抛出这个异常。</exception>
        public virtual void CyclicEncrypt(string encryptKey)
        {
            if (Cycle >= 2)
            {
                for (int i = 0; i < Cycle; i++)
                {
                    if (CanEncrypt)
                    {
                        Encrypt(encryptKey);
                        SecurityStatus = SecurityStatus.Unencrypted;
                    }
                    else throw new CannotEncryptException();
                }
                SecurityStatus = SecurityStatus.Encrypted;
                IsCyclicEncrypt = true;
            }
            else throw new ArgumentOutOfRangeException("Cycle", "The number of cyclic encryption must be ≥ 2.");
        }
        /// <summary>
        /// 使用相同的密钥循环解密密文字符串。
        /// </summary>
        /// <param name="decryptKey">解密的密钥。</param>
        /// <returns>该操作将会返回一个密文经过循环解密得到的明文字符串。</returns>
        /// <exception cref="CannotDecryptException">当无法执行解密操作时，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当循环加密次数低于2的时候，则将会抛出这个异常。</exception>
        public virtual void CyclicDecrypt(string decryptKey)
        {
            if (Cycle >= 2)
            {
                for (int i = 0; i < Cycle; i++)
                {
                    if (CanDecrypt)
                    {
                        Decrypt(decryptKey);
                        SecurityStatus = SecurityStatus.Encrypted;
                    }
                    else throw new CannotDecryptException();
                }
                SecurityStatus = SecurityStatus.Unencrypted;
                IsCyclicEncrypt = false;
            }
            else throw new ArgumentOutOfRangeException(decryptKey, "The number of cyclic decryption must be ≥ 2.");
        }
        /// <summary>
        /// 返回这个实例包含的字符串。
        /// </summary>
        /// <returns>这个方法将会返回当前实例的BasicString属性包含的文本值。</returns>
        public override string ToString() => Content;
    }
}
