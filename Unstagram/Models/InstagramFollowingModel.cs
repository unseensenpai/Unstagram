using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unstagram.Models.Following
{
    public class InstagramFollowingModel
    {
        public List<Relationships_Following> Relationships_Following { get; set; }
    }

    public class Relationships_Following
    {
        public string Title { get; set; }
        public List<object> Media_List_Data { get; set; }
        public List<String_List_Data> String_List_Data { get; set; }
    }

    public class String_List_Data
    {
        public string Href { get; set; }
        public string Value { get; set; }
        public int Timestamp { get; set; }
    }
}
