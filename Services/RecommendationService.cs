using System;
using System.Linq;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Services
{
    public class RecommendationService
    {
        public string GenerateRecommendation(User user, double cardioCals, double strengthCals, double flexibilityCals, double intervalCals)
        {
            if (!user.WorkoutLogs.Any()) 
            {
                return "У вас ще немає тренувань. Додайте свій перший запис, щоб ми могли проаналізувати ваш прогрес і дати пораду!";
            }

            string recommendation = "";
            string goal = user.Goal ?? "";
            
            switch (goal)
            {
                case "Схуднення / Жироспалювання":
                    if (cardioCals + intervalCals == 0)
                        recommendation = "Для схуднення вам бракує інтенсивності. Обов'язково додайте Кардіо або Інтервальні тренування!";
                    else if (cardioCals + intervalCals <= strengthCals) 
                        recommendation = "Для ефективнішого схуднення рекомендуємо збільшити частку Кардіо та Інтервальних тренувань відносно силових.";
                    else 
                        recommendation = "Ви чудово справляєтесь! Продовжуйте активно спалювати калорії на Кардіо та Інтервальних тренуваннях.";
                    break;
                case "Набір м'язової маси":
                    if (strengthCals == 0)
                        recommendation = "Ви взагалі не виконуєте силові тренування! Для набору маси обов'язково додайте роботу з обтяженнями.";
                    else if (strengthCals <= cardioCals + intervalCals) 
                        recommendation = "Для набору м'язової маси фокус має бути на силових тренуваннях. Зменшіть кількість Кардіо та Інтервальних сесій!";
                    else 
                        recommendation = "Відмінна робота на набір. Не забувайте про розтяжку після важких підходів для відновлення.";
                    break;
                case "Розвиток витривалості":
                    if (cardioCals + intervalCals == 0)
                        recommendation = "Витривалість потребує тривалих Кардіо або інтенсивних Інтервальних сесій. Додайте їх у свій план!";
                    else
                        recommendation = "Хороший акцент на витривалість! Регулярно підвищуйте інтенсивність Інтервальних тренувань для кращого результату.";
                    break;
                case "Покращення гнучкості та постави":
                    if (flexibilityCals == 0)
                        recommendation = "Для покращення постави та гнучкості вам необхідно додати тренування типу Flexibility (Йога, Розтяжка).";
                    else
                        recommendation = "Чудово! Ваша спина та суглоби будуть вдячні за регулярні тренування на гнучкість.";
                    break;
                case "Рельєф / Сушка":
                    if (intervalCals == 0 || strengthCals == 0)
                        recommendation = "Для рельєфу найкраще підходить мікс Силових та Інтервальних тренувань. Збалансуйте свій графік.";
                    else
                        recommendation = "Ідеальний підхід до сушки! Силові зберігають м'язи, а інтервальні спалюють жир.";
                    break;
                case "Відновлення / Реабілітація":
                    if (strengthCals > flexibilityCals + cardioCals)
                        recommendation = "Увага! При реабілітації краще уникати важких Силових навантажень. Зосередьтесь на Гнучкості та легкому Кардіо.";
                    else
                        recommendation = "Правильний підхід для відновлення. Легкі навантаження та розтяжка допоможуть тілу повернутися у форму.";
                    break;
                default:
                    if (flexibilityCals == 0) 
                        recommendation = "Для загального тонусу додайте хоча б одне тренування на гнучкість.";
                    else if (cardioCals == 0 || strengthCals == 0)
                        recommendation = "Для гармонійного тонусу намагайтесь поєднувати Силові та Кардіо тренування.";
                    else 
                        recommendation = "Ви підтримуєте ідеальний баланс між усіма типами тренувань. Так тримати!";
                    break;
            }

            return recommendation;
        }
    }
}
