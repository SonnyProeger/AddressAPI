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
        // TODO make this dynamic
        if (!string.IsNullOrEmpty(query.Search))
        {
            addresses = addresses.Where(a =>
                a.StreetName.Contains(query.Search) ||
                a.HouseNumber.Contains(query.Search) ||
                a.ZipCode.Contains(query.Search) ||
                a.City.Contains(query.Search) ||
                a.Country.Contains(query.Search));
        }

        // Apply sort order
        // TODO method doesn't work
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var propertyInfo = typeof(Address).GetProperty(query.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                if (query.SortBy.ToLower() == "asc")
                {
                    addresses = addresses.OrderBy(a => propertyInfo.GetValue(a, null));
                }
                else if (query.SortBy.ToLower() == "desc")
                {
                    addresses = addresses.OrderByDescending(a => propertyInfo.GetValue(a, null));
                }
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
