using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public interface IAttachmentLogic : IDisposable
    {
        Task<IEnumerable<AttachmentDTO>> GetAsync();

        Task<AttachmentDTO> GetAsync(int attachmentId);

        Task<IEnumerable<AttachmentDTO>> GetForProjectAsync(int projectId);

        Task<(ResponseLogic Outcome, int Id)> CreateAsync(AttachmentCreateDTO attachment);

        Task<ResponseLogic> UpdateAsync(AttachmentDTO attachment);

        Task<ResponseLogic> DeleteAsync(int attachmentId);
    }
}
