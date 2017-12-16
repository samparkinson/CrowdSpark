using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface IAttachmentRepository : IDisposable
    {
        Task<int> CreateAsync(AttachmentCreateDTO attatchment);
     
        Task<AttachmentDTO> FindAsync(int attachmentId);

        Task<IReadOnlyCollection<AttachmentDTO>> ReadAsync();

        Task<bool> UpdateAsync(AttachmentDTO details);

        Task<bool> DeleteAsync(int attachmentId);
    }
}
