using game_freak_pokemon_api.Data;
using game_freak_pokemon_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace game_freak_pokemon_api.Controllers
{
    [Route("api/pokemon")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IMongoCollection<Pokemon>? _pokemons;
        private readonly IMongoCollection<Ability>? _abilities;
        public PokemonController(MongoDbService mongoDbService) 
        {
            _pokemons = mongoDbService.Database?.GetCollection<Pokemon>("pokemon");
            _abilities = mongoDbService.Database?.GetCollection<Ability>("ability");
        }

        [HttpGet]
        public async Task<IEnumerable<Pokemon>> Get(string? type = null) 
        {
            if (string.IsNullOrEmpty(type))
                return await _pokemons.Find(_ => true).ToListAsync();
            
            var filter = Builders<Pokemon>.Filter.Eq(x => x.Type, type);
            return await _pokemons.Find(filter).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pokemon?>> GetById(string id) 
        {
            var filter = Builders<Pokemon>.Filter.Eq(x => x.Id, id);
            var pokemon = await _pokemons.Find(filter).FirstOrDefaultAsync();
            return pokemon == null ? NotFound(pokemon) : pokemon;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Pokemon pokemon)
        {
            if (pokemon.Abilities == null || pokemon.Abilities.Count != 4)
                return BadRequest("El Pokémon debe tener exactamente 4 habilidades.");

            foreach (var ability in pokemon.Abilities)
            {
                var existingAbility = await _abilities.Find(a => a.Id == ability.Id).FirstOrDefaultAsync();
                if (existingAbility == null)
                    return BadRequest($"No se encontró la habilidad con el ID '{ability?.Id}'.");

                ability.Name = existingAbility.Name;
                ability.Description = existingAbility.Description;
                ability.Type = existingAbility.Type;
                ability.Damage = existingAbility.Damage;
            }

            if (pokemon.Defense < 1 || pokemon.Defense > 30)
                return BadRequest("La defensa debe estar entre 1 y 30.");

            if (!Enum.IsDefined(typeof(Types), pokemon.Type))
                return BadRequest("El tipo del Pokémon no es válido.");

            await _pokemons.InsertOneAsync(pokemon);
            return Ok("El Pokémon se ha creado correctamente.");
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] List<Pokemon> pokemons)
        {
            foreach (var pokemon in pokemons)
            {
                if (pokemon.Abilities == null || pokemon.Abilities.Count != 4)
                    return BadRequest("Cada Pokémon debe tener exactamente 4 habilidades.");

                foreach (var ability in pokemon.Abilities)
                {
                    var existingAbility = await _abilities.Find(a => a.Id == ability.Id).FirstOrDefaultAsync();
                    if (existingAbility == null)
                        return BadRequest($"No se encontró la habilidad con el ID '{ability.Id}' para el Pokémon '{pokemon.Name}'.");

                    ability.Name = existingAbility.Name;
                    ability.Description = existingAbility.Description;
                    ability.Type = existingAbility.Type;
                    ability.Damage = existingAbility.Damage;
                }

                if (pokemon.Defense < 1 || pokemon.Defense > 30)
                    return BadRequest($"La defensa del Pokémon '{pokemon.Name}' debe estar entre 1 y 30.");

                if (!Enum.IsDefined(typeof(Types), pokemon.Type))
                    return BadRequest($"El tipo del Pokémon '{pokemon.Name}' no es válido.");
            }

            await _pokemons.InsertManyAsync(pokemons);
            return Ok("Los Pokémones se han creado correctamente.");
        }

        [HttpPut]
        public async Task<IActionResult> Update(Pokemon updatedPokemon)
        {

            var filter = Builders<Pokemon>.Filter.Eq(x => x.Id, updatedPokemon.Id);
            var existingPokemon = await _pokemons.Find(filter).FirstOrDefaultAsync();

            if (existingPokemon == null)
                return NotFound($"No se encontró ningún Pokémon con el ID '{updatedPokemon.Id}'.");

            var updateBuilder = Builders<Pokemon>.Update;
            var updateDefinitionList = new List<UpdateDefinition<Pokemon>>();

            if (!string.IsNullOrEmpty(updatedPokemon.Name))
                updateDefinitionList.Add(updateBuilder.Set(x => x.Name, updatedPokemon.Name));
            if (updatedPokemon.Type != null)
                updateDefinitionList.Add(updateBuilder.Set(x => x.Type, updatedPokemon.Type));
            if (updatedPokemon.HitPoints.HasValue)
                updateDefinitionList.Add(updateBuilder.Set(x => x.HitPoints, updatedPokemon.HitPoints));
            if (updatedPokemon.Defense.HasValue)
                updateDefinitionList.Add(updateBuilder.Set(x => x.Defense, updatedPokemon.Defense));
            if (updatedPokemon.Abilities != null)
                updateDefinitionList.Add(updateBuilder.Set(x => x.Abilities, updatedPokemon.Abilities));
           
            var combinedUpdateDefinition = Builders<Pokemon>.Update.Combine(updateDefinitionList);
            var result = await _pokemons.UpdateOneAsync(filter, combinedUpdateDefinition);

            if (result.ModifiedCount == 1)
                return Ok($"Se actualizó correctamente el Pokémon con ID '{updatedPokemon.Id}'.");

            return BadRequest($"No se pudo actualizar el Pokémon con ID '{updatedPokemon.Id}'.");
           
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Pokemon?>> Delete(string id)
        {
            var filter = Builders<Pokemon>.Filter.Eq(x => x.Id, id);
            var result = await _pokemons.DeleteOneAsync(filter);
            if (result.DeletedCount == 1)
                return Ok();
            return NotFound();
        }
    }
}
