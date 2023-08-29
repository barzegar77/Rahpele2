using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NomadicMVC.ViewModels.Pagination;
using Rahpele.Models;
using Rahpele.Models.Data;
using Rahpele.Services.Interfaces;
using Rahpele.ViewModels;
using Rahpele.ViewModels.Location;

namespace Rahpele.Services
{
    public class LocationManager : ILocationManager
    {
        private readonly ApplicationDbContext _context;

        public LocationManager(ApplicationDbContext context)
        {
           _context = context;
        }

        public async Task<BaseFilterViewModel<ListCountriesForManageViewModel>> GetCountriesListForManageAsync(int pageIndex, int pageLength, string searchString)
        {
            var result = new BaseFilterViewModel<ListCountriesForManageViewModel>();

            IQueryable<Country> countriesQuery = _context.Countries
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                countriesQuery = countriesQuery
                    .Where(x => x.Name.Contains(searchString));
            }

            int totalCountries = await countriesQuery.CountAsync();
            int filteredCountries = totalCountries;
            var startingRowId = (pageIndex - 1) * pageLength + 1;
            var filteredCountriesList = countriesQuery
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength)
                .AsEnumerable()
                .Select((x, index) => new ListCountriesForManageViewModel
                {
                    Id = x.Id,
                    RowId = startingRowId + index,
                    Name = x.Name,
                })
                .ToList();

            result.RecordsTotal = totalCountries;
            result.RecordsFiltered = filteredCountries;
            result.Entities = filteredCountriesList;

            return result;
        }


        public FunctionResult CreateCountry(ManageCountryViewModel model)
        {
            if(model != null)
            {
                Country country = new Country
                {
                    Name = model.Name,
                };
                _context.Countries.Add(country);
                _context.SaveChanges();
                return new FunctionResult(true, "کشور با موفقیت ایجاد شد");
            }
            return new FunctionResult(false, "خطا در برقراری ارتباط با دیتابیس");
        }


        public async Task<BaseFilterViewModel<ListProvincesForManageViewModel>> GetProvincesListForManageAsync(int pageIndex, int pageLength, string searchString)
        {
            var result = new BaseFilterViewModel<ListProvincesForManageViewModel>();

            IQueryable<Province> provincesQuery = _context.Provinces.Include(c => c.Country)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                provincesQuery = provincesQuery
                    .Where(x => x.Name.Contains(searchString));
            }

            int totalProvinces = await provincesQuery.CountAsync();
            int filteredProvinces = totalProvinces;
            var startingRowId = (pageIndex - 1) * pageLength + 1;
            var filteredProvincesList = provincesQuery
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength)
                .AsEnumerable()
                .Select((x, index) => new ListProvincesForManageViewModel
                {
                    Id = x.Id,
                    RowId = startingRowId + index,
                    Name = x.Name,
                    CountryName = x.Country.Name,
                })
                .ToList();

            result.RecordsTotal = totalProvinces;
            result.RecordsFiltered = filteredProvinces;
            result.Entities = filteredProvincesList;

            return result;
        }

        public FunctionResult CreateProvince(ManageProvinceViewModel model)
        {
            if (model != null)
            {
                Province province = new Province
                {
                    Name = model.Name,
                    CountryId = model.CountryId
                };
                _context.Provinces.Add(province);
                _context.SaveChanges();
                return new FunctionResult(true, "استان با موفقیت ایجاد شد");
            }
            return new FunctionResult(false, "خطا در برقراری ارتباط با دیتابیس");
        }

        public List<SelectListItem> GetListCountriesForSelectList()
        {
            return _context.Countries.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }


        public async Task<BaseFilterViewModel<ListCitiesForManageViewModel>> GetCitiesListForManageAsync(int pageIndex, int pageLength, string searchString)
        {
            var result = new BaseFilterViewModel<ListCitiesForManageViewModel>();

            IQueryable<City> citiesQuery = _context.Cities.Include(c => c.Province)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                citiesQuery = citiesQuery
                    .Where(x => x.Name.Contains(searchString));
            }

            int totalCities = await citiesQuery.CountAsync();
            int filteredCities = totalCities;
            var startingRowId = (pageIndex - 1) * pageLength + 1;
            var filteredCitiesList = citiesQuery
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength)
                .AsEnumerable()
                .Select((x, index) => new ListCitiesForManageViewModel
                {
                    Id = x.Id,
                    RowId = startingRowId + index,
                    Name = x.Name,
                    ProvinceName = x.Province.Name,
                })
                .ToList();

            result.RecordsTotal = totalCities;
            result.RecordsFiltered = filteredCities;
            result.Entities = filteredCitiesList;

            return result;
        }


        public FunctionResult CreateCity(ManageCityViewModel model)
        {
            if (model != null)
            {
                City city = new City
                {
                    Name = model.Name,
                    ProvinceId = model.ProvinceId
                };
                _context.Cities.Add(city);
                _context.SaveChanges();
                return new FunctionResult(true, "شهر با موفقیت ایجاد شد");
            }
            return new FunctionResult(false, "خطا در برقراری ارتباط با دیتابیس");
        }


        public List<SelectListItem> GetListProvincesForSelectList()
        {
            return _context.Provinces.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }






        public async Task<BaseFilterViewModel<ListTownsForManageViewModel>> GetTownsListForManageAsync(int pageIndex, int pageLength, string searchString)
        {
            var result = new BaseFilterViewModel<ListTownsForManageViewModel>();

            IQueryable<Town> townsQuery = _context.Towns.Include(c => c.City)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                townsQuery = townsQuery
                    .Where(x => x.Name.Contains(searchString));
            }

            int totalTowns = await townsQuery.CountAsync();
            int filteredTowns = totalTowns;
            var startingRowId = (pageIndex - 1) * pageLength + 1;
            var filteredTownsList = townsQuery
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength)
                .AsEnumerable()
                .Select((x, index) => new ListTownsForManageViewModel
                {
                    Id = x.Id,
                    RowId = startingRowId + index,
                    Name = x.Name,
                    CityName = x.City.Name,
                })
                .ToList();

            result.RecordsTotal = totalTowns;
            result.RecordsFiltered = filteredTowns;
            result.Entities = filteredTownsList;

            return result;
        }


        public FunctionResult CreateTown(ManageTownViewModel model)
        {
            if (model != null)
            {
                Town town = new Town
                {
                    Name = model.Name,
                    CityId = model.CityId
                };
                _context.Towns.Add(town);
                _context.SaveChanges();
                return new FunctionResult(true, "محله با موفقیت ایجاد شد");
            }
            return new FunctionResult(false, "خطا در برقراری ارتباط با دیتابیس");
        }


        public List<SelectListItem> GetListCitiesForSelectList()
        {
            return _context.Cities.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }






    }
}
