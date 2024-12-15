using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Newtonsoft.Json;
using SharedModels.models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RoutingServer.Utils
{
    public class ActiveMQUtils
    {
        private static IConnectionFactory connectionFactory;
        private static IConnection connection;
        private static ISession session;
        private static IMessageProducer producer;
        private static IQueueBrowser browser;

        private static string currentProfile;
        private static readonly int BATCH_SIZE = 10;
        private static Instructions[] allInstructions;
        private static Instructions currentInstructions;
        private static int currentInstructionIndex = 0;
        private static int currentRouteIndex = 0;

        // For managing cancellation
        private static CancellationTokenSource cancellationTokenSource;

        public static void Initialize()
        {

            if (session != null)
            {
                session.Close();
                session.Dispose();
            }

            connectionFactory = new ConnectionFactory("tcp://localhost:61616");
            connection = connectionFactory.CreateConnection();
            connection.Start();
            session = connection.CreateSession();
            producer = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQQueue("routing"));

            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine(cancellationTokenSource.Token);


        }

        public static async Task SendInstructions(Instructions[] instructions)
        {
            if (session == null || producer == null)
            {
                throw new InvalidOperationException("ActiveMQUtils is not initialized. Call Initialize() before sending messages.");
            }

            ResetState();

            currentProfile = instructions[0].Profile;
            allInstructions = instructions;

            currentInstructionIndex = 0;
            currentRouteIndex = 0;

            await StartSendingData();
        }

        public static void ResetState()
        {

            currentInstructionIndex = 0;
            currentRouteIndex = 0;
            allInstructions = null;
        }

        public static async Task StartSendingData()
        {
            while (currentInstructionIndex < allInstructions.Length)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Task was cancelled.");
                    return;
                }

                if (IsQueueEmpty())
                {
                    await SendBatch();
                }

                await Task.Delay(500, cancellationTokenSource.Token);
            }
        }

        private static bool IsQueueEmpty()
        {
            var queue = session.GetQueue("routing");
            browser = session.CreateBrowser((IQueue)queue);
            var enumerator = browser.GetEnumerator();
            var isEmpty = !enumerator.MoveNext();
            browser.Dispose();
            return isEmpty;
        }

        public static async Task SendBatch()
        {
            if (allInstructions == null || allInstructions.Length == 0)
            {
                return;
            }

            List<Route> routesToSend = new List<Route>();

            while (routesToSend.Count < BATCH_SIZE && (currentInstructionIndex < allInstructions.Length))
            {
                currentInstructions = allInstructions[currentInstructionIndex];

                while (currentRouteIndex < currentInstructions.Routes.Count && routesToSend.Count < BATCH_SIZE)
                {
                    routesToSend.Add(currentInstructions.Routes[currentRouteIndex]);
                    currentRouteIndex++;
                }

                if (currentRouteIndex >= currentInstructions.Routes.Count)
                {
                    currentInstructionIndex++;
                    currentRouteIndex = 0;
                }
            }

            if (routesToSend.Count > 0)
            {
                foreach (var route in routesToSend)
                {
                    await SendRoute(route, currentProfile);
                }
            }
        }

        public static async Task SendRoute(Route route, string currentProfile)
        {
            if (session == null || producer == null)
            {
                throw new InvalidOperationException("ActiveMQUtils is not initialized. Call Initialize() before sending messages.");
            }

            var routeData = new
            {
                Profile = currentProfile,
                Route = route
            };

            string json = JsonConvert.SerializeObject(routeData);

            ITextMessage message = session.CreateTextMessage(json);

            await Task.Run(() => producer.Send(message));
        }
    }
}
