using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JsonManipulator
{
    public class User : IEquatable<User>
    {
        private List<User> _friendsList;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Index { get; set; }

        public string Id { get; set; }

        public HashSet<int> FriendsIndexs { get; set; }

        public bool Verified { get; set; }

        public int Groups { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get; set; }

        public int PostsNumber { get; set; }

        public DateTime CreationDate { get; set; }

        public int Age { get; set; }

        public Address Address { get; set; }

        [JsonIgnore]
        public int NumberOfFriends { get; private set; }

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

        public bool Equals(User other)
        {
            return other != null && Index == other.Index;
        }
    }
}
