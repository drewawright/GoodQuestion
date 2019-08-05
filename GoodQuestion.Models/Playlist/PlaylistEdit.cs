using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodQuestion.Models.Playlist
{
    public class PlaylistEdit
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public string TracksUrl { get; set; }
    }
}
