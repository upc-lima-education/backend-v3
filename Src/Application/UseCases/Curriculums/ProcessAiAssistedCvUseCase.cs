using Backend.Src.Application.Resolvers.Common;
using Backend.Src.Domain.Contracts.Curriculums;
using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;
using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Curriculums;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.UseCases.Curriculums;

public class ProcessAiAssistedCvUseCase(
    ICvRepository cvRepository,
    ICvStructuredContentRepository structuredRepository,
    IUserRepository userRepository,
    IProfileRepository profileRepository,
    IJobRepository jobRepository,
    UbigeoResolver ubigeoResolver,
    ICvAiAssistantPort aiAssistant
)
{
    public async Task ExecuteAsync(AiAssistedCvGenerationMessage message, CancellationToken cancellationToken = default)
    {
        var cv = await cvRepository.GetByIdForUpdateAsync(message.CvId)
            ?? throw new InvalidOperationException($"CV {message.CvId} was not persisted before processing.");

        var user = await userRepository.GetByIdAsync(message.UserId)
            ?? throw new UserNotFoundException(message.UserId);

        var profile = await profileRepository.GetByUserIdAsync(user.Id)
            ?? throw new ProfileNotFoundException(user.Id);
        if (profile.CandidateProfile is null) throw new CandidateProfileRequiredException();
        if (cv.CandidateId != profile.Id)
            throw new InvalidOperationException("The queued CV does not belong to the requesting profile.");

        var job = message.JobId is null ? null : await jobRepository.GetByIdAsync((Guid)message.JobId);
        JobSnapshot? jobSnapshot = null;
        if (job is not null)
            jobSnapshot = new JobSnapshot(
                job.Title,
                job.Description,
                job.Skills.Select(s => s.Name).ToList()
            );
        var experiences = profile.CandidateProfile.WorkExperiences
            .Select((experience, index) => new
            {
                Reference = $"experience-{index}",
                Entity = experience
            })
            .ToList();
        var profileSnapshot = new CvProfileSnapshot(
            profile.Description,
            experiences.Select(x =>
                new CvWorkExperienceSnapshot(
                    x.Reference,
                    x.Entity.Position,
                    x.Entity.Description
                )
            ).ToList(),
            profile.Skills.Select(s => s.Name).ToList()
        );

        var request = new AiAssistedCvGenerationRequest(
            jobSnapshot,
            profileSnapshot
        );

        var assistanceData = await aiAssistant.GenerateAsync(request, cancellationToken);

        string? location = null;
        if (profile.Ubigeo is null) location = "Perú";
        else
        {
            var ubigeo = await ubigeoResolver.ResolveAsync(profile.Ubigeo);
            if (ubigeo is not null) location = $"{ubigeo.District}, {ubigeo.Department}, Perú";
        }

        var header = new CvHeader(
            $"{profile.CandidateProfile.FirstName} {profile.CandidateProfile.LastName}",
            assistanceData.Headline ?? "Perfil profesional",
            $"{user.Email}",
            $"{profile.PhoneNumber}",
            location
        );
        var summary = new CvTextSection("Perfil profesional", assistanceData.Description ?? "Escribe aquí tu descripción");
        var experience = new CvExperienceSection(
            experiences
                .Select(original =>
                {
                    var improved = assistanceData.WorkExperiences
                        .FirstOrDefault(item => item.Reference == original.Reference);
                    return new CvExperienceItem(
                        original.Entity.Company,
                        improved?.Position ?? original.Entity.Position,
                        original.Entity.StartDate,
                        original.Entity.EndDate,
                        improved?.Description ?? original.Entity.Description ?? "Ingresa aquí una descripción"
                    );
                }
            ).ToList()
        );
        CvEducationSection? education = new (
            profile.CandidateProfile.Educations.Select(
                x => new CvEducationItem(
                    x.Institution,
                    x.FieldOfStudy ?? "Ingrese aquí el Campo de Estudio",
                    x.Degree,
                    x.StartDate,
                    x.EndDate
                )
            ).ToList()
        );
        if (education.Items.Count <= 0) education = null;
        var optimizedSkills = assistanceData.Skills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (optimizedSkills.Count == 0)
            optimizedSkills = profile.Skills.Select(x => x.Name).ToList();

        var skills = new CvTextSection(
            "Conocimientos Técnicos",
            string.Join("; ", optimizedSkills)
        );
        var cvStructuredContent = new CvStructuredContent(
            message.CvId,
            header,
            summary,
            experience,
            education,
            skills,
            null, //languages
            null, //certifications
            null, //projects
            null, //awards
            [] //custom sections
        );
        await structuredRepository.CreateAsync(cvStructuredContent);
        cv.MarkReady();
        await cvRepository.UpdateAsync(cv);
    }
}
