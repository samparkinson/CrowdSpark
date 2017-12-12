using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class SkillRepository : ISkillRepository
    {
        private readonly ICrowdSparkContext _context;

        public SkillRepository(ICrowdSparkContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Skill skill)
        {
            var skillToCreate = new Skill
            {
                Name = skill.Name
            };

            var existingSkill = await _context.Skills.Where(s => s.Name == skill.Name).FirstOrDefaultAsync();

            if (existingSkill != null)
            {
                throw new DbUpdateException("Skill already exists", (Exception)null);
            }

            _context.Skills.Add(skillToCreate);
            if (await saveContextChanges() > 0)
            {
                return skillToCreate.Id;
            }
            else throw new DbUpdateException("Error creating skill", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int skillId)
        {
            var skill = await _context.Skills.FindAsync(skillId);
            _context.Skills.Remove(skill);

            return ( await saveContextChanges() > 0 );
        }

        public async Task<Skill> FindAsync(int skillId)
        {
            return await _context.Skills.FindAsync(skillId);
        }

        public async Task<Skill> FindAsync(string skillName)
        {
            return await _context.Skills.Where(s => s.Name.ToLower() == skillName.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Skill>> FindWildcardAsync(string skillName)
        {
            return await _context.Skills.Where(s => s.Name.ToLower().Contains(skillName.ToLower())).ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<Skill>> ReadAsync()
        {
            return await _context.Skills.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Skill details)
        {
            var skillToUpdate = await _context.Skills.FindAsync(details.Id);
            _context.Skills.Update(skillToUpdate);

            skillToUpdate.Name = details.Name;

            return (await saveContextChanges() > 0);
        }

        async Task<int> saveContextChanges()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (System.Data.DataException e)
            {
                throw new DbUpdateException("Error modifying skill collection", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
