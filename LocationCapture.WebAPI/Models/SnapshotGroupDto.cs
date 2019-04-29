using System.Collections.Generic;

namespace LocationCapture.WebAPI.Models
{
    public class SnapshotGroupDto
    {
        public string Name { get; set; }
        public List<int> SnapshotIds { get; set; }
    }
}
