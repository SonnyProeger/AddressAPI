using System.Linq.Expressions;
using System.Reflection;
using AddressAPI;
using AddressAPI.Models;
using AddressAPI.Services;
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
            // Create an expression parameter for the Address type
            var parameter = Expression.Parameter(typeof(Address), "a");

            // Get all the string properties of the Address type and reate an aggregate expression that
            // combines them using the 'OR' operator
            var searchProperty = typeof(Address).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Aggregate((Expression)null, (agg, prop) =>
                {
                    // Create an expression for checking if the property value contains the search query
                    var propExpr = Expression.Property(parameter, prop);
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
                    var constExpr = Expression.Constant(query.Search);
                    var containsExpr = Expression.Call(propExpr, containsMethod, constExpr);

                    // Combine the expression with the previous expressions using the 'OR' operator
                    if (agg == null) return containsExpr;
                    return Expression.OrElse(agg, containsExpr);
                });

            // Create a lambda expression from the search property expression and apply it to the addresses query
            var searchLambda = Expression.Lambda<Func<Address, bool>>(searchProperty, parameter);
            addresses = addresses.Where(searchLambda);
        }

        // Apply sort order
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            // Get the property info for the sort field
            var propertyInfo = typeof(Address).GetProperty(query.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                // Create an expression for accessing the sort field
                var parameterExpression = Expression.Parameter(typeof(Address), "x");
                var propertyExpression = Expression.Property(parameterExpression, propertyInfo);
                var lambdaExpression = Expression.Lambda(propertyExpression, parameterExpression);

                // Apply the sort order to the addresses query
                addresses = query.SortDescending == false ?
                    Queryable.OrderBy(addresses, (dynamic)lambdaExpression) :
                    Queryable.OrderByDescending(addresses, (dynamic)lambdaExpression);
            }
        }

        // Return the list of addresses
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

    [HttpGet("distance")]
    public async Task<ActionResult<double>> CalculateDistance([FromQuery] int originId, [FromQuery] int destinationId)
    {
        var originAddress = await _context.Address.FindAsync(originId);
        var destinationAddress = await _context.Address.FindAsync(destinationId);

        if (originAddress == null || destinationAddress == null)
        {
            return NotFound("One or both of the provided address IDs were not found in the database.");
        }

        string FormatAddress(Address address)
        {
            return $"{address.StreetName} {address.HouseNumber}, {address.ZipCode}, {address.City}, {address.Country}";
        }

        var originAddressString = FormatAddress(originAddress);
        var destinationAddressString = FormatAddress(destinationAddress);

        var distanceCalculator = new DistanceCalculator();
        var distance = await distanceCalculator.CalculateDistance(originAddressString, destinationAddressString);

        return distance;
    }


}
