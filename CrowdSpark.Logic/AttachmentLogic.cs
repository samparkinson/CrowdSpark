using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public class AttachmentLogic : IAttachmentLogic
    {
        IAttachmentRepository _repository;

        public AttachmentLogic(IAttachmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<(ResponseLogic, int)> CreateAsync(AttachmentCreateDTO attachment)
        {
            var result = await _repository.CreateAsync(attachment);

            if (result > 0) return (ResponseLogic.SUCCESS, result);
            else return (ResponseLogic.ERROR_CREATING, 0);
        }

        public async Task<ResponseLogic> DeleteAsync(int attachmentId)
        {
            var attachment = await _repository.FindAsync(attachmentId);
            if (attachment is null) return ResponseLogic.NOT_FOUND;

            var success = await _repository.DeleteAsync(attachmentId);
            if (success) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<IEnumerable<AttachmentDTO>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<AttachmentDTO> GetAsync(int attachmentId)
        {
            return await _repository.FindAsync(attachmentId);
        }

        public async Task<IEnumerable<AttachmentDTO>> GetForProjectAsync(int projectId)
        {
            return await _repository.ReadForProjectAsync(projectId);
        }

        public async Task<ResponseLogic> UpdateAsync(AttachmentDTO attachment)
        {
            var existingAttachment = await _repository.FindAsync(attachment.Id);
            if (existingAttachment is null) return ResponseLogic.NOT_FOUND;

            existingAttachment.Data = attachment.Data;
            existingAttachment.Description = attachment.Data;
            existingAttachment.Type = attachment.Type;

            var success = await _repository.UpdateAsync(existingAttachment);
            if (success) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_UPDATING;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _repository.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
