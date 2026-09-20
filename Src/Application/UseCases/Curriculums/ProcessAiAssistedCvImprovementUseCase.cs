using Backend.Src.Domain.Contracts.Curriculums;
using Backend.Src.Domain.Contracts.MessageBroker.Curriculums;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Exceptions.Curriculums;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Ports.Curriculums;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Curriculums;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.UseCases.Curriculums;

public class ProcessAiAssistedCvImprovementUseCase(
    ICvRepository cvRepository,
    ICvStructuredContentRepository structuredRepository,
    IUserRepository userRepository,
    IProfileRepository profileRepository,
    IJobRepository jobRepository,
    ICvAiAssistantPort aiAssistant
)
{
    public async Task ExecuteAsync(AiAssistedCvImprovementMessage message, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(message.UserId)
            ?? throw new UserNotFoundException(message.UserId);
        var profile = await profileRepository.GetByUserIdAsync(user.Id)
            ?? throw new ProfileNotFoundException(user.Id);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var cv = await cvRepository.GetByIdForUpdateAsync(message.CvId)
            ?? throw new CvNotFoundException(message.CvId);

        if (cv.CandidateId != profile.Id)
            throw new CvAccessDeniedException(cv.Id);

        var content = await structuredRepository.GetByIdAsync(cv.Id)
            ?? throw new CvStructuredContentNotFoundException(cv.Id);

        JobSnapshot? jobSnapshot = null;

        if (message.JobId is not null)
        {
            var job = await jobRepository.GetByIdAsync(message.JobId.Value);

            if (job is not null)
                jobSnapshot = new JobSnapshot(
                    job.Title,
                    job.Description,
                    job.Skills.Select(s => s.Name).ToList()
                );
        }

        var profileSnapshot = new CvProfileSnapshot(
            profile.Description,
            profile.CandidateProfile.WorkExperiences
                .Select((experience, index) =>
                    new CvWorkExperienceSnapshot(
                        $"profile-experience-{index}",
                        experience.Position,
                        experience.Description
                    )
                )
                .ToList(),
            profile.Skills.Select(s => s.Name).ToList()
        );

        var improveSummary = message.Options.Contains(AiAssistedCvImprovementOption.Summary);
        var improveExperiences = message.Options.Contains(AiAssistedCvImprovementOption.WorkExperience);
        var improveCertifications = message.Options.Contains(AiAssistedCvImprovementOption.Certification);
        var improveProjects = message.Options.Contains(AiAssistedCvImprovementOption.Project);
        var improveAwards = message.Options.Contains(AiAssistedCvImprovementOption.Award);

        var experiences = content.Experience?.Items
            .Select((experience, index) => new
            {
                Reference = $"experience-{index}",
                Entity = experience
            })
            .ToList() ?? [];

        var cvSnapshot = new CvStructuredContentSnapshot(
            improveSummary ? content.Summary?.Description : null,
            improveExperiences
                ? experiences.Select(x =>
                    new CvWorkExperienceSnapshot(
                        x.Reference,
                        x.Entity.Position,
                        x.Entity.Description
                    )
                ).ToList()
                : null,
            improveCertifications ? content.Certifications?.Description : null,
            improveProjects ? content.Projects?.Description : null,
            improveAwards ? content.Awards?.Description : null
        );

        var request = new AiAssistedCvImprovementRequest(
            jobSnapshot,
            profileSnapshot,
            cvSnapshot
        );

        var assistanceData = await aiAssistant.ImproveAsync(
            request,
            cancellationToken
        );

        if (
            improveSummary &&
            assistanceData.Summary is not null &&
            content.Summary is not null
        )
        {
            content.UpdateSummary(
                new CvTextSection(
                    content.Summary.Title,
                    assistanceData.Summary
                )
            );
        }

        if (
            improveExperiences &&
            assistanceData.WorkExperiences is not null &&
            content.Experience is not null
        )
        {
            var updatedExperiences =
                experiences
                    .Select(original =>
                    {
                        var improved = assistanceData.WorkExperiences
                            .FirstOrDefault(item => item.Reference == original.Reference);

                        return new CvExperienceItem(
                            original.Entity.Company,
                            original.Entity.Position,
                            original.Entity.StartDate,
                            original.Entity.EndDate,
                            improved?.Description ?? original.Entity.Description
                        );
                    })
                    .ToList();
            content.UpdateExperience(new CvExperienceSection(updatedExperiences));
        }

        if (
            improveCertifications &&
            assistanceData.Certification is not null &&
            content.Certifications is not null
        )
        {
            content.UpdateCertifications(
                new CvTextSection(
                    content.Certifications.Title,
                    assistanceData.Certification
                )
            );
        }

        if (
            improveProjects &&
            assistanceData.Project is not null &&
            content.Projects is not null
        )
        {
            content.UpdateProjects(
                new CvTextSection(
                    content.Projects.Title,
                    assistanceData.Project
                )
            );
        }

        if (
            improveAwards &&
            assistanceData.Award is not null &&
            content.Awards is not null
        )
        {
            content.UpdateAwards(
                new CvTextSection(
                    content.Awards.Title,
                    assistanceData.Award
                )
            );
        }

        // El frontend usa esta marca para saber que el consumidor asíncrono
        // terminó antes de volver a generar el PDF.
        content.MarkUpdated();
        await structuredRepository.UpdateAsync(content);
        cv.RemoveUploadedContent();
        cv.MarkReady();
        await cvRepository.UpdateAsync(cv);
    }
}
