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

        public async Task<int> CreateAsync(SkillCreateDTO skill)
        {
            var skillToCreate = new Skill
            {
                Name = skill.Name
            };

            var existingSkill = await _context.Skills.Where(s => s.Name == skill.Name).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<SkillDTO> FindAsync(int skillId)
        {
            var skill = await _context.Skills.AsNoTracking().Where(s => s.Id == skillId).FirstOrDefaultAsync();

            if (skill is null) return null;
            return new SkillDTO() { Id = skill.Id, Name = skill.Name };
        }

        public async Task<SkillDTO> FindAsync(string skillName)
        {
            var skill = await _context.Skills.AsNoTracking().Where(s => s.Name.ToLower() == skillName.ToLower()).FirstOrDefaultAsync();

            if (skill is null) return null;
            return new SkillDTO() { Id = skill.Id, Name = skill.Name };
        }

        public async Task<IEnumerable<SkillDTO>> FindWildcardAsync(string skillName)
        {
            return await _context.Skills.AsNoTracking().Where(s => s.Name.ToLower().Contains(skillName.ToLower()))
                .Select(s => new SkillDTO() { Id = s.Id, Name = s.Name })
                .ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<SkillDTO>> ReadAsync()
        {
            return await _context.Skills
                .Select(s => new SkillDTO() { Id = s.Id, Name = s.Name })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(SkillDTO details)
        {
            var skillToUpdate = await _context.Skills.FindAsync(details.Id);
            if (skillToUpdate == null) return false;
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

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
