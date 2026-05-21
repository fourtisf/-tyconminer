using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GoldMineTycoon.Core
{
    /// <summary>
    /// Encrypted local save. Writes to a file under persistentDataPath rather than
    /// PlayerPrefs so the payload can grow and so a reinstall-safe cloud backup can
    /// later push the same blob. AES key is obfuscated, not secret — it only deters
    /// casual save editing, not a determined attacker. Authoritative currency for
    /// IAP must still be validated server-side (see brief 9.1).
    /// </summary>
    public static class SaveSystem
    {
        private const string FileName = "save.dat";
        private const int CurrentVersion = 1;

        // Device-derived material keeps the same save bound loosely to the install.
        private static byte[] Key => SHA256.Create().ComputeHash(
            Encoding.UTF8.GetBytes("GMT_v1_" + SystemInfo.deviceUniqueIdentifier))[..32];

        private static string Path => System.IO.Path.Combine(Application.persistentDataPath, FileName);

        public static void Save(SaveData data)
        {
            try
            {
                data.version = CurrentVersion;
                data.savedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var json = JsonUtility.ToJson(data);
                File.WriteAllBytes(Path, Encrypt(json));
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Save failed: {e.Message}");
            }
        }

        public static SaveData Load()
        {
            if (!File.Exists(Path)) return new SaveData();

            try
            {
                var json = Decrypt(File.ReadAllBytes(Path));
                var data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
                return Migrate(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Load failed, starting fresh: {e.Message}");
                return new SaveData();
            }
        }

        public static bool HasSave() => File.Exists(Path);

        public static void Delete()
        {
            if (File.Exists(Path)) File.Delete(Path);
        }

        private static SaveData Migrate(SaveData data)
        {
            // Future schema migrations branch on data.version here.
            return data;
        }

        private static byte[] Encrypt(string plain)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var input = Encoding.UTF8.GetBytes(plain);
            var cipher = encryptor.TransformFinalBlock(input, 0, input.Length);

            var output = new byte[aes.IV.Length + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, output, 0, aes.IV.Length);
            Buffer.BlockCopy(cipher, 0, output, aes.IV.Length, cipher.Length);
            return output;
        }

        private static string Decrypt(byte[] payload)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            var iv = new byte[16];
            Buffer.BlockCopy(payload, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var plain = decryptor.TransformFinalBlock(payload, iv.Length, payload.Length - iv.Length);
            return Encoding.UTF8.GetString(plain);
        }
    }
}
