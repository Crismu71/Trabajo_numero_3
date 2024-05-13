using game_freak_pokemon_api.Data;
using game_freak_pokemon_api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace game_freak_pokemon_api.Controllers
{
    [Route("api/ability")]
    [ApiController]
    public class AbilityController : ControllerBase
    {
        private readonly IMongoCollection<Ability>? _ability;
        public AbilityController(MongoDbService mongoDbService)
        {
            _ability = mongoDbService.Database?.GetCollection<Ability>("ability");
        }


        [HttpGet]
        public async Task<IEnumerable<Ability>> Get()
        {
            return await _ability.Find(_ => true).ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Ability ability)
        {
            if (!Enum.IsDefined(typeof(Types), ability.Type))
                return BadRequest($"El tipo de movimiento'{ability.Name}' no es válido.");

            if (ability.Damage < 0 || ability.Damage > 40 || !ability.Damage.HasValue)
                return BadRequest("El tipo de movimiento debe de tener un daño entre 0 a 40 puntos.");

            await _ability.InsertOneAsync(ability);
            return Ok("La habilidad se ha creado correctamente.");
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] List<Ability> abilities)
        {
            foreach (var ability in abilities)
            {
                if (!Enum.IsDefined(typeof(Types), ability.Type))
                    return BadRequest($"El tipo de movimiento'{ability.Name}' no es válido.");

                if (ability.Damage < 0 || ability.Damage > 40 || !ability.Damage.HasValue)
                    return BadRequest("El tipo de movimiento debe de tener un daño entre 0 a 40 puntos.");
            }

            await _ability.InsertManyAsync(abilities);
            return Ok("Las habilidades se han creado correctamente.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Ability?>> Delete(string id)
        {
            var filter = Builders<Ability>.Filter.Eq(x => x.Id, id);
            var result = await _ability.DeleteOneAsync(filter);
            if (result.DeletedCount == 1)
                return Ok();
            return NotFound();
        }
    }
}
