/* ***********************************************
 * Copyright (c) 2009-2010 luoshasha. All rights reserved";
 * CLR version: 4.0.30319.296"
 * File name:   KVEntry.cs"
 * Date:        12/5/2012 11:53:49 AM
 * Author :  luoshasha(sand)
 * Email  :  luoshasha@foxmail.com
 * Description: 
	
 * History:  created by luoshasha(sand) 12/5/2012 11:53:49 AM
 
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandLib.Util
{
    public class KVEntry : Entry
    {
        private string _key;
        private string _value;

        public readonly static KVEntry EmptyEntry = new KVEntry(string.Empty, string.Empty);

        public KVEntry(string key, string value)
            : base(EntryType.KeyValue)
        {
            _key = key;
            _value = value;
        }

        public string Key
        {
            get { return _key; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string GetEntryLines()
        {
            return string.Format("{0}={1}", _key, _value);
        }
    }
}
