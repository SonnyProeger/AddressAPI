using System.Linq.Expressions;
using System.Reflection;
using AddressAPI;
using AddressAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/addresses")]
public class AddressController : ControllerBase
{
    private readonly AddressContext _context;

    public AddressController(AddressContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Address>>> GetAddresses([FromQuery] AddressQuery query)

    {
        var addresses = _context.Address.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(query.Search))
        {
            var parameter = Expression.Parameter(typeof(Address), "a");
            var searchProperty = typeof(Address).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Aggregate((Expression)null, (agg, prop) =>
                {
                    var propExpr = Expression.Property(parameter, prop);
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
                    var constExpr = Expression.Constant(query.Search);
                    var containsExpr = Expression.Call(propExpr, containsMethod, constExpr);
                    if (agg == null) return containsExpr;
                    return Expression.OrElse(agg, containsExpr);
                });
            var searchLambda = Expression.Lambda<Func<Address, bool>>(searchProperty, parameter);
            addresses = addresses.Where(searchLambda);
        }



        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var propertyInfo = typeof(Address).GetProperty(query.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                var parameterExpression = Expression.Parameter(typeof(Address), "x");
                var propertyExpression = Expression.Property(parameterExpression, propertyInfo);
                var lambdaExpression = Expression.Lambda(propertyExpression, parameterExpression);

                addresses = query.SortDescending == false ?
                    Queryable.OrderBy(addresses, (dynamic)lambdaExpression) :
                    Queryable.OrderByDescending(addresses, (dynamic)lambdaExpression);
            }
        }



        return await addresses.ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> GetAddress(int id)
    {
        var address = await _context.Address.FindAsync(id);

        if (address == null)
        {
            return NotFound();
        }

        return address;
    }

    [HttpPost]
    public async Task<ActionResult<Address>> PostAddress(Address address)
    {
        _context.Address.Add(address);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAddress(int id, Address address)
    {
        if (id != address.Id)
        {
            return BadRequest();
        }

        _context.Entry(address).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var address = await _context.Address.FindAsync(id);

        if (address == null)
        {
            return NotFound();
        }

        _context.Address    .Remove(address);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
