﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerSpeckUtilities
{
    /// <summary>
    ///     Source: http://bytes.com/topic/net/insights/797169-reading-parsing-ini-file-c
    /// </summary>
    public class IniParser
    {
        private readonly String _iniFilePath;
        private readonly Dictionary<SectionPair, SectionValue> _keyPairs = new Dictionary<SectionPair, SectionValue>();

        /// <summary>
        ///     Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name="iniPath">Full path to INI file.</param>
        public IniParser(String iniPath, Encoding encoding = null)
        {
            TextReader iniFile;
            String currentRoot = null;

            _iniFilePath = iniPath;

            if (!File.Exists(iniPath))
            {
                iniFile = new StringReader((String) iniPath.Clone());
                iniPath = Path.GetTempFileName();
                ConfigurationFile = iniPath;
            }
            else
            {
                iniFile = new StreamReader(iniPath, encoding ?? Encoding.Default);
                ConfigurationFile = iniPath;
            }

            try
            {
                string strLine = iniFile.ReadLine();
                var order = 0;
                while (strLine != null)
                {
                    strLine = strLine.Trim();

                    if (!strLine.StartsWith(";") && strLine.Length > 1)
                    {
                        if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                        {
                            currentRoot = strLine.Substring(1, strLine.Length - 2);
                            order = 0;
                        }
                        else
                        {
                            string[] keyPair = strLine.Split(new[] {'='}, 2);

                            SectionPair sectionPair;
                            String value = null;

                            sectionPair.Section = currentRoot;
                            sectionPair.Key = keyPair[0];

                            if (keyPair.Length > 1)
                                value = keyPair[1];

                            var v = new SectionValue { Value=value, Order = order++};
                            if (_keyPairs.ContainsKey(sectionPair))
                                _keyPairs[sectionPair] = v;
                            else
                                _keyPairs.Add(sectionPair, v);
                        }
                    }

                    strLine = iniFile.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                iniFile.Close();
            }
            /*}
            else
                throw new FileNotFoundException("Unable to locate " + iniPath);*/
        }

        public IniParser()
        {
        }

        public string ConfigurationFile { get; set; }

        /// <summary>
        ///     Returns the value for the given section, key pair.
        /// </summary>
        /// <param name="sectionName">Section name</param>
        /// <param name="settingName">Key name</param>
        /// <param name="order"></param>
        /// <param name="defaultValue">Default value</param>
        private SectionValue GetSetting(String sectionName, String settingName, String defaultValue = null)
        {
            var sectionPair = new SectionPair {Section = sectionName, Key = settingName};

            if (_keyPairs.ContainsKey(sectionPair))
            {
                return _keyPairs[sectionPair];
            }
            return new SectionValue{Value=defaultValue};
        }

        public IEnumerable<string> GetSections()
        {
            var o = new List<String>();
            foreach (SectionPair p in _keyPairs.Keys)
                if (!o.Contains(p.Section))
                    o.Add(p.Section);
            return o;
        }

        /// <summary>
        ///     Enumerates all lines for given section.
        /// </summary>
        /// <param name="sectionName">Section to enum.</param>
        public String[] EnumSection(String sectionName)
        {
            var tmpArray = new ArrayList();

            foreach (SectionPair pair in _keyPairs.Keys)
            {
                if (pair.Section == sectionName)
                    tmpArray.Add(pair.Key);
            }

            return (String[]) tmpArray.ToArray(typeof (String));
        }

        /// <summary>
        ///     Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        /// <param name="settingValue">Value of key.</param>
        public void AddSetting(String sectionName, String settingName, String settingValue)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;

            if (_keyPairs.ContainsKey(sectionPair))
                _keyPairs.Remove(sectionPair);

            _keyPairs.Add(sectionPair, new SectionValue{Value=settingValue});
        }

        /// <summary>
        ///     Adds or replaces a setting to the table to be saved with a null value.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void AddSetting(String sectionName, String settingName)
        {
            AddSetting(sectionName, settingName, null);
        }

        /// <summary>
        ///     Remove a setting.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void DeleteSetting(String sectionName, String settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;

            if (_keyPairs.ContainsKey(sectionPair))
                _keyPairs.Remove(sectionPair);
        }

        /// <summary>
        ///     Save settings to new file.
        /// </summary>
        /// <param name="newFilePath">New file path.</param>
        public void SaveSettings(String newFilePath)
        {
            var sections = new ArrayList();
            string strToSave = "";

            foreach (SectionPair sectionPair in _keyPairs.Keys)
            {
                if (!sections.Contains(sectionPair.Section))
                    sections.Add(sectionPair.Section);
            }

            foreach (object section in sections)
            {
                strToSave += ("[" + section + "]\r\n");

                foreach (SectionPair sectionPair in _keyPairs.Keys)
                {
                    if (sectionPair.Section == (string) section)
                    {
                        string tmpValue = _keyPairs[sectionPair].Value;

                        if (tmpValue != null)
                            tmpValue = "=" + tmpValue;

                        strToSave += (sectionPair.Key + tmpValue + "\r\n");
                    }
                }

                strToSave += "\r\n";
            }

            try
            {
                TextWriter tw = new StreamWriter(newFilePath);
                tw.Write(strToSave);
                tw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Save settings back to ini file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(_iniFilePath);
        }

        public bool GetSettingAsBoolean(string sectionName, string settingName, bool defaultValue)
        {
            return GetSettingAsInteger(sectionName, settingName, defaultValue ? 1 : 0) == 1;
        }

        public int GetSettingAsInteger(string sectionName, string settingName, int defaultValue)
        {
            var s = GetSetting(sectionName, settingName);
            int r;

            return int.TryParse(s, out r) ? r : defaultValue;
        }

        private struct SectionPair
        {
            public String Key,Section;
        }


        private struct SectionValue
        {
            public String Value;
            public int Order;
        }

        public int GetLine(string sectionName, string settingName, string defaultValue, out string returnValue)
        {
            var t = GetSetting(sectionName, settingName, defaultValue);
            returnValue = t.Value;
            return t.Order;
        }

        public String GetSetting(string sectionName, string settingName)
        {
            return GetSetting(sectionName, settingName, null).Value;
        }
    }
}