﻿using CrowdSpark.Common;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrowdSpark.App.Models
{
    public interface IProjectAPI : IDisposable
    {
        Task<IReadOnlyCollection<ProjectDTO>> GetAll();

        Task<IReadOnlyCollection<CategoryCreateDTO>> GetAllCategories();

        Task<IReadOnlyCollection<ProjectDTO>> GetAllFollowed();

        Task<IReadOnlyCollection<ProjectDTO>> GetAllSparked();

        Task<ProjectDTO> Get(int projectID);

        Task<bool> AddSkill(int projectID, string skill);
    }
}
