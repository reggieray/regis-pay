using NBomber.CSharp;
using Refit;
using Regis.Pay.Tests.Shared.ApiClient;
using Regis.Pay.Tests.Shared.EventTestConsumer.MultiEventTestConsumer;
using Response = NBomber.CSharp.Response;

namespace Regis.Pay.LoadTests;

public class PaymentLoadTests
{
    [Fact]
    public void CreatePaymentApiLoadTest()
    {
        var apiClient = RestService.For<IRegisPayApiClient>("https://localhost:7185");

        var scenario = Scenario.Create("create_payment_api_response", async context =>
        {
            var request = new CreatePaymentRequest(100000, "GBP");

            var response = await apiClient.CreatePayment(request);

            var paymentId = response?.Content?.PaymentId;

            return response!.IsSuccessStatusCode ? Response.Ok(paymentId) : Response.Fail(paymentId);
        })
        .WithLoadSimulations(
            Simulation.Inject(rate: 3,
                              interval: TimeSpan.FromSeconds(1),
                              during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void FullPaymentJourneyLoadTest()
    {
        var timeout = TimeSpan.FromMinutes(10);
        var apiClient = RestService.For<IRegisPayApiClient>("https://localhost:7185");

        var paymentInitiatedEvents = new MultiPaymentInitiatedEventTestConsumer();
        var paymentCreatedEvents = new MultiPaymentCreatedEventTestConsumer();
        var paymentSettledEvents = new MultiPaymentSettledEventTestConsumer();
        var paymentCompltedEvents = new MultiPaymentCompletedEventTestConsumer();

        paymentInitiatedEvents.ListenToEvents();
        paymentCreatedEvents.ListenToEvents();
        paymentSettledEvents.ListenToEvents();
        paymentCompltedEvents.ListenToEvents();

        try
        {
            var scenario = Scenario.Create("full_payment_journey", async context =>
            {
                var create_payment_request_step = await Step.Run("create_payment_request", context, async () =>
                {
                    var request = new CreatePaymentRequest(100000, "GBP");

                    var response = await apiClient.CreatePayment(request);

                    var paymentId = response?.Content?.PaymentId;

                    return response!.IsSuccessStatusCode ? Response.Ok(paymentId) : Response.Fail(paymentId);
                }).WaitAsync(timeout);

                var expecetedAggregateId = $"pay:{create_payment_request_step.Payload.Value}";

                var payment_initiated_step = await Step.Run("payment_initiated", context, async () =>
                {
                    while (!paymentInitiatedEvents.EventIds.TryTake(out var result))
                    {
                        await Task.Delay(1);
                    }

                    return Response.Ok();
                }).WaitAsync(timeout);

                var payment_created_step = await Step.Run("payment_created", context, async () =>
                {
                    while (!paymentCreatedEvents.EventIds.TryTake(out var result))
                    {
                        await Task.Delay(1);
                    }

                    return Response.Ok();
                }).WaitAsync(timeout);

                var payment_settled_step = await Step.Run("payment_settled", context, async () =>
                {
                    while (!paymentSettledEvents.EventIds.TryTake(out var result))
                    {
                        await Task.Delay(1);
                    }

                    return Response.Ok();
                }).WaitAsync(timeout);

                var payment_completed_step = await Step.Run("payment_completed", context, async () =>
                {
                    while (!paymentCompltedEvents.EventIds.TryTake(out var result))
                    {
                        await Task.Delay(1);
                    }

                    return Response.Ok();
                }).WaitAsync(timeout);

                return Response.Ok();
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 1,
                                  interval: TimeSpan.FromSeconds(1),
                                  during: TimeSpan.FromSeconds(30))
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
        catch
        (Exception)
        { }
        finally 
        {
            paymentInitiatedEvents.Dispose();
            paymentCreatedEvents.Dispose();
            paymentSettledEvents.Dispose();
            paymentCompltedEvents.Dispose();
        }
    }
}