using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using WebApp7.Cats.BL;
using WebApp7.Cats.BL.Helpers;
using WebApp7.Cats.BL.ModelsDTO;
using WebApp7.Cats.DALL;
using WebApp7.Cats.Helpers;

namespace WebApp7.Cats.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatsController : Controller
    {
        private readonly wg_forge_dbContext _context;
        private readonly CatsService _catsService;

        public CatsController(wg_forge_dbContext context)
        {
            _context = context;
            _catsService = new CatsService(context);
        }

        [Route("/ping")]
        [HttpGet]
        public IActionResult ping()
        {
            return Ok("Cats Service.Version 0.1");
        }

        // GET: Cats
        [HttpGet("allcats")]
        public async Task<IActionResult> Index()
        {
            var cats = await _catsService.GetCats();

            return Ok(cats);

        }
        [HttpGet("catColorInfo")]
        public IActionResult CatColorsInfo()
        {
            //Count and Add or refrash data
            List<CatColorsInfoDTO> info = _catsService.CountAndAddOrRefreshCountData();

            //return Ok(_catsService.GetColorsInfo());
            return Ok(info);

        }
        [HttpGet("stats")]
        public async Task<List<CatsStat>> StatsAsync()
        {
            return await _catsService.GetCatsStat();
        }
        [HttpGet("stats/recount")]
        public async Task<List<CatsStat>> StatsRecountAsync()
        {
            return await _catsService.UpdateCatsStatAsync();
        }

        [HttpGet("sort")]
        public async Task<PaginatedList<DALL.Cats>> SortAsync(string? attribute = "name", string? order = "asc", int offset = 0, int limit = 10, int page = 1)
        {
            //int pageOffset = limit * (page - 1);
            //var query = _context.Cats.Skip(offset).Take(limit);
            Type myType = typeof(DALL.Cats);

            var yourString = attribute.ToLower();
            TextInfo info = new CultureInfo("en-US", false).TextInfo;
            yourString = info.ToTitleCase(yourString).Replace("_", " ").Replace(" ", string.Empty);

            PropertyInfo? propertyNameInfo = myType.GetProperty(yourString);
            IQueryable<DALL.Cats>? query = null;
            if (propertyNameInfo != null)
            {
                string propertyName = propertyNameInfo.Name;
                switch (order)
                {
                    case "asc":
                        query = _catsService.OrderBy(propertyName);
                        break;
                    case "desc":
                        query = _catsService.OrderByDescending(propertyName);
                        break;
                    default:
                        query = _catsService.OrderBy(propertyName);
                        break;
                }
            }
            return await PaginatedList<DALL.Cats>.CreateAsync(query, page, limit);
        }

        // POST: Cats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] DALL.Cats cat)
        {
            if (cat != null)
            {
                if (IsCorrectCat(cat))
                {
                    _context.Add(cat);
                    await _context.SaveChangesAsync();
                    return Ok(cat);
                }
            }
            return BadRequest("Incorrect information");
        }

        private bool IsCorrectCat(DALL.Cats cat)
        {
            return !IsNegative(cat) && IsUniqCat(cat) && IsColor(cat);
        }

        private bool IsColor(DALL.Cats cat)
        {
            var allColours = Enum.GetValues(CatColor.None.GetType());
            foreach (CatColor item in allColours)
            {
                if (item == cat.Color)
                {
                    return true;
                }
                //DoTO
                //if (item.GetAttribute<PgNameAttribute>().PgName == cat.Color.ToString())
                //{
                //    break;
                //}
            }
            return false;
        }

        private bool IsUniqCat(DALL.Cats cat)
        {
            var catChecked = _context.Cats.Where(c => c.Name == cat.Name).FirstOrDefault();
            if (catChecked == null)
            {
                return true;
            }
            return false;
        }

        private bool IsNegative(DALL.Cats cat)
        {
            if (cat.TailLength >= 0 && cat.WhiskersLength >= 0)
            {
                return false;
            }
            return true;
        }

        private bool CatsExists(string id)
        {
            return _context.Cats.Any(e => e.Name == id);
        }
    }
}
