using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSpark.Entitites
{
    public class UserSkill
    {
        public int SkillId { get; set; }

        public int UserId { get; set; }

        public Skill Skill { get; set; }

        public User User { get; set; }
    }
}
