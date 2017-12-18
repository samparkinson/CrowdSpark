using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.App.Models
{
    public interface IAttachmentAPI : IDisposable
    {
        Task<AttachmentDTO> Get(int attachmentId);

        Task<IReadOnlyCollection<AttachmentDTO>> GetForProject(int projectId);

        Task<int> Create(AttachmentCreateDTO attachment);

        Task<bool> Update(AttachmentDTO attachment);
    }
}
