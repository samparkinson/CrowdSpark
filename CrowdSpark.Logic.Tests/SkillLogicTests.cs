using System;
using System.Collections.Generic;
using System.Linq;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using CrowdSpark.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrowdSpark.Logic.Tests
{
    public class SkillLogicTests
    {
        Mock<ISkillRepository> skillRepositoryMock;
        SkillRepository skillRepository;
        CrowdSparkContext context;
        Mock<IUserRepository> userRepositoryMock;
        Mock<IProjectRepository> projectRepositoryMock;

        public SkillLogicTests()
        {
            skillRepositoryMock = new Mock<ISkillRepository>();
            projectRepositoryMock = new Mock<IProjectRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
        }

        public void Dispose()
        {
            DisposeContextIfExists();
        }

        void DisposeContextIfExists()
        {
            if (skillRepository != null)
            {
                context?.Database?.RollbackTransaction();
                skillRepository.Dispose();
            }
        }

        #region UnitTests

        [Fact]
        public async void CreateAsync_GivenValidSkill_ReturnsSUCCESS()
        {
            var skillToCreate = new SkillCreateDTO
            {
                Name = "Skill"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToCreate.Name)).ReturnsAsync(default(SkillDTO));
            skillRepositoryMock.Setup(c => c.CreateAsync(skillToCreate)).ReturnsAsync(1);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(skillToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToCreate.Name));
                skillRepositoryMock.Verify(c => c.CreateAsync(skillToCreate));
            }
        }

        [Fact]
        public async void CreateAsync_GivenSkillExists_ReturnsSUCCESS()
        {
            var skillToCreate = new SkillCreateDTO
            {
                Name = "Skill"
            };

            var existingSkill = new SkillDTO
            {
                Id = 1,
                Name = "Skill"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToCreate.Name)).ReturnsAsync(existingSkill);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(skillToCreate);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToCreate.Name));
                skillRepositoryMock.Verify(c => c.CreateAsync(It.IsAny<SkillCreateDTO>()), Times.Never());
            }
        }

        [Fact]
        public async void CreateAsync_GivenNothingCreated_ReturnsERROR_CREATING()
        {
            var skillToCreate = new SkillCreateDTO
            {
                Name = "Skill"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToCreate.Name)).ReturnsAsync(default(SkillDTO));
            skillRepositoryMock.Setup(c => c.CreateAsync(skillToCreate)).ReturnsAsync(0);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.CreateAsync(skillToCreate);

                Assert.Equal(ResponseLogic.ERROR_CREATING, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToCreate.Name));
                skillRepositoryMock.Verify(c => c.CreateAsync(skillToCreate));
            }
        }

        [Fact]
        public async void UpdateAsync_GivenSkillExists_ReturnsSuccess()
        {
            var skillToUpdate = new SkillDTO
            {
                Id = 1,
                Name = "Skill"
            };

            var skillToUpdateWithChanges = new SkillDTO
            {
                Id = 1,
                Name = "Skill123"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToUpdateWithChanges.Id)).ReturnsAsync(skillToUpdate);
            skillRepositoryMock.Setup(c => c.UpdateAsync(skillToUpdateWithChanges)).ReturnsAsync(true);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(skillToUpdateWithChanges);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToUpdateWithChanges.Id));
                skillRepositoryMock.Verify(c => c.UpdateAsync(skillToUpdateWithChanges));
            }
        }

        [Fact]
        public async void UpdateAsync_GivenSkillDoesNotExist_ReturnsNOT_FOUND()
        {
            var skillToUpdateWithChanges = new SkillDTO
            {
                Id = 1,
                Name = "Skill123"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToUpdateWithChanges.Id)).ReturnsAsync(default(SkillDTO));

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(skillToUpdateWithChanges);

                Assert.Equal(ResponseLogic.NOT_FOUND, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToUpdateWithChanges.Id));
                skillRepositoryMock.Verify(c => c.UpdateAsync(It.IsAny<SkillDTO>()), Times.Never());
            }
        }

        [Fact]
        public async void UpdateAsync_GivenErrorUpdating_ReturnsERROR_UPDATING()
        {
            var skillToUpdate = new SkillDTO
            {
                Id = 1,
                Name = "Skill"
            };

            var skillToUpdateWithChanges = new SkillDTO
            {
                Id = 1,
                Name = "Skill123"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToUpdateWithChanges.Id)).ReturnsAsync(skillToUpdate);
            skillRepositoryMock.Setup(c => c.UpdateAsync(skillToUpdateWithChanges)).ReturnsAsync(false);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.UpdateAsync(skillToUpdateWithChanges);

                Assert.Equal(ResponseLogic.ERROR_UPDATING, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToUpdateWithChanges.Id));
                skillRepositoryMock.Verify(c => c.UpdateAsync(skillToUpdateWithChanges));
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenSkillExistsAndInNoProjectsOrUsers_ReturnsSuccess()
        {
            var skillToDelete = new SkillDTO
            {
                Id = 1,
                Name = "Cooking"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToDelete.Id)).ReturnsAsync(skillToDelete);
            skillRepositoryMock.Setup(c => c.DeleteAsync(skillToDelete.Id)).ReturnsAsync(true);
            projectRepositoryMock.Setup(p => p.ReadDetailedAsync()).ReturnsAsync(new ProjectDTO[] { });
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(skillToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToDelete.Id));
                skillRepositoryMock.Verify(c => c.DeleteAsync(skillToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadDetailedAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenSkillExistsAndInOneProjectOrUser_ReturnsSuccess()
        {
            var skillToDelete = new SkillDTO
            {
                Id = 1,
                Name = "Cooking"
            };

            var projectsArray = new ProjectDTO[]
            {
                new ProjectDTO { Title = "Project1", Description = "Foo", Skills = new SkillDTO[] { skillToDelete} }
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToDelete.Id)).ReturnsAsync(skillToDelete);
            skillRepositoryMock.Setup(c => c.DeleteAsync(skillToDelete.Id)).ReturnsAsync(true);
            projectRepositoryMock.Setup(p => p.ReadDetailedAsync()).ReturnsAsync(projectsArray);
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(skillToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToDelete.Id));
                skillRepositoryMock.Verify(c => c.DeleteAsync(skillToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadDetailedAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenSkillExistsInMoreThanOneProjectAndUser_ReturnsSuccess()
        {
            var skillToDelete = new SkillDTO
            {
                Id = 1,
                Name = "Cooking"
            };

            var projectsArray = new ProjectDTO[]
            {
                new ProjectDTO { Title = "Project1", Description = "Foo", Skills = new SkillDTO[] { skillToDelete} },
                new ProjectDTO { Title = "Project2", Description = "Bar", Skills = new SkillDTO[] { skillToDelete} }
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToDelete.Id)).ReturnsAsync(skillToDelete);
            projectRepositoryMock.Setup(p => p.ReadDetailedAsync()).ReturnsAsync(projectsArray);
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(skillToDelete);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToDelete.Id));
                skillRepositoryMock.Verify(c => c.DeleteAsync(It.IsAny<int>()), Times.Never());
                projectRepositoryMock.Verify(p => p.ReadDetailedAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenDatabaseError_ReturnsERROR_DELETING()
        {
            var skillToDelete = new SkillDTO
            {
                Id = 1,
                Name = "Cooking"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToDelete.Id)).ReturnsAsync(skillToDelete);
            skillRepositoryMock.Setup(c => c.DeleteAsync(skillToDelete.Id)).ReturnsAsync(false);
            projectRepositoryMock.Setup(p => p.ReadDetailedAsync()).ReturnsAsync(new ProjectDTO[] { });
            userRepositoryMock.Setup(u => u.ReadAsync()).ReturnsAsync(new UserDTO[] { });

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(skillToDelete);

                Assert.Equal(ResponseLogic.ERROR_DELETING, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToDelete.Id));
                skillRepositoryMock.Verify(c => c.DeleteAsync(skillToDelete.Id));
                projectRepositoryMock.Verify(p => p.ReadDetailedAsync());
                userRepositoryMock.Verify(u => u.ReadAsync());
            }
        }

        [Fact]
        public async void RemoveWithObjectAsync_GivenSkillDoesNotExist_ReturnsNOT_FOUND()
        {
            var skillToDelete = new SkillDTO
            {
                Id = 1,
                Name = "Cooking"
            };

            skillRepositoryMock.Setup(l => l.FindAsync(skillToDelete.Id)).ReturnsAsync(default(SkillDTO));

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.RemoveWithObjectAsync(skillToDelete);

                Assert.Equal(ResponseLogic.NOT_FOUND, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToDelete.Id));
                skillRepositoryMock.Verify(c => c.DeleteAsync(It.IsAny<int>()), Times.Never());
                projectRepositoryMock.Verify(p => p.ReadDetailedAsync(), Times.Never());
                userRepositoryMock.Verify(u => u.ReadAsync(), Times.Never());
            }
        }

        [Fact]
        public async void DeleteAsync_GivenDatabaseError_ReturnsERROR_DELETING()
        {
            var skillToDelete = new SkillDTO
            {
                Id = 1,
                Name = "Cooking"
            };

            skillRepositoryMock.Setup(c => c.FindAsync(skillToDelete.Id)).ReturnsAsync(skillToDelete);
            skillRepositoryMock.Setup(c => c.DeleteAsync(skillToDelete.Id)).ReturnsAsync(false);

            using (var logic = new SkillLogic(skillRepositoryMock.Object, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.DeleteAsync(skillToDelete.Id);

                Assert.Equal(ResponseLogic.ERROR_DELETING, response);
                skillRepositoryMock.Verify(c => c.FindAsync(skillToDelete.Id));
                skillRepositoryMock.Verify(c => c.DeleteAsync(skillToDelete.Id));
            }
        }

        #endregion

        #region IntegreationTests

        [Fact]
        public async void SearchAsync_GivenMatchingSkills_ReturnsSkills()
        {
            var existingSkills = new Skill[]
            {
                new Skill() { Id = 1, Name = "Cooking" },
                new Skill() { Id = 2, Name = "Cooking Bacon"},
                new Skill() { Id = 3, Name = "Burning things while trying to cook"},
                new Skill() { Id = 4, Name = "Yoddeling"}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.AddRange(existingSkills);
            context.SaveChanges();

            //Sanity Check
            Assert.Equal(4, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.SearchAsync("cook");

                Assert.Equal(3, results.Count());
                Assert.Equal(existingSkills[0].Name, results.ToArray()[0].Name);
                Assert.Equal(existingSkills[1].Name, results.ToArray()[1].Name);
                Assert.Equal(existingSkills[2].Name, results.ToArray()[2].Name);
            }
        }

        [Fact]
        public async void SearchAsync_GivenNoSkillsInDB_ReturnsEmptyEnumerable()
        {
            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            //Sanity Check
            Assert.Equal(0, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.SearchAsync("cook");

                Assert.Equal(0, results.Count());
            }
        }

        [Fact]
        public async void FindExactAsync_GivenNoMatchingSkills_ReturnsEmptyEnumerable()
        {
            var existingSkills = new Skill[]
            {
                new Skill() { Id = 1, Name = "Cooking" },
                new Skill() { Id = 2, Name = "Cooking Bacon"},
                new Skill() { Id = 3, Name = "Burning things while trying to cook"},
                new Skill() { Id = 4, Name = "Yoddeling"}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.AddRange(existingSkills);
            context.SaveChanges();

            //Sanity Check
            Assert.Equal(4, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.FindExactAsync("cooking");

                Assert.Equal(existingSkills[0].Name, result.Name);
            }
        }

        [Fact]
        public async void FindExactAsync_GivenNoSkillsInDB_ReturnsNull()
        {
            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            //Sanity Check
            Assert.Equal(0, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.FindExactAsync("cook");

                Assert.Null(result);
            }
        }

        [Fact]
        public async void GetAsync_GivenSkillsInDb_ReturnsAllSkills()
        {
            var existingSkills = new Skill[]
            {
                new Skill() { Id = 1, Name = "Cooking" },
                new Skill() { Id = 2, Name = "Cooking Bacon"},
                new Skill() { Id = 3, Name = "Burning things while trying to cook"},
                new Skill() { Id = 4, Name = "Yoddeling"}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.AddRange(existingSkills);
            context.SaveChanges();

            //Sanity Check
            Assert.Equal(4, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.GetAsync();

                Assert.Equal(4, results.Count());
                Assert.Equal(existingSkills[0].Name, results.ToArray()[0].Name);
                Assert.Equal(existingSkills[1].Name, results.ToArray()[1].Name);
                Assert.Equal(existingSkills[2].Name, results.ToArray()[2].Name);
                Assert.Equal(existingSkills[3].Name, results.ToArray()[3].Name);
            }
        }

        [Fact]
        public async void GetAsync_GivenNoSkillsInDb_ReturnsEmptyEnumerable()
        {
            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            //Sanity Check
            Assert.Equal(0, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var results = await logic.GetAsync();

                Assert.Equal(0, results.Count());
            }
        }

        [Fact]
        public async void GetAsyncWithId_GivenSkillInDb_ReturnsSkill()
        {
            var existingSkills = new Skill[]
            {
                new Skill() { Id = 1, Name = "Cooking" },
                new Skill() { Id = 2, Name = "Cooking Bacon"},
                new Skill() { Id = 3, Name = "Burning things while trying to cook"},
                new Skill() { Id = 4, Name = "Yoddeling"}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.AddRange(existingSkills);
            context.SaveChanges();

            //Sanity Check
            Assert.Equal(4, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.GetAsync(1);

                Assert.Equal(1, result.Id);
                Assert.Equal(existingSkills[0].Name, result.Name);
            }
        }

        [Fact]
        public async void GetAsyncWithId_GivenSkillNotInDb_ReturnsNull()
        {
            var existingSkills = new Skill[]
            {
                new Skill() { Id = 1, Name = "Cooking" },
                new Skill() { Id = 2, Name = "Cooking Bacon"},
                new Skill() { Id = 3, Name = "Burning things while trying to cook"},
                new Skill() { Id = 4, Name = "Yoddeling"}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.AddRange(existingSkills);
            context.SaveChanges();

            //Sanity Check
            Assert.Equal(4, await context.Skills.CountAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var result = await logic.GetAsync(5);

                Assert.Null(result);
            }
        }

        [Fact]
        public async void DeleteAsync_GivenSkillExistsAndInNoProjects_ReturnsSuccess()
        {
            var skillToDelete = new Skill
            {
                Id = 1,
                Name = "Cooking"
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.Add(skillToDelete);
            context.SaveChanges();

            //Sanity Check
            Assert.NotNull(context.Skills.Find(skillToDelete.Id));
            Assert.Empty(await context.Projects.ToArrayAsync());

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.DeleteAsync(skillToDelete.Id);

                Assert.Equal(ResponseLogic.SUCCESS, response);
            }
        }

        [Fact]
        public async void DeleteAsync_GivenSkillExistsAndInProjectsAndUser_ReturnsSuccess()
        {
            var skillToDelete = new Skill
            {
                Id = 1,
                Name = "Category"
            };

            var creatorUser = new User() { Id = 1, Firstname = "First", Surname = "Sur", AzureUId = "Azure", Mail = "test@example.com", Skills = new List<Skill> { skillToDelete }, };

            var projects = new List<Project>
            {
                new Project{ Title = "Project1", Skills = new List<Skill> { skillToDelete }, CreatedDate = DateTime.Now, Description = "abcd", CreatorId = creatorUser.Id},
                new Project{ Title = "Project2", Skills = new List<Skill> { skillToDelete }, CreatedDate = DateTime.Now, Description = "abcd", CreatorId = creatorUser.Id}
            };

            skillRepository = new SkillRepository(setupContextForIntegrationTests());

            context.Skills.Add(skillToDelete);
            context.Users.Add(creatorUser);
            context.Projects.AddRange(projects);
            context.SaveChanges();

            //Sanity Check
            Assert.NotNull(context.Skills.Find(skillToDelete.Id));
            Assert.Equal(2, (await context.Projects.ToArrayAsync()).Length);

            using (var logic = new SkillLogic(skillRepository, userRepositoryMock.Object, projectRepositoryMock.Object))
            {
                var response = await logic.DeleteAsync(skillToDelete.Id);

                Assert.Equal(ResponseLogic.SUCCESS, response);
                foreach (var project in await context.Projects.ToArrayAsync())
                {
                    Assert.Empty(project.Skills);
                }
            }
        }

        private CrowdSparkContext setupContextForIntegrationTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<CrowdSparkContext>()
                .UseSqlite(connection);

            context = new CrowdSparkContext(builder.Options);
            context.Database.EnsureCreated();
            context.Database.BeginTransaction();

            return context;
        }

        #endregion

    }
}
