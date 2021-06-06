using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenQA.Selenium;

namespace WebsiteFinder
{
    /// <summary>
    /// Reads & writes data to a key-value structure, has an encryption feature
    /// </summary>
    public static class Values
    {
        private static readonly string keysFile = "keys.data";
        private static readonly string valuesFile = "values.data";

        /// <summary>
        /// Reads a value from a key-value structure
        /// </summary>
        /// <param name="key"></param>
        /// <param name="decrypt"></param>
        /// <returns>The value of the key passed as an argument</returns>
        public static string Get(string key, bool decrypt = false)
        {
            if (File.Exists(keysFile) && File.Exists(valuesFile))
            {
                string[] keys = File.ReadAllLines(keysFile);

                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == key)
                    {
                        if (decrypt == false)
                        {
                            return File.ReadAllLines(valuesFile)[i];
                        }
                        else
                        {
                            return File.ReadAllLines(valuesFile)[i].Decrypt();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Saves a value in a key-value structure
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encrypt"></param>
        public static void Set(string key, string value, bool encrypt = false)
        {
            if (File.Exists(keysFile) && File.Exists(valuesFile))
            {
                string[] keys = File.ReadAllLines(keysFile);
                string[] values = File.ReadAllLines(valuesFile);

                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == key)
                    {
                        if (encrypt == false)
                        {
                            values[i] = value;
                        }
                        else
                        {
                            values[i] = value.Crypt();
                        }

                        File.WriteAllLines(valuesFile, values);
                        return;
                    }
                }

                File.AppendAllLines(keysFile, new string[] { key });

                if (encrypt == false)
                {
                    File.AppendAllLines(valuesFile, new string[] { value });
                }
                else
                {
                    File.AppendAllLines(valuesFile, new string[] { value.Crypt() });
                }
            }
            else
            {
                File.AppendAllLines(keysFile, new string[] { key });

                if (encrypt == false)
                {
                    File.AppendAllLines(valuesFile, new string[] { value });
                }
                else
                {
                    File.AppendAllLines(valuesFile, new string[] { value.Crypt() });
                }
            }
        }
    }
}