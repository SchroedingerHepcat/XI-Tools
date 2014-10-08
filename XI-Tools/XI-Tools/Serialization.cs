
/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*////////////////////////////////////////////////////////////////////

using FFACETools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ZeroLimits.XITools
{
    /// <summary>
    /// This class is essential the code graveyard. Any code that I would rather not dispose
    /// of because I found use for it in the past ends up here.
    /// </summary>
    public static class Serialization
    {
        public static void Serialize<T>(string filename, T value)
        {
            using (Stream fStream = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(fStream, value);
            }
        }

        public static T Deserialize<T>(string filename, T value)
        {
                if (System.IO.File.Exists(filename))
                {
                    using (Stream fStream = new FileStream(filename,
                        FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        XmlSerializer xmlDeserializer = new XmlSerializer(typeof(T));
                        return (T) xmlDeserializer.Deserialize(fStream);
                    }
                }

            return value;
        }
    }
}
