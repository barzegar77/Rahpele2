using Microsoft.AspNetCore.Mvc.Rendering;
using NomadicMVC.ViewModels.Pagination;
using Rahpele.ViewModels;
using Rahpele.ViewModels.Location;

namespace Rahpele.Services.Interfaces
{
    public interface ILocationManager
    {
        Task<BaseFilterViewModel<ListCountriesForManageViewModel>> GetCountriesListForManageAsync(int pageIndex, int pageLength, string searchString);
        FunctionResult CreateCountry(ManageCountryViewModel model);

        Task<BaseFilterViewModel<ListProvincesForManageViewModel>> GetProvincesListForManageAsync(int pageIndex, int pageLength, string searchString);
        FunctionResult CreateProvince(ManageProvinceViewModel model);
        List<SelectListItem> GetListCountriesForSelectList();

        Task<BaseFilterViewModel<ListCitiesForManageViewModel>> GetCitiesListForManageAsync(int pageIndex, int pageLength, string searchString);
        FunctionResult CreateCity(ManageCityViewModel model);
        List<SelectListItem> GetListProvincesForSelectList();


        Task<BaseFilterViewModel<ListTownsForManageViewModel>> GetTownsListForManageAsync(int pageIndex, int pageLength, string searchString);
        FunctionResult CreateTown(ManageTownViewModel model);
        List<SelectListItem> GetListCitiesForSelectList();
    }
}
