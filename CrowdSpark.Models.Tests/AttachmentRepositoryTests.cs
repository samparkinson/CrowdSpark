using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using FluentAssertions;
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
        public async void FindAsync_GivenAttachmentDoesNotExist_ReturnsNull()
        {
            //SanityCheck
            Assert.Empty(context.Categories.ToArray());

            using (var repo = new AttachmentRepository(context))
            {
                var result = await repo.FindAsync(1);

                Assert.Null(result);
            }
        }

        [Fact]
        public async void FindAsync_GivenAttachmentExists_ReturnsAttachment()
        {
            var attachment = new Attachment() { Id =1, Description = "Foo", Data = "thurfh", ProjectId = 1, Project = new Project() { Id = 1}, Type = (int)AttachmentTypes.PDF };

            var contextMock = new Mock<ICrowdSparkContext>();
            contextMock.Setup(c => c.Attachments.FindAsync(1)).ReturnsAsync(attachment);

            using (var repo = new AttachmentRepository(contextMock.Object))
            {
                var result = await repo.FindAsync(1);

                var expected = new AttachmentDTO() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Type = (int)AttachmentTypes.PDF };
                result.ShouldBeEquivalentTo(expected);
            }
        }

        [Fact]
        public async void ReadAsync_GivenAttachmentsExist_ReturnsAttachments()
        {
            var user = new User() { Id = 1, Firstname = "Bob", Surname = "Smith", AzureUId = "foo", Mail = "test@example.com" };
            var project = new Project() { Id = 1, Title = "Foo", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };

            var attachment1 = new Attachment() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Project = project, Type = (int)AttachmentTypes.PDF };
            var attachment2 = new Attachment() { Id = 2, Description = "Moo", Data = "thurfh", ProjectId = 1, Project = project, Type = (int)AttachmentTypes.PDF };

            context.Users.Add(user);
            context.Projects.Add(project);
            context.Attachments.Add(attachment1);
            context.Attachments.Add(attachment2);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(2, context.Attachments.ToArray().Count());

            using (var repo = new AttachmentRepository(context))
            {
                var results = await repo.ReadAsync();

                var expected1 = new AttachmentDTO() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Type = (int)AttachmentTypes.PDF };
                var expected2 = new AttachmentDTO() { Id = 2, Description = "Moo", Data = "thurfh", ProjectId = 1, Type = (int)AttachmentTypes.PDF };

                results.ToArray()[0].ShouldBeEquivalentTo(expected1);
                results.ToArray()[1].ShouldBeEquivalentTo(expected2);
                Assert.Equal(2, results.Count());
            }
        }

        [Fact]
        public async void UpdateAsync_GivenAttachmentExists_ReturnsTrue()
        {
            var user = new User() { Id = 1, Firstname = "Bob", Surname = "Smith", AzureUId = "foo", Mail = "test@example.com" };
            var project = new Project() { Id = 1, Title = "Foo", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };

            var attachment = new Attachment() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Project = project, Type = (int)AttachmentTypes.PDF };

            context.Users.Add(user);
            context.Projects.Add(project);
            context.Attachments.Add(attachment);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(1, context.Attachments.ToArray().Count());

            using (var repo = new AttachmentRepository(context))
            {
                var updatedAttachment = new AttachmentDTO() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Type = (int)AttachmentTypes.BITMAP };

                var result = await repo.UpdateAsync(updatedAttachment);

                var expectedInDB = new Attachment() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Project = project, Type = (int)AttachmentTypes.BITMAP };

                Assert.True(result);
                Assert.Equal(1, context.Attachments.ToArray().Count());

                var attachmentinDB = context.Attachments.First();
                attachmentinDB.ShouldBeEquivalentTo(expectedInDB);
            }
        }

        [Fact]
        public async void UpdateAsync_GivenAttachmentDoesNotExist_ReturnsFalse()
        {
            //SanityCheck
            Assert.Equal(0, context.Attachments.ToArray().Count());

            using (var repo = new AttachmentRepository(context))
            {
                var updatedAttachment = new AttachmentDTO() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Type = (int)AttachmentTypes.BITMAP };

                var result = await repo.UpdateAsync(updatedAttachment);

                Assert.False(result);
                Assert.Equal(0, context.Attachments.ToArray().Count());
            }
        }

        [Fact]
        public async void ReadAsync_GivenAttachmentsDoNotExist_ReturnsEmptyCollection()
        {
            //sanityCheck
            Assert.Empty(context.Attachments.ToArray());

            using (var repo = new AttachmentRepository(context))
            {
                var result = await repo.ReadAsync();

                Assert.Empty(result);
            }
        }

        [Fact]
        public async void ReadForProjectAsync_GivenAttachmentsExist_ReturnsAttachments()
        {
            var user = new User() { Id = 1, Firstname = "Bob", Surname = "Smith", AzureUId = "foo", Mail = "test@example.com" };
            var project1 = new Project() { Id = 1, Title = "Foo", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };
            var project2 = new Project() { Id = 2, Title = "Foo2", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };

            var attachment1 = new Attachment() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Project = project1, Type = (int)AttachmentTypes.PDF };
            var attachment2 = new Attachment() { Id = 2, Description = "Moo", Data = "thurfh", ProjectId = 1, Project = project1, Type = (int)AttachmentTypes.PDF };
            var attachment3 = new Attachment() { Id = 3, Description = "Moo", Data = "thurfh", ProjectId = 2, Project = project2, Type = (int)AttachmentTypes.PDF };

            context.Users.Add(user);
            context.Projects.Add(project1);
            context.Projects.Add(project2);
            context.Attachments.Add(attachment1);
            context.Attachments.Add(attachment2);
            context.Attachments.Add(attachment3);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(3, context.Attachments.ToArray().Count());


            using (var repo = new AttachmentRepository(context))
            {
                var results = await repo.ReadForProjectAsync(1);

                var expected1 = new AttachmentDTO() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Type = (int)AttachmentTypes.PDF };
                var expected2 = new AttachmentDTO() { Id = 2, Description = "Moo", Data = "thurfh", ProjectId = 1, Type = (int)AttachmentTypes.PDF };

                results.ToArray()[0].ShouldBeEquivalentTo(expected1);
                results.ToArray()[1].ShouldBeEquivalentTo(expected2);
                Assert.Equal(2, results.Count());
            }
        }

        [Fact]
        public async void ReadForProjectAsync_GivenAttachmentsDoNotExist_ReturnsEmptyCollection()
        {
            var user = new User() { Id = 1, Firstname = "Bob", Surname = "Smith", AzureUId = "foo", Mail = "test@example.com" };
            var project1 = new Project() { Id = 1, Title = "Foo", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };
            var project2 = new Project() { Id = 2, Title = "Foo2", Description = "Bar", Creator = user, CreatedDate = System.DateTime.UtcNow };

            var attachment1 = new Attachment() { Id = 1, Description = "Foo", Data = "thurfh", ProjectId = 1, Project = project1, Type = (int)AttachmentTypes.PDF };
            var attachment2 = new Attachment() { Id = 2, Description = "Moo", Data = "thurfh", ProjectId = 1, Project = project1, Type = (int)AttachmentTypes.PDF };
            var attachment3 = new Attachment() { Id = 3, Description = "Moo", Data = "thurfh", ProjectId = 2, Project = project2, Type = (int)AttachmentTypes.PDF };

            context.Users.Add(user);
            context.Projects.Add(project1);
            context.Projects.Add(project2);
            context.Attachments.Add(attachment1);
            context.Attachments.Add(attachment2);
            context.Attachments.Add(attachment3);
            context.SaveChanges();

            //SanityCheck
            Assert.Equal(3, context.Attachments.ToArray().Count());

            using (var repo = new AttachmentRepository(context))
            {
                var result = await repo.ReadForProjectAsync(3);

                Assert.Empty(result);
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
