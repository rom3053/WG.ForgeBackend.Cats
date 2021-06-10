using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebApp7.Cats.BL.Helpers;
using WebApp7.Cats.DALL;
using NpgsqlTypes;
using System.Data;
using WebApp7.Cats.BL.ModelsDTO;

namespace WebApp7.Cats.BL
{
    public class CatsService
    {
        private readonly wg_forge_dbContext _context;
        public CatsService(wg_forge_dbContext context)
        {
            _context = context;
        }
        public async Task<List<CatsStat>> UpdateCatsStatAsync()
        {
            //var connString = "Host=localhost;Port=5432;Database=wg_forge_db;Username=wg_forge;Password=a42";

            await using var conn = new NpgsqlConnection(wg_forge_dbContext.connString);
            try
            {

                await conn.OpenAsync(); //DELETE FROM table_name
                await using (var cmd = new NpgsqlCommand("DELETE FROM cats_stat", conn))
                {
                    cmd.Prepare();
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                await using (var cmd = new NpgsqlCommand("INSERT INTO cats_stat (tail_length_mean,tail_length_median,tail_length_mode," +
                    "whiskers_length_mean,whiskers_length_median,whiskers_length_mode) values" +
                    " (:tail_length_mean,:tail_length_median,:tail_length_mode," +
                    ":whiskers_length_mean,:whiskers_length_median,:whiskers_length_mode)", conn))
                {

                    cmd.Parameters.Add(AddNpgsqlParameter<decimal>("tail_length_mean", NpgsqlDbType.Numeric,
                        MeanForCats(c => c.TailLength)));
                    cmd.Parameters.Add(AddNpgsqlParameter<int>("tail_length_median", NpgsqlDbType.Integer,
                        MedianCats(t => t.TailLength)));
                    cmd.Parameters.Add(AddNpgsqlParameter<int[]>("tail_length_mode", NpgsqlDbType.Array | NpgsqlDbType.Integer,
                        Bimodal(t => (int)t.TailLength).ToArray()));

                    cmd.Parameters.Add(AddNpgsqlParameter<decimal>("whiskers_length_mean", NpgsqlDbType.Numeric,
                        MeanForCats(w => w.WhiskersLength)));
                    cmd.Parameters.Add(AddNpgsqlParameter<int>("whiskers_length_median", NpgsqlDbType.Integer,
                        MedianCats(w => w.WhiskersLength)));
                    cmd.Parameters.Add(AddNpgsqlParameter<int[]>("whiskers_length_mode", NpgsqlDbType.Array | NpgsqlDbType.Integer,
                        Bimodal(w => (int)w.WhiskersLength).ToArray()));

                    cmd.Prepare();
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }

            //SELECT ALL CATS
            //await using (var cmd = new NpgsqlCommand("SELECT * FROM cats", conn))
            //await using (var reader = await cmd.ExecuteReaderAsync())
            //    while (await reader.ReadAsync())
            //        res.Add(new CatsDTO
            //        { Color = reader.GetFieldValue<CatColor>(1).GetAttribute<PgNameAttribute>().PgName,
            //            Name = reader.GetFieldValue<string>(0), TailLength = reader.GetInt32(2), WhiskersLength = reader.GetInt32(3) });
            return await GetCatsStat();
        }

        public async Task<List<CatsStat>> GetCatsStat()
        {
            List<CatsStat> res = new List<CatsStat>();
            await using var conn = new NpgsqlConnection(wg_forge_dbContext.connString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT * FROM cats_stat", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                res.Add(new CatsStat
                {
                    TailLengthMean = reader.GetDecimal(0),
                    TailLengthMedian = reader.GetInt32(1),
                    TailLengthMode = reader.GetFieldValue<int[]>(2),
                    WhiskersLengthMean = reader.GetDecimal(3),
                    WhiskersLengthMedian = reader.GetInt32(4),
                    WhiskersLengthMode = reader.GetFieldValue<int[]>(5)
                }

                );
            return res;
        }

        private static NpgsqlParameter AddNpgsqlParameter<T>(string parameterName, NpgsqlDbType type, T Value)
        {
            var parameter = new NpgsqlParameter(parameterName, type);
            parameter.Value = Value;
            return parameter;
        }

        public IQueryable<DALL.Cats> OrderBy(string property)
        {
            return _context.Cats.OrderBy(property);
        }
        public IQueryable<DALL.Cats> OrderByDescending(string property)
        {
            return _context.Cats.OrderByDescending(property);
        }
        public IQueryable<DALL.Cats> OrderBy(IQueryable<DALL.Cats> query, string property)
        {
            return query.OrderBy(property);
        }
        public IQueryable<DALL.Cats> OrderByDescending(IQueryable<DALL.Cats> query, string property)
        {
            return query.OrderByDescending(property);
        }
        public async Task<List<CatsDTO>> GetCats()
        {
            return await _context.Cats.Select(s => new CatsDTO(s.Name, s.TailLength, s.WhiskersLength, s.Color.ToString()))
                                                            .ToListAsync();
        }
        public List<CatColorsInfoDTO> GetColorsInfo()
        {
            var query = _context.CatColorsInfo.Select(c => new CatColorsInfoDTO(c.Color.ToString(),c.Count)).ToList();
            return query;
        }
        public List<CatColorsInfoDTO> CountAndAddOrRefreshCountData()
        {
            List<CatColorsInfoDTO> info = new List<CatColorsInfoDTO>();
            var allColours = Enum.GetValues(CatColor.None.GetType());
            foreach (CatColor colour in allColours)
            {
                if (colour != CatColor.None)
                {
                    int counter = CountCatsForColor(colour);
                    CatColorsInfo catInfo = AddCatColorInfo(colour, counter);
                    info.Add(new CatColorsInfoDTO(catInfo.Color.GetAttribute<PgNameAttribute>().PgName, catInfo.Count));
                }
            }

            return info;
        }
        public CatColorsInfo AddCatColorInfo(CatColor color, int count)
        {
            var s = _context.CatColorsInfo.Select(c => c).FirstOrDefault();
            CatColorsInfo catColorsInfo = new CatColorsInfo() { Color = color, Count = count };

            if (s != null)
            {
                s.Count = count;
                _context.CatColorsInfo.Update(s);
            }
            else if (s != null)
            {
                _context.CatColorsInfo.Add(catColorsInfo);
            }

            _context.SaveChanges();
            return catColorsInfo;
        }
        public int CountCatsForColor(CatColor color)
        {
            return _context.Cats.Where(s => s.Color == color).Count();
        }
        public decimal MeanForCats(Expression<Func<DALL.Cats, int?>> predicateForSum)
        {
            int roundToOneSymbolAfteDot = 1;

            var sumTailLenghts = (decimal)_context.Cats.Sum(predicateForSum);

            decimal numCats = _context.Cats.Count();

            return Mean(sumTailLenghts, numCats, roundToOneSymbolAfteDot);
        }
        public List<int> Bimodal(Expression<Func<DALL.Cats, int>> predicate)
        {
            var mode = _context.Cats.GroupBy(predicate).
                                        OrderByDescending(g => g.Count()).
                                        Select(g => g.Key).Take(2).OrderBy(s => s).ToList();

            return mode;
        }
        public int MedianCats(Expression<Func<DALL.Cats, int?>> predicate)
        {
            List<int?> list = _context.Cats.Select(predicate).OrderBy(s => s.Value).ToList();
            int median = (list.Count + 1) / 2;
            int medianValue = list[median].Value;
            return medianValue;
        }
        private static decimal Mean(decimal sumAllNumbers, decimal numberOfEntries, int roundToNSymbolsAfteDot = 1)
        {
            decimal mean = Decimal.Round(sumAllNumbers / numberOfEntries, roundToNSymbolsAfteDot);
            return mean;
        }
    }
}
