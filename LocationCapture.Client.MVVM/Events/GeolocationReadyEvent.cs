using LocationCapture.Models;
using Prism.Events;

namespace LocationCapture.Client.MVVM.Events
{
    public class GeolocationReadyEvent : PubSubEvent<LocationDescriptor>
    {
    }
}
