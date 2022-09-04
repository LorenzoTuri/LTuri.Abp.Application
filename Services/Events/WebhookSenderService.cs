using LTuri.Abp.Application.EntityFramework;
using LTuri.Abp.Application.Events.EventBus;
using LTuri.Abp.Application.Events.Webhook;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Volo.Abp.Domain.Repositories;

namespace LTuri.Abp.Application.Services.Events
{
    public class WebhookSenderService
    {
        protected HttpClient httpClient;
        protected ILogger logger;

        protected IRepository<EventEntity> eventRepository;
        protected IRepository<WebhookEntity> webhookRepository;
        protected AggregatedEventBus eventBus;

        public WebhookSenderService(
            HttpClient httpClient,
            ILogger logger,
            IRepository<EventEntity> eventRepository,
            IRepository<WebhookEntity> webhookRepository,
            AggregatedEventBus eventBus
        )
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.eventRepository = eventRepository;
            this.webhookRepository = webhookRepository;
            this.eventBus = eventBus;
        }

        public async Task Send()
        {
            var events = (await eventRepository.GetQueryableAsync()).Where(x =>
                x.EventStatus == EntityFramework.Enum.EventStatus.Pending
            ).OrderBy(x => x.CreationTime).Take(10);

            var sendableEvents = new List<EventEntity>();

            // Change all statuses to running, if not skippable
            foreach (var eventEntity in events)
            {
                eventEntity.EventStatus = EntityFramework.Enum.EventStatus.Running;
                await eventRepository.UpdateAsync(eventEntity);

                var beforeSendEvent = new WebhookEventServiceWorkerBeforeSendEvent()
                {
                    Event = eventEntity
                };
                await eventBus.PublishAsync(beforeSendEvent);
                if (!beforeSendEvent.SkipWebhooks)
                {
                    sendableEvents.Add(eventEntity);
                }
            }

            foreach (var eventEntity in sendableEvents)
            {
                try
                {
                    // Send the event and log it's results. Also wait for each togheter, so the
                    // total amount of time awaited should be ~ equal to the longest webhook execution,
                    // with a maximal threshold (configured int the options).
                    await SendEvent(
                        webhookRepository,
                        eventEntity
                    );

                    await eventBus.PublishAsync(new WebhookEventServiceWorkerBeforeSendEvent()
                    {
                        Event = eventEntity
                    });

                    eventEntity.EventStatus = EntityFramework.Enum.EventStatus.Success;
                }
                catch (Exception ex)
                {
                    // TODO: here must be logged into database AND logger
                    eventEntity.EventStatus = EntityFramework.Enum.EventStatus.Error;
                }
                await eventRepository.UpdateAsync(eventEntity);
            }
        }

        protected async Task SendEvent(
            IRepository<WebhookEntity> webhookRepository,
            EventEntity eventEntity
        )
        {
            // Detect all subscribers
            var webhooks = (await webhookRepository.GetQueryableAsync()).Where(x =>
                x.IsActive &&
                x.EventName == eventEntity.EventName
            );

            var tasks = new List<Task>();
            foreach (var webhook in webhooks)
            {
                tasks.Add(SendEventToWebhook(webhook, eventEntity));
            }
            await Task.WhenAll(tasks);
        }

        protected async Task SendEventToWebhook(
            WebhookEntity webhook,
            EventEntity eventEntity
        )
        {
            var request = new HttpRequestMessage();
            request.Method = new HttpMethod(webhook.Method.ToString());
            request.RequestUri = new Uri(webhook.Url);

            // TODO: Add header to detect from remote client that this is the correct application calling the endpoint

            // TODO: should I send eventEntity or some kind of DTO??? Also does this populate the request or the body?
            request.Content = new StringContent(
                JsonSerializer.Serialize(eventEntity),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // TODO: error
            }

            var responseContent = JsonSerializer.Deserialize<object>(await response.Content.ReadAsStringAsync());

            // TODO: log

            // TODO: all
            // REMEMBER: timeout, certificate, error handling, success handling
            await Task.Delay(100);
        }
    }
}
