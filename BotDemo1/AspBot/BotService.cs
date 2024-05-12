using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AspBot
{
    public class BotService : BackgroundService
    {
        private readonly TelegramBotClient _botClient;
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };
        public BotService()
        {
            _botClient = new TelegramBotClient("7182360517:AAFMJseBoasM9m0P0LygoFiz2jX0ufZV5fU");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                stoppingToken
            );

            var me = await _botClient.GetMeAsync(stoppingToken);
            Console.WriteLine($"Start listening for @{me.Username}");

            // Wait for the bot to stop
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            var userName = message.Chat.Username;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}. an user Name {userName}");

            switch (messageText)
            {
                case "api":
                    await botClient.SendTextMessageAsync(chatId, "Ссылка на документацию API бота: https://core.telegram.org/bots/api", cancellationToken: cancellationToken);
                    break;
                case "help":
                    await botClient.SendTextMessageAsync(chatId, "Вы написали умному боту, используйте команды: api, help, привет", cancellationToken: cancellationToken);
                    break;
                case "привет":
                    await botClient.SendTextMessageAsync(chatId, "Привет! Я ваш умный Telegram бот. Я могу помочь вам с информацией по API и другим вопросам.", cancellationToken: cancellationToken);
                    break;
                case "/keyboard":
                    ReplyKeyboardMarkup kyboard = new(new[]{
                new KeyboardButton[]{"Hello", "GoodBuy"},
                new KeyboardButton[]{"Привет", "Пока"}

            })
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendTextMessageAsync(chatId, "Choose", replyMarkup: kyboard, cancellationToken: cancellationToken);
                    break;


                default:
                    // Отправляем сообщение обратно пользователю
                    await botClient.SendTextMessageAsync(chatId, "Вы сказали: " + messageText, cancellationToken: cancellationToken);
                    break;
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
