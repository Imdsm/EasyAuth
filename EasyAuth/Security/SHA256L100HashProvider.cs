﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EasyAuth.Security
{
    public class SHA256L100HashProvider : HashProvider
    {
        private const int hashLoops = 100; // SHA256 L100
        private SHA256Managed hashAlgorithm = new SHA256Managed();

        public override int HashLength
        {
            get { return hashAlgorithm.HashSize; }
        }

        public override string GetSalt()
        {
            return GetSalt(SaltLength);
        }

        public override string GetSalt(int length)
        {
            var buffer = new byte[length];
            Random random = new Random(DateTime.Now.Millisecond);
            random.NextBytes(buffer);
            return BitConverter.ToString(buffer).Replace("-", "").ToLowerInvariant();
        }

        public override string GetHash(string data, string salt)
        {
            var combined = string.Format("{0}{1}", data, salt);
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = hashAlgorithm.ComputeHash(bytes);
            for (int i = 0; i < hashLoops - 1; i++)
                hash = hashAlgorithm.ComputeHash(hash);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}