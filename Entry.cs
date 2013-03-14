/* ***********************************************
 * Copyright (c) 2009-2010 luoshasha. All rights reserved";
 * CLR version: 4.0.30319.296"
 * File name:   Entry.cs"
 * Date:        12/5/2012 11:44:15 AM
 * Author :  luoshasha(sand)
 * Email  :  luoshasha@foxmail.com
 * Description: 
	
 * History:  created by luoshasha(sand) 12/5/2012 11:44:15 AM
 
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandLib.Util
{
    public abstract class Entry
    {
        private EntryType _type;

        public Entry(EntryType type)
        {
            _type = type;
        }

        public EntryType Type
        {
            get { return _type; }
        }

        public abstract string GetEntryLines();
    }
}
