using Backend.Src.Application.Dtos.Requests.Payments;
using Backend.Src.Application.Dtos.Responses.Payments;
using Backend.Src.Application.UseCases.Payments;
using Backend.Src.Domain.ValueObjects.Payments;
using Backend.Src.Infrastructure.Extensions.Auth;
using Backend.Src.Infrastructure.Options.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Src.Api.Rest.Controllers.Payments;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController(
    CreatePaymentUseCase createPayment,
    CapturePaymentUseCase capturePayment,
    GetCreditBalanceUseCase getCreditBalance,
    IOptions<PayPalOptions> payPalOptions
) : ControllerBase
{
    [HttpGet("plans")]
    [ProducesResponseType<IReadOnlyList<CreditPlanResponse>>(StatusCodes.Status200OK)]
    public IActionResult GetPlans()
    {
        var response = CreditPlanCatalog.GetAll()
            .Select(plan => new CreditPlanResponse(
                plan.Plan.ToString(),
                plan.Name,
                plan.Description,
                plan.Credits,
                plan.Price,
                payPalOptions.Value.Currency,
                plan.Price > 0
            ))
            .ToList();

        return Ok(response);
    }

    [HttpGet("balance")]
    [ProducesResponseType<CreditBalanceResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance()
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var balance = await getCreditBalance.ExecuteAsync(selfUserId);
        var freePlan = CreditPlanCatalog.Get(CreditPlan.Free);
        return Ok(new CreditBalanceResponse(balance, freePlan.Credits));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreatePaymentRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await createPayment.ExecuteAsync(selfUserId, request);

        return Ok(response);
    }

    [HttpPost("capture/{orderId}")]
    public async Task<IActionResult> Capture(string orderId)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await capturePayment.ExecuteAsync(orderId, selfUserId);
        return Ok(response);
    }
}
