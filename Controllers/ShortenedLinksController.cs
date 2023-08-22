using DevEncurtaUrl.API.Entities;
using DevEncurtaUrl.API.Models;
using DevEncurtaUrl.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DevEncurtaUrl.API.Controllers
{
    [ApiController]
    [Route("api/shortenedLinks")]
    public class ShortenedLinksController : ControllerBase
    {
        private readonly DevEncurtaUrlDbContext _context;
        public ShortenedLinksController(DevEncurtaUrlDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Log.Information("GetAll is called");
            return Ok(_context.Links);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var link = _context.Links.SingleOrDefault(x => x.Id == id);

            if (link == null) return NotFound();

            Log.Information("GetById is called");

            return Ok(link);
        }

        /// <summary>
        /// Cadastrar um link encurtado
        /// </summary>
        /// <remarks>
        /// {"title": "ultimo-artigo", "destinationLink":"https://github.com/gbrGabriel/DevEncurtaUrl.API"}
        /// </remarks>
        /// <param name="model">Dados do link</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Sucesso</response>
        [HttpPost]
        [ProducesResponseType(201)]
        public IActionResult Post(AddOrUpdateShortenedLinkModel model)
        {
            var link = new ShortenedCustomLink(model.Title, model.DestinationLink);

            _context.Links.Add(link);

            _context.SaveChanges();

            Log.Information("Post is called");

            return CreatedAtAction("GetById", new { id = link.Id }, link);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, AddOrUpdateShortenedLinkModel model)
        {
            var link = _context.Links.SingleOrDefault(x => x.Id == id);

            if (link == null) return NotFound();

            link.Update(model.Title, model.DestinationLink);

            _context.Links.Update(link);
            _context.SaveChanges();

            Log.Information("Put is called");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var link = _context.Links.SingleOrDefault(x => x.Id == id);

            if (link == null) return NotFound();

            _context.Links.Remove(link);
            _context.SaveChanges();

            Log.Information("Delete is called");

            return NoContent();
        }

        [HttpGet("/{code}")]
        public IActionResult RedirectLink(string code)
        {
            var link = _context.Links.SingleOrDefault(x => x.Code == code);

            if (link == null) return NotFound();

            Log.Information("RedirectLink is called");

            return Redirect(link.DestinationLink);
        }
    }
}
