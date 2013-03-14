/* ***********************************************
 * Copyright (c) 2009-2010 luoshasha. All rights reserved";
 * CLR version: 4.0.30319.296"
 * File name:   SectionEntry.cs"
 * Date:        12/5/2012 11:51:59 AM
 * Author :  luoshasha(sand)
 * Email  :  luoshasha@foxmail.com
 * Description: 
	
 * History:  created by luoshasha(sand) 12/5/2012 11:51:59 AM
 
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandLib.Util
{
    /// <summary>
    /// a section entry holds kv entry and empty lines, comments lines etc.
    /// </summary>
    public class SectionEntry : Entry
    {
        private List<Entry> _subentries = new List<Entry>();
        private string _sec_name;
        private Dictionary<string, KVEntry> _kvs = new Dictionary<string, KVEntry>();

        public SectionEntry(string secname)
            : base(EntryType.Section)
        {
            _sec_name = secname;
        }

        public string SectionName
        {
            get { return _sec_name; }
        }

        public void AddEntry(Entry entry)
        {
            if (entry.Type == EntryType.Section)
                throw new InvalidOperationException("section entry can't contain another section!");
            _subentries.Add(entry);
        }

        public void SetKeyValue(string key, string value)
        {
            if (_kvs.ContainsKey(key))
            {
                _kvs[key].Value = value;
            }
            else
            {
                //make a new entry
                KVEntry kv = new KVEntry(key, value);
                this.AddEntry(kv);
                _kvs[key] = kv;
            }
        }

        public KVEntry GetKVEntry(string key)
        {
            KVEntry kv = null;
            _kvs.TryGetValue(key, out kv);
            return kv;
        }
        /// <summary>
        /// build entry lines 
        /// </summary>
        /// <returns></returns>
        public override string GetEntryLines()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}]", _sec_name);
            sb.Append("\r\n");
            for (int i = 0; i < _subentries.Count; ++i)
            {
                string sublines = _subentries[i].GetEntryLines();
                sb.AppendLine(sublines);
            }
            return sb.ToString();
        }
    }
}
