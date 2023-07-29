using NewStreamSupporter.Data.Abstractions;
using NewStreamSupporter.Models;

namespace NewStreamSupporter.Data
{
    public class StreamerFollow : BaseModel
    {
        public string StreamerId { get; set; }
        public string FollowerId { get; set; }
        public Platform Platform { get; set; }

        public StreamerFollow(string streamerId, string followerId, Platform platform)
        {
            StreamerId = streamerId;
            FollowerId = followerId;
            Platform = platform;
        }
    }
}
