using System.Reflection;
using UselessApp.Components.Pages;

namespace UselessApp.UnitTests;

public class QuestionOrderTests
{
    private static void Invoke(object instance, string method) =>
        instance.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(instance, null);
    private static T Field<T>(object instance, string name) =>
        (T)instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(instance)!;

    [Fact]
    public void Captcha_VisitsEveryChallengeBeforeRepeating_AndShufflesAllChoices()
    {
        var game = new ArcadeExtras();
        Invoke(game, "PrepareCaptcha");
        var count = Field<int[]>(game, "captchaDeck").Length;
        Assert.True(count >= 32);
        var seen = new HashSet<int>();
        for (var i = 0; i < count; i++)
        {
            var deck = Field<int[]>(game, "captchaDeck");
            Assert.True(seen.Add(deck[Field<int>(game, "captchaCursor")]));
            Assert.Equal(new[] { 0, 1, 2, 3 }, Field<int[]>(game, "captchaChoiceOrder").Order().ToArray());
            Invoke(game, "PrepareCaptcha");
        }
    }

    [Fact]
    public void Trivia_VisitsEveryQuestionBeforeRepeating()
    {
        var game = new Arcade();
        Invoke(game, "NewTrivia");
        var count = Field<int[]>(game, "triviaDeck").Length;
        var seen = new HashSet<int>();
        for (var i = 0; i < count; i++)
        {
            Assert.True(seen.Add(Field<int>(game, "questionIndex")));
            Assert.Equal(4, Field<string[]>(game, "triviaAnswers").Distinct().Count());
            Invoke(game, "NewTrivia");
        }
    }
}
