using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Models
{
    internal static class EntityConversionHelper
    {
        public static ICollection<Skill> ConvertSkillDTOsToSkills(ICollection<SkillDTO> skillDTOs)
        {
            var result = new List<Skill>();
            foreach (var skill in skillDTOs ?? Enumerable.Empty<SkillDTO>())
            {
                result.Add( new Skill() { Id = skill.Id, Name = skill.Name });
            }
            return result;
        }

        public static ICollection<SkillDTO> ConvertSkillsToSkillDTOs(ICollection<Skill> skills)
        {
            var result = new List<SkillDTO>();
            foreach (var skill in skills ?? Enumerable.Empty<Skill>())
            {
                result.Add( new SkillDTO() { Id = skill.Id, Name = skill.Name });
            }
            return result;
        }

        public static ICollection<Spark> ConvertSparkDTOsToSparks(ICollection<SparkDTO> sparkDTOs)
        {
            var result = new List<Spark>();
            foreach (var spark in sparkDTOs ?? Enumerable.Empty<SparkDTO>())
            {
                result.Add( new Spark() { ProjectId = spark.PId, UserId = spark.UId, CreatedDate = spark.CreatedDate, Status = spark.Status });
            }
            return result;
        }

        public static ICollection<SparkDTO> ConvertSparksToSparkDTOs(ICollection<Spark> sparks)
        {
            var result = new List<SparkDTO>();
            foreach (var spark in sparks ?? Enumerable.Empty<Spark>())
            {
                result.Add( new SparkDTO() { UId = spark.UserId, PId = spark.ProjectId, Status = spark.Status, CreatedDate = spark.CreatedDate });
            }
            return result;
        }
    }
}
