using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JsonManipulator
{
    public class User : IEquatable<User>
    {
        private List<User> _friendsList;

        [JsonProperty(nameof(Id))]
        public string Id { get; set; }

        public int Age { get; set; }

        [JsonIgnore]
        public List<User> FriendsList
        {
            get => _friendsList;
            set
            {
                _friendsList = value;
                NumberOfFriends = value.Count;
            }
        }

        [JsonProperty(nameof(Verified))]
        public bool Verified { get; set; }

        [JsonProperty(nameof(Groups))]
        public int Groups { get; set; }


        //[JsonProperty(nameof(FriendsIndexs))]
        [JsonProperty("FriendsIds")]
        public HashSet<int> FriendsIndexs { get; set; }

        [JsonIgnore]
        public int NumberOfFriends { get; set; }

        public string Gender { get; set; }

        public Address Address { get; set; }

        [JsonProperty(nameof(PostsNumber))]
        public int PostsNumber { get; set; }

        [JsonProperty(nameof(CreationDate))]
        public DateTime CreationDate { get; set; }

        public User()
        {
            FriendsList = new List<User>();
            //
        }

        [JsonProperty("Index")]
        public int Index { get; set; }

        public bool Equals(User other)
        {
            return other != null && Index == other.Index;
        }
    }
}
