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
            if (await _context.SaveChangesAsync() > 0)
            {
                return attachmentToCreate.Id;
            }
            else throw new DbUpdateException("Error creating attachment", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int attachmentId)
        {
            var attachment = await _context.Attachments.FindAsync(attachmentId);
            _context.Attachments.Remove(attachment);

            return ( await _context.SaveChangesAsync() > 0 );
        }

        public async Task<Attachment> FindAsync(int attachmentId)
        {
            return await _context.Attachments.FindAsync(attachmentId);
        }

        public async Task<IReadOnlyCollection<Attachment>> ReadAsync()
        {
            return await _context.Attachments.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Attachment details)
        {
            var attachmentToUpdate = await _context.Attachments.FindAsync(details.Id);
            _context.Attachments.Update(attachmentToUpdate);

            attachmentToUpdate.Description = details.Description;
            attachmentToUpdate.Data = details.Data;
            attachmentToUpdate.Type = details.Type;

            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
