using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response
{
    public class PostHasMemberResponse
    {
        public PostResponse Post
        {
            get; set;
        }
        public List<string> MembersNotJoined { get; set; }
        public List<string> MembersJoined { get; set; }

    }
}
