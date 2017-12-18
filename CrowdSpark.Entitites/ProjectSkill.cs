using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSpark.Entitites
{
    public class ProjectSkill
    {
        public int SkillId { get; set; }

        public int ProjectId { get; set; }

        public Skill Skill { get; set; }

        public Project Project { get; set; }
    }
}
