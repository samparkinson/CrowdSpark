using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ICrowdSparkContext _context;

        public AttachmentRepository(ICrowdSparkContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(AttachmentCreateDTO attachment)
        {
            var attachmentToCreate = new Attachment
            {
                Description = attachment.Description,
                Data = attachment.Data,
                Type = attachment.Type
            };

            _context.Attachments.Add(attachmentToCreate);
            if (await saveContextChanges() > 0)
            {
                return attachmentToCreate.Id;
            }
            else throw new DbUpdateException("Error creating attachment", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int attachmentId)
        {
            var attachment = await _context.Attachments.FindAsync(attachmentId);

            if (attachment == null) return false;
            else _context.Attachments.Remove(attachment);

            return ( await saveContextChanges() > 0 );
        }

        public async Task<AttachmentDTO> FindAsync(int attachmentId)
        {
            var attachment =  await _context.Attachments.FindAsync(attachmentId);

            if (attachment is null) return null;

            return new AttachmentDTO()
            {
                Id = attachment.Id,
                Description = attachment.Description,
                Data = attachment.Data,
                Type = attachment.Type
            };
        }

        public async Task<IReadOnlyCollection<AttachmentDTO>> ReadAsync()
        {
            return await _context.Attachments
                            .Select(a => new AttachmentDTO()
                            {
                                Id = a.Id,
                                Description = a.Description,
                                Data = a.Data,
                                Type = a.Type
                            })
                            .ToArrayAsync();
        }

        public async Task<bool> UpdateAsync(AttachmentDTO details)
        {
            var attachmentToUpdate = await _context.Attachments.FindAsync(details.Id);
            _context.Attachments.Update(attachmentToUpdate);

            attachmentToUpdate.Description = details.Description;
            attachmentToUpdate.Data = details.Data;
            attachmentToUpdate.Type = details.Type;

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
                throw new DbUpdateException("Error modifying attachment collection", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
