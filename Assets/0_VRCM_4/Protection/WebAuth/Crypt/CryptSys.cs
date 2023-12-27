using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public static class CryptSys
{
    public static byte[] EncryptJson(string json)
    {
        try
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.Key = Convert.FromBase64String("6XyGh8ezNIE/QhnEG0Msw645yqJznjHTAAnRURebmUU=");
                myRijndael.IV = Convert.FromBase64String("Z+serdqDtathqoRAqXPHdg==");

                // to Base64
                string keyb64 = Convert.ToBase64String(myRijndael.Key);
                //Debug.Log(keyb64);

                string ivb64 = Convert.ToBase64String(myRijndael.IV);
                //Debug.Log(ivb64);

                byte[] encrypted = EncryptStringToBytes(json, myRijndael.Key, myRijndael.IV);
                return encrypted;
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static String DecryptJson(byte[] json)
    {
        try
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.Key = Convert.FromBase64String("6XyGh8ezNIE/QhnEG0Msw645yqJznjHTAAnRURebmUU=");
                myRijndael.IV = Convert.FromBase64String("Z+serdqDtathqoRAqXPHdg==");

                string decrypted = DecryptStringFromBytes(json, myRijndael.Key, myRijndael.IV);
                return decrypted;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;
        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}
