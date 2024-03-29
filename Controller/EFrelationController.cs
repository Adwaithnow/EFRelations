
using EFRelations.Data;
using EFRelations.DTOs;
using EFRelations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFRelations.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EFrelationController : ControllerBase
    {
        private readonly DataContext _context;
        public EFrelationController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<Character>> GetCharacterById(int id){
            var Character=await _context.Characters
            .Include(c=>c.Weapons)
            .Include(c=>c.Backpack)
            .Include(c=>c.Factions)
            .FirstOrDefaultAsync(c=>c.Id==id);
            return Ok(Character);
        }
        [HttpPost]
        public async Task<ActionResult<List<Character>>> CreateCharacter(CharacterCreateDTO request)
        {
            Character newCharacter = new Character
            {
                Name = request.Name,
            };
            Backpack backpack = new Backpack { Description = request.Backpack.Description, Character = newCharacter };
            List<Weapon> weapons = request.Weapons.Select(w => new Weapon { Name = w.Name, Character = newCharacter }).ToList();
            List<Faction> factions = request.Factions.Select(w => new Faction { Name = w.Name, Characters = new List<Character> { newCharacter } }).ToList();

            newCharacter.Backpack = backpack;
            newCharacter.Weapons = weapons;
            newCharacter.Factions = factions;

            _context.Characters.Add(newCharacter);
            await _context.SaveChangesAsync();
            return Ok(await _context.Characters.Include(c => c.Backpack).Include(c => c.Weapons).ToListAsync());
        }
    }
}