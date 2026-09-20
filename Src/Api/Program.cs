using Backend.Src.Infrastructure.DependencyInjections;
using Backend.Src.Api.DependencyInjections;
using Backend.Src.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);

builder.Services.AddPersistenceModule(builder.Configuration);

builder.Services
    .AddCommonModule(builder.Configuration)
    .AddAuthModule(builder.Configuration)
    .AddProfilesModule(builder.Configuration)
    .AddJobsModule(builder.Configuration)
    .AddConversationModule()
    .AddMessageBrokerModule(builder.Configuration)
    .AddCvModule(builder.Configuration)
    .AddNotificationsModule(builder.Configuration)
    .AddRecruitmentModule()
    .AddPaymentsModule(builder.Configuration)
    .AddSkillsModule();

var app = builder.Build();

app.UseApi();

app.Run();
