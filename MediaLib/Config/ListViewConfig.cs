using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLib.Config
{
    public class ListViewConfig : IConfig
    {
        public int ItemFont;
        public MediaType type;
        public class HeaderConfig
        {
            public string Text;
            public int Width;
            public string DataMap;
        }
        public List<HeaderConfig> Headers;
        public string configFilePath()
        {
            return "./Config/ListViewConfig.json";
        }
    }

   
}
