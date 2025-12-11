using AutoMapper;
using Common;
using Common.Enums;
using Data;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Services.Services;
using Services.Services.CMS.Programs;
using SharedModels.CustomMapping;
using SharedModels.Dtos.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Services.Tests
{
    public class ProgramServiceTests
    {
        private const int GymId = 1;
        private const int ManagerUserId = 1;
        private const int SecondaryManagerUserId = 2;
        private const int PracticeCategoryId = 100;
        private const int PracticeAId = 1000;
        private const int PracticeBId = 1001;
        private const int PracticeCId = 1002;
        private const int PaperFileId1 = 2000;
        private const int PaperFileId2 = 2001;
        private const string DefaultNote = "نکات تمرین";

        private readonly IMapper _mapper;

        public ProgramServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddCustomMappingProfile(
                    typeof(ProgramDto).Assembly,
                    typeof(Entities.Program).Assembly);
            });
            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task CreateAsync_CreatesSupersetAndSingleRoutineItems()
        {
            using var context = CreateContext(nameof(CreateAsync_CreatesSupersetAndSingleRoutineItems));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = BuildDefaultProgramDto();

            var response = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);

            Assert.True(response.IsSuccess, response.Message);
            var program = await context.Set<Entities.Program>()
                .Include(p => p.ProgramRoutineItems)
                    .ThenInclude(ri => ri.ProgramPractices)
                .SingleAsync();

            Assert.Equal(2, program.ProgramRoutineItems.Count);
            Assert.Equal(3, program.CountOfPractice);
            Assert.Equal(DefaultNote, program.Note);

            var superset = program.ProgramRoutineItems.Single(ri => ri.ItemType == ProgramRoutineItemType.Superset);
            Assert.Equal("Upper Body Blast", superset.Title);
            Assert.Equal(2, superset.ProgramPractices.Count);
            Assert.Equal(new[] { 1, 2 }, superset.ProgramPractices.OrderBy(pp => pp.InternalOrder).Select(pp => pp.InternalOrder));

            var single = program.ProgramRoutineItems.Single(ri => ri.ItemType == ProgramRoutineItemType.Single);
            Assert.Single(single.ProgramPractices);
            Assert.Equal(2, single.DisplayOrder);
        }

        [Fact]
        public async Task CreateAsync_CreatesPaperProgramWithFiles()
        {
            using var context = CreateContext(nameof(CreateAsync_CreatesPaperProgramWithFiles));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = new ProgramDto
            {
                Title = "Paper Program",
                Type = ProgramTypes.Paper,
                PaperFileIds = new List<int> { PaperFileId1, PaperFileId2 }
            };

            var response = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);

            Assert.True(response.IsSuccess, response.Message);
            var program = await context.Set<Entities.Program>()
                .Include(p => p.PaperFiles)
                .SingleAsync();

            Assert.Equal(ProgramTypes.Paper, program.Type);
            Assert.Equal(0, program.CountOfPractice);
            Assert.Equal(new[] { PaperFileId1, PaperFileId2 }, program.PaperFiles
                .OrderBy(pf => pf.DisplayOrder)
                .Select(pf => pf.GymFileId)
                .ToArray());
        }

        [Fact]
        public async Task CreateAsync_AttachesOwnerAsUserProgram()
        {
            using var context = CreateContext(nameof(CreateAsync_AttachesOwnerAsUserProgram));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = BuildDefaultProgramDto();
            dto.OwnerId = ManagerUserId;
            dto.OwnerStartDate = DateTime.Today.AddDays(-5);
            dto.OwnerEndDate = DateTime.Today.AddDays(10);

            var response = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);

            Assert.True(response.IsSuccess, response.Message);
            Assert.NotNull(response.Model);

            var programId = response.Model.Id;
            var ownerLink = await context.Set<UserProgram>()
                .SingleOrDefaultAsync(up => up.ProgramId == programId && up.UserId == ManagerUserId);

            Assert.NotNull(ownerLink);
            Assert.Equal(dto.OwnerStartDate.Value.Date, ownerLink!.StartDate);
            Assert.Equal(dto.OwnerEndDate?.Date, ownerLink.EndDate);
        }

        [Fact]
        public async Task UpdateAsync_SyncsOwnerUserProgram()
        {
            using var context = CreateContext(nameof(UpdateAsync_SyncsOwnerUserProgram));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var createDto = BuildDefaultProgramDto();
            createDto.OwnerId = ManagerUserId;
            var createResponse = await service.CreateAsync(GymId, ManagerUserId, createDto, CancellationToken.None);
            Assert.True(createResponse.IsSuccess, createResponse.Message);

            var programId = createResponse.Model.Id;

            var updateDto = BuildDefaultProgramDto();
            updateDto.OwnerId = SecondaryManagerUserId;
            updateDto.OwnerStartDate = DateTime.Today.AddDays(-1);
            updateDto.OwnerEndDate = DateTime.Today.AddDays(3);
            const string updatedNoteRaw = "  تمرین جدید  ";
            updateDto.Note = updatedNoteRaw;

            var updateResponse = await service.UpdateAsync(GymId, ManagerUserId, programId, updateDto, CancellationToken.None);
            Assert.True(updateResponse.IsSuccess, updateResponse.Description);
            var updatedProgram = await context.Set<Entities.Program>().SingleAsync(p => p.Id == programId);
            Assert.Equal(updatedNoteRaw.Trim(), updatedProgram.Note);

            var oldOwnerLink = await context.Set<UserProgram>()
                .FirstOrDefaultAsync(up => up.ProgramId == programId && up.UserId == ManagerUserId);
            Assert.Null(oldOwnerLink);

            var newOwnerLink = await context.Set<UserProgram>()
                .SingleOrDefaultAsync(up => up.ProgramId == programId && up.UserId == SecondaryManagerUserId);
            Assert.NotNull(newOwnerLink);
            Assert.Equal(updateDto.OwnerStartDate!.Value.Date, newOwnerLink!.StartDate);
            Assert.Equal(updateDto.OwnerEndDate?.Date, newOwnerLink.EndDate);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsPaperFileMetadata()
        {
            using var context = CreateContext(nameof(GetByIdAsync_ReturnsPaperFileMetadata));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = new ProgramDto
            {
                Title = "Paper Program",
                Type = ProgramTypes.Paper,
                PaperFileIds = new List<int> { PaperFileId1, PaperFileId2 }
            };

            var createResponse = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);
            Assert.True(createResponse.IsSuccess, createResponse.Message);
            var programId = createResponse.Model.Id;

            var detailResponse = await service.GetByIdAsync(GymId, ManagerUserId, programId, CancellationToken.None);
            Assert.True(detailResponse.IsSuccess);
            var model = detailResponse.Model;
            Assert.NotNull(model);
            Assert.Equal(2, model.PaperFiles.Count);
            Assert.Equal(@"paper\plan1.pdf", model.PaperFiles[0].RelativePath);
            Assert.Equal("application/pdf", model.PaperFiles[0].MediaType);
        }

        [Fact]
        public async Task CreateAsync_ReturnsErrorWhenSupersetHasSingleMovement()
        {
            using var context = CreateContext(nameof(CreateAsync_ReturnsErrorWhenSupersetHasSingleMovement));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = new ProgramDto
            {
                Title = "Invalid Program",
                Type = ProgramTypes.Private,
                RoutineItems = new List<ProgramRoutineItemInputDto>
                {
                    new ProgramRoutineItemInputDto
                    {
                        ItemType = ProgramRoutineItemType.Superset,
                        Title = "Bad Superset",
                        RepeatCount = 2,
                        RestBetweenRepeats = 60,
                        Practices = new List<ProgramPracticeInputDto>
                        {
                            new ProgramPracticeInputDto
                            {
                                PracticeId = PracticeAId,
                                Type = PracticeType.Set,
                                SetCount = 3,
                                MovementCount = 10,
                                Rest = 45
                            }
                        }
                    }
                }
            };

            var response = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);

            Assert.False(response.IsSuccess);
            Assert.Contains("must include at least two movements", response.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_ReturnsErrorWhenSetPracticeMissingMovementCount()
        {
            using var context = CreateContext(nameof(CreateAsync_ReturnsErrorWhenSetPracticeMissingMovementCount));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = new ProgramDto
            {
                Title = "Invalid Practice Data",
                Type = ProgramTypes.Private,
                RoutineItems = new List<ProgramRoutineItemInputDto>
                {
                    new ProgramRoutineItemInputDto
                    {
                        ItemType = ProgramRoutineItemType.Single,
                        Practices = new List<ProgramPracticeInputDto>
                        {
                            new ProgramPracticeInputDto
                            {
                                PracticeId = PracticeAId,
                                Type = PracticeType.Set,
                                SetCount = 3,
                                Rest = 60
                            }
                        }
                    }
                }
            };

            var response = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);

            Assert.False(response.IsSuccess);
            Assert.Contains("movement count", response.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ReorderRoutineItemsAsync_ReordersDisplayOrder()
        {
            using var context = CreateContext(nameof(ReorderRoutineItemsAsync_ReordersDisplayOrder));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = BuildDefaultProgramDto();
            var createResponse = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);
            Assert.True(createResponse.IsSuccess, createResponse.Message);

            var program = await context.Set<Entities.Program>()
                .Include(p => p.ProgramRoutineItems)
                .SingleAsync();

            var originalOrder = program.ProgramRoutineItems
                .OrderBy(ri => ri.DisplayOrder)
                .Select(ri => ri.Id)
                .ToList();

            var desiredOrder = originalOrder.AsEnumerable().Reverse().ToList();

            var reordered = new ProgramRoutineItemReorderDto
            {
                ProgramId = program.Id,
                Items = desiredOrder
                    .Select((id, index) => new ProgramRoutineItemOrderDto
                    {
                        RoutineItemId = id,
                        DisplayOrder = index + 1
                    })
                    .ToList()
            };

            var response = await service.ReorderRoutineItemsAsync(GymId, ManagerUserId, program.Id, reordered, CancellationToken.None);

            Assert.True(response.IsSuccess, response.Description);
            var refreshedProgram = await context.Set<Entities.Program>()
                .Include(p => p.ProgramRoutineItems)
                .SingleAsync();

            var orders = refreshedProgram.ProgramRoutineItems
                .OrderBy(ri => ri.DisplayOrder)
                .Select(ri => ri.Id)
                .ToList();
            Assert.Equal(desiredOrder, orders);
        }

        [Fact]
        public async Task ReorderSupersetPracticesAsync_ReassignsInternalOrder()
        {
            using var context = CreateContext(nameof(ReorderSupersetPracticesAsync_ReassignsInternalOrder));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = BuildDefaultProgramDto();
            var createResponse = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);
            Assert.True(createResponse.IsSuccess, createResponse.Message);

            var superset = await context.Set<ProgramRoutineItem>()
                .Include(ri => ri.ProgramPractices)
                .Where(ri => ri.ItemType == ProgramRoutineItemType.Superset)
                .SingleAsync();

            var originalOrder = superset.ProgramPractices
                .OrderBy(pp => pp.InternalOrder)
                .Select(pp => pp.Id)
                .ToList();

            var desiredOrder = originalOrder.AsEnumerable().Reverse().ToList();

            var reorderDto = new ProgramSupersetPracticeReorderDto
            {
                RoutineItemId = superset.Id,
                Practices = desiredOrder
                    .Select((id, index) => new ProgramSupersetPracticeOrderDto
                    {
                        ProgramPracticeId = id,
                        InternalOrder = index + 1
                    })
                    .ToList()
            };

            var response = await service.ReorderSupersetPracticesAsync(GymId, ManagerUserId, reorderDto, CancellationToken.None);

            Assert.True(response.IsSuccess, response.Description);
            var refreshedSuperset = await context.Set<ProgramRoutineItem>()
                .Include(ri => ri.ProgramPractices)
                .Where(ri => ri.Id == superset.Id)
                .SingleAsync();

            var refreshedOrder = refreshedSuperset.ProgramPractices
                .OrderBy(pp => pp.InternalOrder)
                .Select(pp => pp.Id)
                .ToList();
            Assert.Equal(desiredOrder, refreshedOrder);
        }

        [Fact]
        public async Task UpdateRoutineItemMetadataAsync_UpdatesSupersetFields()
        {
            using var context = CreateContext(nameof(UpdateRoutineItemMetadataAsync_UpdatesSupersetFields));
            await SeedReferenceDataAsync(context);
            var service = CreateService(context);

            var dto = BuildDefaultProgramDto();
            var createResponse = await service.CreateAsync(GymId, ManagerUserId, dto, CancellationToken.None);
            Assert.True(createResponse.IsSuccess, createResponse.Message);

            var superset = await context.Set<ProgramRoutineItem>()
                .Where(ri => ri.ItemType == ProgramRoutineItemType.Superset)
                .SingleAsync();

            var metadataDto = new ProgramRoutineItemMetadataDto
            {
                RoutineItemId = superset.Id,
                Title = "Updated Title",
                RepeatCount = 4,
                RestBetweenRepeats = 75,
                Notes = "Updated notes"
            };

            var response = await service.UpdateRoutineItemMetadataAsync(GymId, ManagerUserId, metadataDto, CancellationToken.None);

            Assert.True(response.IsSuccess, response.Description);
            var refreshedSuperset = await context.Set<ProgramRoutineItem>().FindAsync(superset.Id);
            Assert.NotNull(refreshedSuperset);
            Assert.Equal("Updated Title", refreshedSuperset!.Title);
            Assert.Equal(4, refreshedSuperset.RepeatCount);
            Assert.Equal(75, refreshedSuperset.RestBetweenRepeats);
            Assert.Equal("Updated notes", refreshedSuperset.Notes);
        }

        private ProgramDto BuildDefaultProgramDto()
        {
            return new ProgramDto
            {
                Title = "Hybrid Program",
                Note = $"  {DefaultNote}  ",
                Type = ProgramTypes.Private,
                RoutineItems = new List<ProgramRoutineItemInputDto>
                {
                    new ProgramRoutineItemInputDto
                    {
                        ItemType = ProgramRoutineItemType.Superset,
                        Title = "Upper Body Blast",
                        RepeatCount = 3,
                        RestBetweenRepeats = 90,
                        Notes = "Focus on pulling and pushing",
                        Practices = new List<ProgramPracticeInputDto>
                        {
                            new ProgramPracticeInputDto
                            {
                                PracticeId = PracticeAId,
                                Type = PracticeType.Set,
                                SetCount = 3,
                                MovementCount = 10,
                                Rest = 60
                            },
                            new ProgramPracticeInputDto
                            {
                                PracticeId = PracticeBId,
                                Type = PracticeType.Time,
                                Duration = 45,
                                Rest = 60
                            }
                        }
                    },
                    new ProgramRoutineItemInputDto
                    {
                        ItemType = ProgramRoutineItemType.Single,
                        Practices = new List<ProgramPracticeInputDto>
                        {
                            new ProgramPracticeInputDto
                            {
                                PracticeId = PracticeCId,
                                Type = PracticeType.Set,
                                SetCount = 4,
                                MovementCount = 8,
                                Rest = 90,
                                Notes = "Focus on tempo"
                            }
                        }
                    }
                }
            };
        }

        private ProgramService CreateService(ApplicationDbContext context)
        {
            return new ProgramService(
                new Repository<Entities.Program>(context),
                new Repository<ProgramRoutineItem>(context),
                new Repository<ProgramPractice>(context),
                new Repository<ProgramPaperFile>(context),
                new Repository<GymFile>(context),
                new Repository<GymUser>(context),
                new Repository<Practice>(context),
                new Repository<UserProgram>(context),
                _mapper,
                new FakeSmsService(),
                new TestOptionsSnapshot(new ProjectSettings()),
                new TestHostEnvironment());
        }

        private ApplicationDbContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            var httpContextAccessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
            var context = new ApplicationDbContext(options, httpContextAccessor);
            context.Database.EnsureCreated();
            return context;
        }

        private async Task SeedReferenceDataAsync(ApplicationDbContext context)
        {
            var manager = new ApplicationUser
            {
                Id = ManagerUserId,
                UserName = "manager",
                NormalizedUserName = "MANAGER",
                Email = "manager@example.com",
                NormalizedEmail = "MANAGER@EXAMPLE.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "00000000000",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true
            };

            var secondaryManager = new ApplicationUser
            {
                Id = SecondaryManagerUserId,
                UserName = "manager2",
                NormalizedUserName = "MANAGER2",
                Email = "manager2@example.com",
                NormalizedEmail = "MANAGER2@EXAMPLE.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "00000000001",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true
            };

            var gym = new Gym
            {
                Id = GymId,
                Title = "Test Gym",
                Slug = $"test-gym-{GymId}",
                Address = "123 Test Street",
                CreatorIP = "127.0.0.1",
                CreatorUserId = ManagerUserId,
                CreateDate = DateTime.UtcNow
            };

            var gymUser = new GymUser
            {
                GymId = GymId,
                UserId = ManagerUserId,
                Role = UsersRole.manager,
                JoinDate = DateTime.UtcNow
            };

            var secondaryGymUser = new GymUser
            {
                GymId = GymId,
                UserId = SecondaryManagerUserId,
                Role = UsersRole.manager,
                JoinDate = DateTime.UtcNow
            };

            var category = new PracticeCategory
            {
                Id = PracticeCategoryId,
                Title = "Strength",
                SubmitterUserId = ManagerUserId
            };

            var practices = new List<Practice>
            {
                new Practice
                {
                    Id = PracticeAId,
                    Name = "Barbell Row",
                    EnTitle = "Barbell Row",
                    PracticeCategoryId = PracticeCategoryId,
                    GymId = GymId,
                    UserId = ManagerUserId,
                    CreateDate = DateTime.UtcNow
                },
                new Practice
                {
                    Id = PracticeBId,
                    Name = "Assault Bike",
                    EnTitle = "Assault Bike",
                    PracticeCategoryId = PracticeCategoryId,
                    GymId = GymId,
                    UserId = ManagerUserId,
                    CreateDate = DateTime.UtcNow
                },
                new Practice
                {
                    Id = PracticeCId,
                    Name = "Back Squat",
                    EnTitle = "Back Squat",
                    PracticeCategoryId = PracticeCategoryId,
                    GymId = GymId,
                    UserId = ManagerUserId,
                    CreateDate = DateTime.UtcNow
                }
            };

            context.Users.Add(manager);
            context.Users.Add(secondaryManager);
            context.Set<Gym>().Add(gym);
            context.Set<GymUser>().Add(gymUser);
            context.Set<GymUser>().Add(secondaryGymUser);
            context.Set<PracticeCategory>().Add(category);
            context.Set<Practice>().AddRange(practices);
            var gymFiles = new List<GymFile>
            {
                new GymFile
                {
                    Id = PaperFileId1,
                    GymId = GymId,
                    OriginalFileName = "plan1.pdf",
                    StoredFileName = $"plan1_{Guid.NewGuid():N}.pdf",
                    RelativePath = @"paper\plan1.pdf",
                    ContentType = "application/pdf",
                    SizeBytes = 2048,
                    IsImage = false,
                    UploadedByUserId = ManagerUserId,
                    UploadedAt = DateTime.UtcNow,
                    CreatorUserId = ManagerUserId,
                    CreatorIP = "127.0.0.1",
                    CreateDate = DateTime.UtcNow
                },
                new GymFile
                {
                    Id = PaperFileId2,
                    GymId = GymId,
                    OriginalFileName = "plan2.pdf",
                    StoredFileName = $"plan2_{Guid.NewGuid():N}.pdf",
                    RelativePath = @"paper\plan2.pdf",
                    ContentType = "application/pdf",
                    SizeBytes = 4096,
                    IsImage = false,
                    UploadedByUserId = ManagerUserId,
                    UploadedAt = DateTime.UtcNow,
                    CreatorUserId = ManagerUserId,
                    CreatorIP = "127.0.0.1",
                    CreateDate = DateTime.UtcNow
                }
            };

            context.Set<GymFile>().AddRange(gymFiles);

            await context.SaveChangesAsync();
        }

        private class FakeSmsService : ISMSService
        {
            public Task<ResponseModel> SendSMSAsync(string UserToken, string Url, string to, string text)
                => Task.FromResult(new ResponseModel(true));

            public Task<ResponseModel<string>> IncreseCharge(string Url, int Amount)
                => Task.FromResult(new ResponseModel<string>(true, string.Empty));

            public Task<ResponseModel> ValidatePayment(string Url, int Amount, int id)
                => Task.FromResult(new ResponseModel(true));
        }

        private class TestOptionsSnapshot : IOptionsSnapshot<ProjectSettings>
        {
            public TestOptionsSnapshot(ProjectSettings value)
            {
                Value = value ?? new ProjectSettings();
            }

            public ProjectSettings Value { get; }

            public ProjectSettings Get(string name) => Value;
        }

        private class TestHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = Environments.Development;
            public string ApplicationName { get; set; } = "TestHost";
            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}
