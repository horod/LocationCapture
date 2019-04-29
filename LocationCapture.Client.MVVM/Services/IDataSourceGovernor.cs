using LocationCapture.Enums;
using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public interface IDataSourceGovernor
    {
        Task ChooseDataSourceAsync();

        Task<DataSourceType> GetCurrentDataSourceTypeAsync();
    }
}
