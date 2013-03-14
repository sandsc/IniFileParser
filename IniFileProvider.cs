/* ***********************************************
 * Copyright (c) 2009-2010 luoshasha. All rights reserved";
 * CLR version: 4.0.30319.296"
 * File name:   IniFileProvider.cs"
 * Date:        12/5/2012 11:44:15 AM
 * Author :  luoshasha(sand)
 * Email  :  luoshasha@foxmail.com
 * Description: 
	
 * History:  created by luoshasha(sand) 12/5/2012 11:44:15 AM
 
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace SandLib.Util
{
    /// <summary>
    /// a ini file provider can read and write ini files,
    /// support empty lines and comment.
    /// </summary>
    public class IniFileProvider
    {
        private readonly Regex _section_rex = new Regex(@"(?<=\[)(?<SectionName>[^\]]+)(?=\])");
        private readonly Regex _kv_rex = new Regex(@"(?<Key>[^=]+)=(?<Value>.+)");
        private readonly Regex _comment_rex = new Regex(@"^[;#]");
        private readonly Regex _empty_rex = new Regex(@"\s+");

        private List<Entry> _file_global_entries = new List<Entry>();
        private Dictionary<string, SectionEntry> _sections = new Dictionary<string, SectionEntry>();
        private string _filename;

        
        public IniFileProvider() : this(null) { }

        public IniFileProvider(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
                Load(filename);
        }

        /// <summary>
        /// gets current loaded ini file
        /// </summary>
        public string FileName
        {
            get { return _filename; }
        }

        /// <summary>
        /// Get a specific value from the .ini file
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <returns>The value of the given key in the given section, or NULL if not found</returns>
        public string GetValue(string sectionName, string key)
        {
            sectionName = sectionName.ToLower();
            key = key.ToLower();
            SectionEntry se = null;
            _sections.TryGetValue(sectionName, out se);
            if (se != null)
                return se.GetKVEntry(key).Value;
            return null;
        }

        /// <summary>
        /// Set a specific value in a section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string sectionName, string key, string value)
        {
            sectionName = sectionName.ToLower();
            key = key.ToLower();
            SectionEntry se = null;
            if (!_sections.TryGetValue(sectionName, out se))
            {
                se = new SectionEntry(sectionName);
                _sections[sectionName] = se;
                _file_global_entries.Add(se);
            }
            se.SetKeyValue(key, value);
        }

        /// <summary>
        /// Set an entire sections values
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="sectionValues"></param>
        public void SetSection(string sectionName, IDictionary<string, string> sectionValues)
        {
            sectionName = sectionName.ToLower();
            SectionEntry se = null;
            if (!_sections.TryGetValue(sectionName, out se))
            {
                se = new SectionEntry(sectionName);
                _sections[sectionName] = se;
                _file_global_entries.Add(se);
            }
            foreach (var kv in sectionValues)
            {
                string key = kv.Key.ToLower();
                se.SetKeyValue(key, kv.Value);
            }
        }

        /// <summary>
        /// load an ini file by default encoding
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Load(string filename)
        {
            return this.Load(filename, Encoding.Default);
        }

        /// <summary>
        /// Load an .INI File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Load(string filename, Encoding encoding)
        {
            _filename = filename;
            if (!File.Exists(filename))
                return false;
            try
            {
                string[] content = File.ReadAllLines(filename, encoding);
                SectionEntry current_se = null;
                foreach (var line in content)
                {
                    //match section
                    Match m = _section_rex.Match(line);
                    if (m.Success)
                    {
                        current_se = new SectionEntry(m.Groups[1].Value.Trim().ToLower());
                        _file_global_entries.Add(current_se);
                        _sections[current_se.SectionName] = current_se;
                        continue;
                    }

                    //match empty lines
                    m = _empty_rex.Match(line);
                    if (m.Success)
                    {
                        Entry entry = new TextLineEntry(EntryType.Empty, line);
                        if (current_se == null)
                            _file_global_entries.Add(entry);//global entry
                        else
                            current_se.AddEntry(entry);//sub entry
                        continue;
                    }
                   
                    //match comment
                    m = _comment_rex.Match(line);
                    if (m.Success)
                    {
                        Entry entry = new TextLineEntry(EntryType.Comment, line);
                        if (current_se == null)
                            _file_global_entries.Add(entry);//global entry
                        else
                            current_se.AddEntry(entry);//sub entry
                        continue;
                    }

                    m = _kv_rex.Match(line);
                    if (m.Success)
                    {
                        string key = m.Groups[1].Value.Trim().ToLower();
                        string value = m.Groups[2].Value.Trim();
                        //如果字符串值带有前后引号，则删除前后引号
                        if (value.StartsWith("\"") && value.EndsWith("\""))
                            value = value.Substring(1, value.Length - 2);
                        //must has a section
                        if (current_se != null)
                        {
                            current_se.SetKeyValue(key, value);
                        }
                        else
                        {
                            //or we consider it as a unknown line
                            Entry unknown = new TextLineEntry(EntryType.Unknown, line);
                            _file_global_entries.Add(unknown);
                        }
                        continue;
                    }

                    //at this point all matches are failed
                    //create a unknown entry
                    Entry unknown_entry = new TextLineEntry(EntryType.Unknown, line);
                    if (current_se != null)
                        current_se.AddEntry(unknown_entry);
                    else
                        _file_global_entries.Add(unknown_entry);
                    
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// save back to loaded file
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return Save(_filename);
        }
        
        /// <summary>
        /// save to an ini file using a default ini
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Save(string filename)
        {
            return Save(filename, Encoding.Default);
        }

        /// <summary>
        /// Save the content of this class to an INI File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Save(string filename, Encoding encoding)
        {
            var sb = new StringBuilder();
            int entry_count = _file_global_entries.Count;
            for (int i = 0; i < entry_count; ++i)
            {
                sb.AppendLine(_file_global_entries[i].GetEntryLines());
            }
            try
            {
                File.WriteAllText(filename, sb.ToString(), encoding);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get a specific value
        /// </summary>
        /// <param name="key">format:sectionMane:keyName</param>
        /// <returns></returns>
        private string this[string key]
        {
            get
            {
                string text1 = null;
                try
                {
                    int separatorIndex = key.IndexOf(':');
                    string sectionName = key.Substring(0, separatorIndex).Trim().ToLower();
                    string keyName = key.Substring(separatorIndex + 1).Trim().ToLower();
                    text1 = this.GetValue(sectionName, keyName);
                }
                catch
                {
                }
                return text1;
            }
        }

        /// <summary>
        /// gets all section names
        /// </summary>
        public IEnumerable<string> SectionNames
        {
            get { return _sections.Keys; }
        }

        /// <summary>
        /// gets number of sections
        /// </summary>
        public int SectionCount
        {
            get { return _sections.Count; }
        }

        /// <summary>
        /// gets all file entry count
        /// </summary>
        public int EntryCount
        {
            get { return _file_global_entries.Count; }
        }

        /// <summary>
        /// gets entry by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Entry GetEntry(int index)
        {
            return _file_global_entries[index];
        }

        public string GetString(string key, string defaultValue)
        {
            string retValue = this[key];
            if (retValue != null)
                return retValue;
            else
                return defaultValue;
        }

        public object Get(string key, Type type)
        {
            if (key == null || type == null)
            {
                throw new ArgumentNullException();
            }
            if (type == typeof(string))
            {
                return this[key];
            }
            if (type == typeof(int))
            {
                return this.GetInt(key, 0);
            }
            if (type == typeof(float))
            {
                return this.GetSingle(key, 0.0f);
            }
            if (type == typeof(double))
            {
                return this.GetDouble(key, 0.0);
            }
            if (type == typeof(bool))
            {
                return this.GetBoolean(key, false);
            }
            if (type == typeof(DateTime))
            {
                return this.GetDateTime(key, DateTime.MinValue);
            }
            if (type == typeof(TimeSpan))
            {
                return this.GetTimeSpan(key, TimeSpan.MinValue);
            }
            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(type, this[key]);
                }
                catch { }
            }
            return null;
        }

        public int GetInt(string key, int defaultValue)
        {
            try
            {
                return int.Parse(this[key]);
            }
            catch { }
            return defaultValue;
        }

        public Single GetSingle(string key, float defaultValue)
        {
            try
            {
                return Single.Parse(this[key]);
            }
            catch { }
            return defaultValue;
        }

        public double GetDouble(string key, double defaultValue)
        {
            try
            {
                return double.Parse(this[key]);
            }
            catch { }
            return defaultValue;
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            try
            {
                return bool.Parse(this[key]);
            }
            catch { }
            return defaultValue;
        }

        public DateTime GetDateTime(string key, DateTime defaultValue)
        {
            try
            {
                return DateTime.Parse(this[key].Trim());
            }
            catch
            { }
            return defaultValue;
        }
        public TimeSpan GetTimeSpan(string key, TimeSpan defaultValue)
        {
            try
            {
                return TimeSpan.Parse(this[key]);
            }
            catch
            { }
            return defaultValue;
        }
    }
}
