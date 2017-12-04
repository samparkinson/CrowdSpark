using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface IAttachmentRepository : IDisposable
    {
        Task<int> CreateAsync(AttachmentCreateDTO attatchment);
     
        Task<Attachment> FindAsync(int attachmentId);

        Task<IReadOnlyCollection<Attachment>> ReadAsync();

        Task<bool> UpdateAsync(Attachment details);

        Task<bool> DeleteAsync(int attachmentId);
    }
}
