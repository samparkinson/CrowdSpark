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

            _context.Skills.Add(skillToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                return skillToCreate.Id;
            }
            else throw new DbUpdateException("Error creating skill", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int skillId)
        {
            var skill = await _context.Skills.FindAsync(skillId);
            _context.Skills.Remove(skill);

            return ( await _context.SaveChangesAsync() > 0 );
        }

        public async Task<Skill> FindAsync(int skillId)
        {
            return await _context.Skills.FindAsync(skillId);
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

            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
