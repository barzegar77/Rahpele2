using Microsoft.AspNetCore.Mvc.Rendering;
using NomadicMVC.ViewModels.Pagination;
using Rahpele.ViewModels.ProductCategory;

namespace Rahpele.Services.Interfaces
{
    public interface IProductCategoryManager
    {
        Task<BaseFilterViewModel<ListProductCategoriesForManageViewModel>> GetProductCategoriesForManageAsync(int pageIndex, int pageLength, string searchString);
        Task<bool> CreateProductCategory(ManageProductCategoryViewModel model);
        ManageProductCategoryViewModel GetProductCategoryForUpdate(Guid id);
        Task<bool> UpdateProductCategory(ManageProductCategoryViewModel model);
        List<SelectListItem> GetListMainMenusForSelectList();
        List<SelectListItem> GetListParentCategoriesForSelectList(Guid menuId);

    }
}
