using Dynamic2Db.Models;

namespace Dynamic2Db.Services
{
    public interface  IDataModelService
    {
        Task RefreshDataModel();
        Task<DataModel> GetDataModel(string key);
    }
}
