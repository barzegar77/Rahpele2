using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NomadicMVC.ViewModels.Pagination;
using Rahpele.Models;
using Rahpele.Models.Data;
using Rahpele.Services.Interfaces;
using Rahpele.ViewModels.ProductCategory;

namespace Rahpele.Services
{
    public class ProductCategoryManager : IProductCategoryManager
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryManager(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<BaseFilterViewModel<ListProductCategoriesForManageViewModel>> GetProductCategoriesForManageAsync(int pageIndex, int pageLength, string searchString)
        {
            var result = new BaseFilterViewModel<ListProductCategoriesForManageViewModel>();

            IQueryable<ProductCategory> categoriesQuery = _context.ProductCategories
                .Where(x => !x.IsDeleted.Value)
                .AsNoTracking()
                .Include(x => x.Parent);

            if (!string.IsNullOrEmpty(searchString))
            {
                categoriesQuery = categoriesQuery
                    .Where(x => x.Title.Contains(searchString) || (x.Parent != null && x.Parent.Title.Contains(searchString)));
            }

            int totalCategories = await categoriesQuery.CountAsync();
            int filteredCategories = totalCategories;
            var startingRowId = (pageIndex - 1) * pageLength + 1;
            var filteredCategoriesList =  categoriesQuery
                .OrderByDescending(x => x.CreateDate)
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength)
                .AsEnumerable()
                .Select((x, index) => new ListProductCategoriesForManageViewModel
                {
                    RowId = startingRowId + index,
                    IconName = x.IconName,
                    Id = x.Id,
                    ParentTitle = x.Parent != null ? x.Parent.Title : "---",
                    Title = x.Title
                })
                .ToList();

            result.RecordsTotal = totalCategories;
            result.RecordsFiltered = filteredCategories;
            result.Entities = filteredCategoriesList;

            return result;
        }




        public async Task<bool> CreateProductCategory(ManageProductCategoryViewModel model)
        {
            if (model != null)
            {
                ProductCategory ProductCategory = new ProductCategory
                {
                    CreateDate = DateTime.Now,
                    Title = model.Title,
                    Description = model.Description,
                    IconName = model.IconName,
                    IsDeleted = false,
                    ParentId = model.ParentId
                };
                await _context.ProductCategories.AddAsync(ProductCategory);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public ManageProductCategoryViewModel GetProductCategoryForUpdate(Guid id)
        {
            var category = _context.ProductCategories.Find(id);
            ManageProductCategoryViewModel model = new ManageProductCategoryViewModel
            {
                Description = category.Description,
                IconName = category.IconName,
                Id = category.Id,
                ParentId = category.ParentId,
                Title = category.Title
            };
            return model;
        }

        public async Task<bool> UpdateProductCategory(ManageProductCategoryViewModel model)
        {
            if (model != null)
            {
                var ProductCategory = await _context.ProductCategories.FirstOrDefaultAsync(x => x.Id == model.Id);
                ProductCategory.Title = model.Title;
                ProductCategory.Description = model.Description;
                ProductCategory.IconName = model.IconName;
                ProductCategory.ParentId = model.ParentId;
                _context.ProductCategories.Update(ProductCategory);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public List<SelectListItem> GetListMainMenusForSelectList()
        {
            return _context.ProductCategories
                .Where(x => x.ParentId  == null)
                .Select(x => new SelectListItem
            {
                Text = x.Title,
                Value =x.Id.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetListParentCategoriesForSelectList(Guid menuId)
        {
            return _context.ProductCategories
                .Where(x => x.ParentId == menuId)
                .Select(x => new SelectListItem
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                }).ToList();
        }

    }
}
