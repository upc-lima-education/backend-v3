using Backend.Src.Application.Dtos.Requests.Conversations;
using Backend.Src.Application.UseCases.Conversations;
using Backend.Src.Domain.Entities.Conversations;
using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Exceptions.Conversations;
using Backend.Src.Domain.Repositories.Conversations;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.ValueObjects.Jobs;
using NSubstitute;
using Xunit;

namespace Backend.Tests.Conversations;

public class ConversationSecurityAndUseCaseTests
{
    [Fact]
    public void Conversation_SendMessage_WhenSenderNotInConversation_ThrowsUserNotInConversationException()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var stranger = Guid.NewGuid();
        var conversation = new Conversation(Guid.NewGuid(), [user1, user2]);

        // Act & Assert
        Assert.Throws<UserNotInConversationException>(() => conversation.SendMessage(stranger, "Hello!"));
    }

    [Fact]
    public void Conversation_SendMessage_WhenSenderIsInConversation_AddsMessageSuccessfully()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var conversation = new Conversation(Guid.NewGuid(), [user1, user2]);

        // Act
        conversation.SendMessage(user1, "Hello from user 1!");

        // Assert
        Assert.Single(conversation.Messages);
        Assert.Equal("Hello from user 1!", conversation.Messages.First().Content);
        Assert.Equal(user1, conversation.Messages.First().SenderActorId);
    }

    [Fact]
    public async Task GetConversationByIdUseCase_WhenUserIsParticipant_ReturnsConversation()
    {
        // Arrange
        var repo = Substitute.For<IConversationRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();
        var jobRepo = Substitute.For<IJobRepository>();

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var conversation = new Conversation(Guid.NewGuid(), [user1, user2]);

        repo.GetByIdAsync(conversation.Id).Returns(conversation);

        var useCase = new GetConversationByIdUseCase(repo, profileRepo, jobRepo);

        // Act
        var response = await useCase.ExecuteAsync(conversation.Id, user1);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(conversation.Id, response.Id);
    }

    [Fact]
    public async Task GetConversationByIdUseCase_WhenUserIsStranger_ThrowsConversationAccessDeniedException()
    {
        // Arrange
        var repo = Substitute.For<IConversationRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();
        var jobRepo = Substitute.For<IJobRepository>();

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var stranger = Guid.NewGuid();
        var conversation = new Conversation(Guid.NewGuid(), [user1, user2]);

        repo.GetByIdAsync(conversation.Id).Returns(conversation);
        profileRepo.GetByUserIdAsync(stranger).Returns((Profile?)null);

        var useCase = new GetConversationByIdUseCase(repo, profileRepo, jobRepo);

        // Act & Assert
        await Assert.ThrowsAsync<ConversationAccessDeniedException>(() => useCase.ExecuteAsync(conversation.Id, stranger));
    }

    [Fact]
    public async Task DeleteConversationUseCase_WhenUserIsStranger_ThrowsConversationAccessDeniedException()
    {
        // Arrange
        var repo = Substitute.For<IConversationRepository>();
        var profileRepo = Substitute.For<IProfileRepository>();
        var jobRepo = Substitute.For<IJobRepository>();

        var user1 = Guid.NewGuid();
        var stranger = Guid.NewGuid();
        var conversation = new Conversation(Guid.NewGuid(), [user1]);

        repo.GetByIdAsync(conversation.Id).Returns(conversation);
        profileRepo.GetByUserIdAsync(stranger).Returns((Profile?)null);

        var useCase = new DeleteConversationUseCase(repo, profileRepo, jobRepo);

        // Act & Assert
        await Assert.ThrowsAsync<ConversationAccessDeniedException>(() => useCase.ExecuteAsync(conversation.Id, stranger));
    }

    [Fact]
    public async Task GetMyConversationsUseCase_ReturnsUserConversations()
    {
        // Arrange
        var repo = Substitute.For<IConversationRepository>();
        var userId = Guid.NewGuid();
        var conversation = new Conversation(Guid.NewGuid(), [userId]);
        conversation.SendMessage(userId, "Test message");

        repo.GetConversationListByUserIdAsync(userId).Returns([conversation]);

        var useCase = new GetMyConversationsUseCase(repo);

        // Act
        var result = await useCase.ExecuteAsync(userId);

        // Assert
        Assert.Single(result);
        Assert.Equal(conversation.Id, result[0].Id);
        Assert.Single(result[0].Messages);
        Assert.Equal("Test message", result[0].Messages[0].Content);
    }
}
