using System;
using Utils.IniConfig;
using UnityEngine;
using System.IO;
namespace Manager
{
    class ConfigManager
    {
        static IniFile _ini_file;
        static IniFile ini_file
        {
            get
            {
                return _ini_file;
            }
            set
            {
                _ini_file = value;
            }
        }
        String config_file_name = "config.ini";
        String config_file_dir;
        public ConfigManager()
        {
#if UNITY_EDITOR
            config_file_dir = ".";
#elif !UNITY_ANDROID
            config_file_dir = ".";
#elif UNITY_ANDROID
            config_file_dir=Application.persistentDataPath;
#else
            config_file_dir=".";
#endif
            if (ini_file == null)
            {
                if (!File.Exists(Path.Combine(config_file_dir, config_file_name)))
                {
                    File.Create(Path.Combine(config_file_dir, config_file_name));
                }
                ini_file = new IniFile(Path.Combine(config_file_dir, config_file_name));
            }
        }
        public string GetStringValue(String section, String key, String default_ = "")
        {
            if (ini_file == null)
            {
                return default_;
            }
            return ini_file.getValue(section, key, default_);
        }
        public int getIntValue(String section, String key, int default_ = -1)
        {
            if (ini_file == null)
            {
                return default_;
            }
            return ini_file.getValue(section, key, default_);
        }
        public void SetStringValue(String section, String key, String value)
        {
            if (ini_file == null)
            {
                return;
            }
            ini_file.setValue(section, key, value);
        }
        public void setIntValue(String section, String key, int value)
        {
            if (ini_file == null)
            {
                return;
            }
            ini_file.setValue(section, key, value);
        }
        public void Save()
        {
            if (ini_file == null)
            {
                return;
            }
            ini_file.Dispose();
            ini_file = new IniFile(Path.Combine(config_file_dir, config_file_name));
        }
    }
}