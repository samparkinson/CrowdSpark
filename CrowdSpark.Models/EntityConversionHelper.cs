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
        public static ICollection<UserSkill> ConvertSkillDTOsToUserSkills(ICollection<SkillDTO> skillDTOs, int userId)
        {
            var result = new List<UserSkill>();
            foreach (var skill in skillDTOs ?? Enumerable.Empty<SkillDTO>())
            {
                result.Add( new UserSkill() { UserId = userId, SkillId = skill.Id });
            }
            return result;
        }

        public static ICollection<ProjectSkill> ConvertSkillDTOsToProjectSkills(ICollection<SkillDTO> skillDTOs, int projectId)
        {
            var result = new List<ProjectSkill>();
            foreach (var skill in skillDTOs ?? Enumerable.Empty<SkillDTO>())
            {
                result.Add(new ProjectSkill() { ProjectId = projectId, SkillId = skill.Id });
            }
            return result;
        }

        public static ICollection<SkillDTO> ConvertSkillsToSkillDTOs(ICollection<UserSkill> skills)
        {
            var result = new List<SkillDTO>();
            foreach (var skill in skills ?? Enumerable.Empty<UserSkill>())
            {
                result.Add(new SkillDTO() { Id = skill.Skill.Id, Name = skill.Skill.Name });
            }
            return result;
        }

        public static ICollection<SkillDTO> ConvertSkillsToSkillDTOs(ICollection<ProjectSkill> skills)
        {
            var result = new List<SkillDTO>();
            foreach (var skill in skills ?? Enumerable.Empty<ProjectSkill>())
            {
                result.Add( new SkillDTO() { Id = skill.Skill.Id, Name = skill.Skill.Name });
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
