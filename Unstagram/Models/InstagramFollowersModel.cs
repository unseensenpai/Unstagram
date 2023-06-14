namespace Unstagram.Models.Follower
{

    public class InstagramFollowersModel
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


    //public class InstagramFollowersModel
    //{
    //    public string Title { get; set; } = string.Empty;
    //    public List<object> MediaDatas { get; set; } = new();
    //    public List<StringDataList> FollowersDatas { get; set; } = new();

    //}
    //public class StringDataList
    //{
    //    public string Href { get; set; } = string.Empty;
    //    public string Value { get; set; } = string.Empty;
    //    public int Timestamp { get; set; }
    //}
}
