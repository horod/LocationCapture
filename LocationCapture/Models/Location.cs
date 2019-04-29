using System.Collections.Generic;

namespace LocationCapture.Models
{
    public class Location : NotificationBase
    {
        public int Id { get; set; }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        public List<LocationSnapshot> LocationSnapshots { get; set; } = new List<LocationSnapshot>();
    }
}
