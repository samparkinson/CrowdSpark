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
using System.Data;
using System.Linq;

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
                Type = (int)AttachmentTypes.BITMAP,
                ProjectId = 1
            };

            var user = new User() {Id = 1, Firstname = "John", Surname = "Smith", AzureUId = "rfaweaw", Mail = "test@example.com"};
            var project = new Project() { Id = 1, Title = "Foo", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow};

            context.Projects.Add(project);
            context.SaveChanges();

            //SanityCheck
            Assert.NotNull(context.Projects.AsNoTracking().Where(p => p.Id == 1).First());

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
                Type = (int)AttachmentTypes.BITMAP,
                ProjectId = 1
            };

            var attachmentToCreate2 = new AttachmentCreateDTO
            {
                Description = "A second example attachment",
                Data = "fuvwygwiu gbuywgykawrfiluhwrilhuihgfwefaguygdchjbaeiuyxgciuyadhviu bwrhjdsiyeabfcuyuw wyadvfjcvyut3er78t2euabdcbeaiyc eqdcgfw",
                Type = (int)AttachmentTypes.PDF,
                ProjectId = 1
            };

            var user = new User() { Id = 1, Firstname = "John", Surname = "Smith", AzureUId = "rfaweaw", Mail = "test@example.com" };
            var project = new Project() { Id = 1, Title = "Foo", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };

            context.Projects.Add(project);
            context.SaveChanges();

            //SanityCheck
            Assert.NotNull(context.Projects.AsNoTracking().Where(p => p.Id == 1).First());

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

            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(0);
            contextMock.Setup(c => c.Attachments.Add(It.IsAny<Attachment>()));

            using (var repository = new AttachmentRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateAsync(attachmentToCreate));
            }
        }

        [Fact]
        public async void CreateAsync_GivenSaveChangesError_ReturnsDbUpdateException()
        {
            var attachmentToCreate = new AttachmentCreateDTO
            {
                Description = "An example attachment",
                Data = "fuvwygwiu gbuywgykaguygdchjbaeiuyxgciuyadhviu bwrhjdsiyeabfcuyuw wyadvfjcvyut3er78t2euabdcbeaiyc eqdcgfw",
                Type = (int)AttachmentTypes.BITMAP
            };

            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ThrowsAsync(new DataException("error"));
            contextMock.Setup(c => c.Attachments.Add(It.IsAny<Attachment>()));

            using (var repository = new AttachmentRepository(contextMock.Object))
            {
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateAsync(attachmentToCreate));
            }
        }

        [Fact]
        public async void DeleteAsync_GivenAttachmentExists_DeletesAttachmentAndReturnsSuccess()
        {
            var existingAttachment = new Attachment
            {
                Data = "sgivehfuihvuaeirhvuhrsuvinfi",
                Description = "Attachment",
                Type = (int)AttachmentTypes.BITMAP,
                ProjectId = 1
            };

            var user = new User() { Id = 1, Firstname = "John", Surname = "Smith", AzureUId = "rfaweaw", Mail = "test@example.com" };
            var project = new Project() { Id = 1, Title = "Foo", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };

            context.Projects.Add(project);
            var attachment = context.Attachments.Add(existingAttachment);
            context.SaveChanges();

            //SanityCheck
            Assert.NotNull(context.Attachments.Find(attachment.Entity.Id));

            using (var repository = new AttachmentRepository(context))
            {
                var success = await repository.DeleteAsync(attachment.Entity.Id);

                Assert.True(success);
                Assert.Null(context.Attachments.Find(attachment.Entity.Id));
            }
        }

        [Fact]
        public async void DeleteAsync_GivenSaveChangesError_ReturnFalse()
        {
            var contextMock = new Mock<ICrowdSparkContext>();

            contextMock.Setup(c => c.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(0);
            contextMock.Setup(c => c.Attachments.Remove(It.IsAny<Attachment>()));

            using (var repository = new AttachmentRepository(contextMock.Object))
            {
                 Assert.False( await repository.DeleteAsync(1));
            }
        }

        [Fact]
        public async void DeleteAsync_GivenAttachmentDoesNotExist_ReturnsFalse()
        { 
            //SanityCheck
            Assert.Null(context.Attachments.Find(1));

            using (var repository = new AttachmentRepository(context))
            {
                var success = await repository.DeleteAsync(1);

                Assert.False(success);
            }
        }
    }
}
