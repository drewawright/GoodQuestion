using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodQuestion.Models.Song
{
    public class Edit
    {
        public string Name { get; set; }
        public string SongId { get; set; }
        public List<string> PlaylistNames { get; set; }
        public List<string> PlaylistIds { get; set; }
    }
}
