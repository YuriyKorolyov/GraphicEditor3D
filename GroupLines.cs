using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Paint3
{
    [Serializable]
    public class GroupLines
    {
        [JsonInclude]
        public List<int> group = new List<int>();
        [JsonInclude]
        public string Name;
    }
}
