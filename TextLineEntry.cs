/* ***********************************************
 * Copyright (c) 2009-2010 luoshasha. All rights reserved";
 * CLR version: 4.0.30319.296"
 * File name:   TextLineEntry.cs"
 * Date:        12/5/2012 11:49:57 AM
 * Author :  luoshasha(sand)
 * Email  :  luoshasha@foxmail.com
 * Description: 
	
 * History:  created by luoshasha(sand) 12/5/2012 11:49:57 AM
 
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandLib.Util
{
    /// <summary>
    /// a text line entry. e.g empty lines, comments
    /// </summary>
    public class TextLineEntry : Entry
    {
        private string _line;
        public TextLineEntry(EntryType type, string line) :
            base (type)
        {
            _line = line;
        }
        public override string GetEntryLines()
        {
            return _line;
        }
    }
}
