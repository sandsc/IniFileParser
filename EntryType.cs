/* ***********************************************
 * Copyright (c) 2009-2010 luoshasha. All rights reserved";
 * CLR version: 4.0.30319.296"
 * File name:   LineType.cs"
 * Date:        12/5/2012 11:42:19 AM
 * Author :  luoshasha(sand)
 * Email  :  luoshasha@foxmail.com
 * Description: 
	
 * History:  created by luoshasha(sand) 12/5/2012 11:42:19 AM
 
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandLib.Util
{
    /// <summary>
    /// ini line semantic
    /// </summary>
    public enum EntryType
    {
        /// <summary>
        /// Unknown line
        /// </summary>
        Unknown,
        /// <summary>
        /// comment line
        /// </summary>
        Comment,
        /// <summary>
        /// empty line
        /// </summary>
        Empty,
        /// <summary>
        /// section line
        /// </summary>
        Section,
        /// <summary>
        /// kv line
        /// </summary>
        KeyValue
    }
}
