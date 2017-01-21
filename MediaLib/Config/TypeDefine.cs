using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaLib.IO;

namespace MediaLib
{
    namespace Config
    {

        public enum MediaType
        {
            Anime,
            Music
           
        }

        public interface IConfig
        {
             String configFilePath();
        }

        public interface IRootConfig: IConfig {
            MediaType type { get; }
            string name { get; set; }

            IConfigHelper getConfigHelper();

            IO.IMediaRootManager buildRootMangerFromConfig();
            bool valid();
        }

        public abstract class RootConfig<T>: IRootConfig,  IO.IMediaTravelConfig where T: Lib.Media
        {
            public string name { set; get; }
            public string dirName { set; get; }
            public MediaType type { get {
                    string prefix = typeof(T).ToString().Substring(13);
                    return (MediaType)Enum.Parse(typeof(MediaType), prefix);
                } }
            public Dictionary<String, T> mediaDic;
            public string configFilePath()
            {
                string  prefix = typeof(T).ToString().Substring(13);
                return "./UserData/"+prefix+"RootConfig.json";
            }

            public virtual  bool valid()
            {
                return name != null;              
            }
            public abstract MediaFileTraveler getFileTraveler();
            public abstract IConfigHelper getConfigHelper();

            public IMediaRootManager buildRootMangerFromConfig()
            {
                return new IO.MediaRootManager<T>(this);
            }
        }
      

        public class ListViewConfig:  IConfig
        {
            public int ItemFont;
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

        public enum ResourceType
        {
            LargeImage,
        }
        public class ResourceConfig: IConfig
        {
            public string resouceType;
            public string mediaType;
            public string storePath;
            public ResourceType resouceType_t { get { return (ResourceType)Enum.Parse(typeof(ResourceType), resouceType); } }
            public MediaType mediaType_t { get { return (MediaType)Enum.Parse(typeof(MediaType), mediaType); } }
            public string configFilePath()
            {
                return "./Config/ResourceConfig.json";
            }
        }

    }
}
