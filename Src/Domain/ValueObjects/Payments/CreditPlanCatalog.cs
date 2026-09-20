namespace Backend.Src.Domain.ValueObjects.Payments;

public static class CreditPlanCatalog
{
    private static readonly IReadOnlyDictionary<CreditPlan, CreditPlanInfo> Plans =
        new Dictionary<CreditPlan, CreditPlanInfo>
        {
            {
                CreditPlan.Free,
                new CreditPlanInfo(
                    CreditPlan.Free,
                    "Free",
                    "Incluye tres créditos iniciales para generar CV con inteligencia artificial.",
                    3,
                    0m
                )
            },
            {
                CreditPlan.Starter,
                new CreditPlanInfo(
                    CreditPlan.Starter,
                    "Starter",
                    "Ideal para mejorar algunas postulaciones y probar las funciones premium.",
                    5,
                    0.99m
                )
            },
            {
                CreditPlan.Pro,
                new CreditPlanInfo(
                    CreditPlan.Pro,
                    "Pro",
                    "La mejor opción para candidatos que buscan empleo de forma constante.",
                    15,
                    2.49m
                )
            },
            {
                CreditPlan.Max,
                new CreditPlanInfo(
                    CreditPlan.Max,
                    "Max",
                    "Pensado para usuarios intensivos que optimizan CVs y postulan a múltiples oportunidades.",
                    35,
                    4.99m
                )
            }
        };

    public static CreditPlanInfo Get(CreditPlan plan) => Plans[plan];

    public static IEnumerable<CreditPlanInfo> GetAll() => Plans.Values;
}
