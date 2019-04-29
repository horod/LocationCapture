using LocationCapture.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface ILocationDataImporter
    {
        Task ImportAsync(IEnumerable<Location> locationsToImport, CancellationToken cancellationToken);
    }
}
