using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Models.Tests
{
    public class AttachmentRepositoryTests
    {
        private readonly CrowdSparkContext context;       
     
        public AttachmentRepositoryTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<CrowdSparkContext>()
                .UseSqlite(connection);

            context = new CrowdSparkContext(builder.Options);
            context.Database.EnsureCreated();

            //SEED IN HERE IF YOU WANT

            context.Database.BeginTransaction();            
        }

        public void Dispose()
        {
            context.Database.RollbackTransaction();
            context.Dispose();
        }

        [Fact]
        public async void CreateAsync_GivenValidAttachment_ReturnsNewAttachmentId()
        {
            var attachmentToCreate = new AttachmentCreateDTO
            {
                Description = "An example attachment",
                Data = "fuvwygwiu gbuywgykaguygdchjbaeiuyxgciuyadhviu bwrhjdsiyeabfcuyuw wyadvfjcvyut3er78t2euabdcbeaiyc eqdcgfw",
                Type = (int)AttachmentTypes.BITMAP
            };

            using (var repository = new AttachmentRepository(context))
            {
                var id = await repository.CreateAsync(attachmentToCreate);
                Assert.Equal((await context.Attachments.FirstAsync()).Id, id);
            }
        }

        [Fact]
        public async void CreateAsync_GivenValidAttachments_ReturnsNewAttachmentsIds()
        {
            var attachmentToCreate1 = new AttachmentCreateDTO
            {
                Description = "An example attachment",
                Data = "fuvwygwiu gbuywgykaguygdchjbaeiuyxgciuyadhviu bwrhjdsiyeabfcuyuw wyadvfjcvyut3er78t2euabdcbeaiyc eqdcgfw",
                Type = (int)AttachmentTypes.BITMAP
            };

            var attachmentToCreate2 = new AttachmentCreateDTO
            {
                Description = "A second example attachment",
                Data = "fuvwygwiu gbuywgykawrfiluhwrilhuihgfwefaguygdchjbaeiuyxgciuyadhviu bwrhjdsiyeabfcuyuw wyadvfjcvyut3er78t2euabdcbeaiyc eqdcgfw",
                Type = (int)AttachmentTypes.PDF
            };

            using (var repository = new AttachmentRepository(context))
            {
                var id1 = await repository.CreateAsync(attachmentToCreate1);
                var id2 = await repository.CreateAsync(attachmentToCreate2);

                Assert.Equal((await context.Attachments.FirstAsync()).Id, id1);
                Assert.Equal((await context.Attachments.ToArrayAsync())[1].Id, id2);
            }
        }

        [Fact]
        public async void CreateAsync_GivenDBDoesNotCreateAttachment_ReturnsDbUpdateException()
        {
            var attachmentToCreate = new AttachmentCreateDTO
            {
                Description = "An example attachment",
                Data = "fuvwygwiu gbuywgykaguygdchjbaeiuyxgciuyadhviu bwrhjdsiyeabfcuyuw wyadvfjcvyut3er78t2euabdcbeaiyc eqdcgfw",
                Type = (int)AttachmentTypes.BITMAP
            };

            //var contextMoq = new Moq
        }
    }
}
