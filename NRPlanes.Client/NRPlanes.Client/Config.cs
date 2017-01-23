﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace NRPlanes.Client
{
    public class Config
    {
        public static Config Default { get; set; }

        public bool IsFullScreen { get; set; }

        public int BufferWidth { get; set; }
        public int BufferHeight { get; set; }

        static Config()
        {
            try
            {
                InitFromConfig();
            }
            catch
            {
                // np
            }
        }

        public static void InitFromConfig()
        {
            Default = new Config()
            {
                IsFullScreen = Convert.ToBoolean(ReadValue("IsFullScreen", "true")),
                BufferWidth = Convert.ToInt32(ReadValue("BufferWidth", "800")),
                BufferHeight = Convert.ToInt32(ReadValue("BufferHeight", "600")),
            };
        }

        private static string ReadValue(string key, string defaultValue)
        {
            if (ConfigurationManager.AppSettings.Keys.OfType<string>().Contains(key))
            {
                return ConfigurationManager.AppSettings[key];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
