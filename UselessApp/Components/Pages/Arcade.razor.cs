using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using UselessApp.Services;

namespace UselessApp.Components.Pages;

public partial class Arcade
{
 [Parameter] public string Feature {get;set;}="converter";
 private string Title=>Feature switch {"converter"=>"Bananas for Scale","calculator"=>"Arcade Calculator","counter"=>"Achievements for Nothing","todo"=>"Excuse Department","translator"=>"Language Mangler","travel"=>"Trip Fortune","trivia"=>"Trick Trivia","soundboard"=>"Noise Department","rock"=>"Pet Rock Sanctuary","bubbles"=>"Infinite Bubble Wrap",_=>"Missing experiment"};
 private string Subtitle=>"An impressively elaborate solution to a problem you did not have.";
 private string kind="length",from="",to="",reason="",unitSort="name",conversionError="",fromSearch="",toSearch="";private double? amount;private bool fromOpen,toOpen;private bool bureaucracy,approved,agreed;private double? conversion;
 private IEnumerable<AbsurdUnit> AvailableUnits {get {var units=AbsurdConversions.Units.Where(x=>x.Kind==kind);return unitSort switch {"small"=>units.OrderBy(x=>x.BaseValue),"large"=>units.OrderByDescending(x=>x.BaseValue),_=>units.OrderBy(x=>x.Name)};}}
 private IEnumerable<AbsurdUnit> SourceMatches=>AbsurdConversions.Units.Where(x=>x.Name.Contains(fromSearch,StringComparison.OrdinalIgnoreCase)).OrderBy(x=>x.Name);
 private IEnumerable<AbsurdUnit> TargetMatches=>AvailableUnits.Where(x=>x.Name.Contains(toSearch,StringComparison.OrdinalIgnoreCase));
 private void ToggleUnits(bool source){if(source){fromOpen=!fromOpen;toOpen=false;fromSearch="";}else{toOpen=!toOpen;fromOpen=false;toSearch="";}}
 private void PickFrom(string value){from=value;fromOpen=false;toOpen=false;SourceChanged();}
 private void PickTo(string value){to=value;toOpen=false;conversion=null;}
 private void SourceChanged(){var u=AbsurdConversions.Units.FirstOrDefault(x=>x.Name.Equals(from,StringComparison.OrdinalIgnoreCase));if(u is null){conversion=null;conversionError="Choose a unit from the suggestions.";return;}kind=u.Kind;conversionError="";if(!AvailableUnits.Any(x=>x.Name.Equals(to,StringComparison.OrdinalIgnoreCase)))to="";conversion=null;approved=false;}
 private void RandomTarget(){if(string.IsNullOrEmpty(from)){conversionError="Choose a starting unit first.";return;}SourceChanged();var choices=AvailableUnits.Where(x=>!x.Name.Equals(from,StringComparison.OrdinalIgnoreCase)).ToArray();if(choices.Length>0)to=choices[Random.Shared.Next(choices.Length)].Name;conversion=null;}
 private string UnitNote=>AbsurdConversions.Units.FirstOrDefault(x=>x.Name.Equals(to,StringComparison.OrdinalIgnoreCase))?.Note ?? "Select a known unit.";
 private string UnitEmoji=>to.ToLowerInvariant() switch {var x when x.Contains("banana")=>"🍌",var x when x.Contains("eagle")=>"🦅",var x when x.Contains("giraffe")=>"🦒",var x when x.Contains("lion")=>"🦁",var x when x.Contains("whale")=>"🐋",var x when x.Contains("bus")=>"🚌",_=>"📏"};
 private void Convert(){try{var source=AbsurdConversions.Units.First(x=>x.Name.Equals(from,StringComparison.OrdinalIgnoreCase));var target=AbsurdConversions.Units.First(x=>x.Name.Equals(to,StringComparison.OrdinalIgnoreCase));if(amount is null || amount < 0){conversionError="Enter a nonnegative amount.";conversion=null;return;}conversion=AbsurdConversions.Convert(amount.Value,source.Name,target.Name);conversionError="";approved=false;}catch(Exception ex) when(ex is ArgumentException or InvalidOperationException){conversion=null;conversionError="Select compatible known units and a nonnegative amount.";}}
 private string calcMode="Catch Me",expression="12+3",calcResult="",slotDisplay="🎰 🎰 🎰",dodgeDifficulty="Medium";private int dodgeX,dodgeY,slotSpins,slotWins;private bool slotWinner,leverPulled;
 private void ResetCalcMode(){calcResult="";slotDisplay="🎰 🎰 🎰";slotSpins=0;slotWinner=false;dodgeX=0;dodgeY=0;RestartBattle();}
 private void ResetDodge(){dodgeX=0;dodgeY=0;}
 private void Dodge(){var range=dodgeDifficulty switch {"Easy"=>30,"Medium"=>100,"Hard"=>175,_=>260};dodgeX=Random.Shared.Next(-range,range+1);dodgeY=Random.Shared.Next(0,Math.Min(range,115)+1);}
 private string CalculateExpression()=>new CalculatorEvaluatorService().Evaluate(System.Text.RegularExpressions.Regex.Matches(expression.Replace(" ", ""), @"(?:\d+(?:\.\d+)?|\.\d+)|[A-Za-z]+|[+*/%^()\-]").Select(m => m.Value).ToList(), scrambleOperators: false);
 private void Calculate(){calcResult=CalculateExpression();}
 private void Spin(){var actual=CalculateExpression();if(!double.TryParse(actual,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out var answer)){calcResult=actual;return;}leverPulled=!leverPulled;slotWinner=Random.Shared.Next(3)==0;slotSpins++;if(slotWinner)slotWins++;var shown=slotWinner?answer:answer+(Random.Shared.Next(1,10)*(Random.Shared.Next(2)==0?-1:1));slotDisplay=shown.ToString("G8",System.Globalization.CultureInfo.InvariantCulture);calcResult=slotWinner?"Correct! The house briefly respects mathematics.":"Try again to reveal the correct result.";}
 private int playerHp=100,bossHp=100,bossLevel=1,questionA=4,questionB=7;private int? bossAnswer;private char questionOperator='+';private string battleMessage="Answer correctly to damage the boss.";private string bossName=>bossLevel switch {1=>"Addition Goblin",2=>"Subtraction Ogre",3=>"Multiplication Dragon",_=>"The Final Exam"};
 private string bossDifficulty="Normal",bossAnswerMode="Multiple Choice";private int[] BossChoices=[11,12,13,14];
 private int ExpectedBossAnswer=>questionOperator switch {'-'=>questionA-questionB,'*'=>questionA*questionB,_=>questionA+questionB};
 private void NextQuestion(){var scale=bossDifficulty switch {"Easy"=>6,"Normal"=>10,"Hard"=>18,_=>35};questionA=Random.Shared.Next(2,scale+bossLevel*2);questionB=Random.Shared.Next(1,scale+bossLevel*2);questionOperator=bossDifficulty=="Easy"?'+':bossLevel switch {1=>'+',2=>'-',_=>'*'};bossAnswer=null;var correct=ExpectedBossAnswer;var choices=new HashSet<int>{correct};while(choices.Count<4)choices.Add(correct+Random.Shared.Next(-Math.Max(10,scale),Math.Max(10,scale)+1));BossChoices=choices.OrderBy(_=>Random.Shared.Next()).ToArray();}
 private void AttackChoice(int choice){bossAnswer=choice;Attack();}
 private void Attack(){if(playerHp<=0||bossHp<=0)return;var expected=ExpectedBossAnswer;if(bossAnswer==expected){var damage=Math.Clamp(Math.Abs(expected),8,35) * (bossDifficulty=="Easy"?2:bossDifficulty=="Hard"?1:bossDifficulty=="Nightmare"?1:1);bossHp=Math.Max(0,bossHp-damage);battleMessage=$"Correct! {damage} damage to {bossName}.";if(bossHp==0){bossLevel++;bossHp=100;playerHp=Math.Min(100,playerHp+25);battleMessage+=" Boss defeated! Next boss arrives.";}}else {playerHp=Math.Max(0,playerHp-(bossDifficulty switch {"Easy"=>10,"Hard"=>30,"Nightmare"=>45,_=>20}));battleMessage=$"Incorrect! Answer: {expected}. You take {bossDifficulty switch {"Easy"=>10,"Hard"=>30,"Nightmare"=>45,_=>20}} damage.";if(playerHp==0)battleMessage+=" Game over. Restart to try again.";}NextQuestion();}
 private void RestartBattle(){playerHp=100;bossHp=100;bossLevel=1;battleMessage="Fresh battle. Good luck!";NextQuestion();}
 private int count,lifetime;private string speedMode="normal",speedStatus="Click to begin.";private double currentCps,peakCps,bestFive,bestTen;private DateTimeOffset? clickStart;private readonly List<DateTimeOffset> clickTimes=[];
 private readonly int[] milestones=[1,5,10,25,50,75,100,150,250,500,750,1000,1500,2500,5000,10000,25000];
 private string AchievementName(int milestone)=>milestone switch {1=>"First mistake",5=>"Five-finger discount",10=>"Button admirer",25=>"Questionable commitment",50=>"Click enthusiast",100=>"Certified finger athlete",250=>"Click department manager",500=>"Button supervisor",1000=>"Thousand-click stare",2500=>"Mouse warranty void",5000=>"Legendary clicker",10000=>"Unpaid button CEO",_=>"Absolutely pointless prestige"};
 private void ResetSpeed(){count=0;clickStart=null;clickTimes.Clear();currentCps=0;speedStatus="Ready. First click starts the clock.";}
 private void ClickCounter(){var now=DateTimeOffset.UtcNow;if(speedMode is "five" or "ten"){var duration=speedMode=="five"?5:10;if(clickStart.HasValue&&(now-clickStart.Value).TotalSeconds>=duration){var score=count/(double)duration;if(speedMode=="five")bestFive=Math.Max(bestFive,score);else bestTen=Math.Max(bestTen,score);speedStatus=$"Finished: {score:F2} CPS. Reset to start again.";return;}}clickStart??=now;count++;lifetime++;clickTimes.Add(now);clickTimes.RemoveAll(t=>(now-t).TotalSeconds>1);currentCps=clickTimes.Count;peakCps=Math.Max(peakCps,currentCps);if(speedMode is "five" or "ten"){var duration=speedMode=="five"?5:10;var elapsed=(now-clickStart.Value).TotalSeconds;if(elapsed>=duration){var score=count/(double)duration;if(speedMode=="five")bestFive=Math.Max(bestFive,score);else bestTen=Math.Max(bestTen,score);speedStatus=$"Finished: {score:F2} CPS.";}else speedStatus=$"{Math.Max(0,duration-elapsed):F1}s left — click faster!";}else speedStatus=speedMode=="free"?$"Best one-second burst: {peakCps:F0} CPS":"Every click counts. Tragically.";}
 private sealed class ExcuseTask {public string Name{get;set;}="";public string Excuse{get;set;}="";}
 private readonly List<ExcuseTask> tasks=[]; private string taskInput="";
 private void AddTask(){if(string.IsNullOrWhiteSpace(taskInput)||tasks.Count>=40)return;tasks.Add(new(){Name=taskInput.Trim(),Excuse=AbsurdText.Excuse(taskInput,Random.Shared.Next())});taskInput="";}
 private void Reroll(ExcuseTask t)=>t.Excuse=AbsurdText.Excuse(t.Name,Random.Shared.Next());
 private readonly string[] modes=["Gen Z","Gen Z → English","Pirate","Corporate","Medieval","1980s"];
 private string voice="Gen Z",translationInput="That cool friend has charm and money.",translation="";
 private string destination="",travelResult="",fictionalWeather="";
 private void Foretell(){if(string.IsNullOrWhiteSpace(destination)){travelResult="Please name a destination.";return;}travelResult=AbsurdText.Prophecy(destination,Random.Shared.Next());fictionalWeather=absurdWeather[Random.Shared.Next(absurdWeather.Length)];}
 private readonly string[] absurdWeather=["Blizzard with a chance of flamingos (even in Florida in June).","104°F and raining marshmallows.","Light snowfall followed by a solar eclipse at lunch.","Tornado warning issued by three extremely confident squirrels.","Partly cloudy with localized spaghetti storms.","A heat wave interrupted by a surprise penguin parade.","-40°F and somehow humid.","Thunderstorms of glitter, bring safety goggles.","Clear skies until a rogue snowman appears.","A 100% chance of weather; details classified."];
 private int volume=35;private readonly string[] sounds=["Boing","Applause","Explosion","Record Scratch","Victory","Failure","Laser","Coin","Drama","Squeak","Pop","Drum Roll"];
 private async Task PlaySound(string sound)
 {
   try { await JS.InvokeVoidAsync("absurdArcade.play", sound, volume / 100.0); }
   catch (Microsoft.JSInterop.JSException) { /* Audio should never terminate the Blazor circuit. */ }
 }
 private string rockName="Rocky",rockMessage="Your rock refuses to comment.";private int rockActions;
 private void FeedRock()=>CareRock("feed");private void PetRock()=>CareRock("pet");private void SleepRock()=>CareRock("sleep");
 private void CareRock(string action){rockActions++;rockMessage=action switch {"feed"=>"The rock is not hungry. Somehow it ate nothing.","pet"=>"The rock tolerates your affection.",_=>"Rocky is now sleeping with its eyes open."};}
 private sealed record TrickQuestion(string Prompt,string[] Options,int Odd,string Explanation);
 private readonly TrickQuestion[] questionBank=[
  new("Which of these can you put in a sandwich?",["Peanut butter","Pickles","Turkey","A traffic jam"],3,"Traffic jams are best avoided at lunch."),
  new("Which might you find in a library?",["Atlases","Novels","Dictionaries","A working volcano"],3,"Volcanoes generally don't meet library return policies."),
  new("Which things can have a screen?",["A smartphone","A window","A projector setup","An unpeeled banana"],3,"A banana has a peel, not a screen."),
  new("Which could reasonably be called a mouse?",["A small rodent","A computer input device","A cartoon character","A refrigerator compressor"],3,"Compressor isn't a conventional mouse meaning."),
  new("Which can run?",["A human","Software","Water","A brick without assistance"],3,"Bricks are not known for running."),
  new("Which of these can have keys?",["A piano","A keyboard","A car","A plain potato"],3,"Potatoes lack keys except in very strange cartoons."),
  new("Which can be served cold?",["Ice cream","Revenge (figuratively)","Iced coffee","A live lightning bolt"],3,"Lightning is an electrical discharge, not cuisine."),
  new("Which could break the ice?",["An icebreaker ship","A hammer","A funny introduction","An invisible promise"],3,"A promise can't physically or conversationally break ice by itself."),
  new("Which can be in a band?",["A guitarist","A drummer","A brass player","A parking meter"],3,"Parking meters aren't musicians."),
  new("Which can have a tail?",["A fox","A comet","A coin in 'heads or tails'","A plain cube"],3,"A plain cube has no tail."),
  new("Which can be a star?",["The Sun","An actor","A five-point symbol","A stapler by default"],3,"Staplers rarely qualify as stars."),
  new("Which can be a date?",["A calendar day","A fruit","A romantic outing","A left shoe"],3,"Shoes are not dates."),
  new("Which can have a bank?",["A river","A financial institution","A food bank","A plate of spaghetti"],3,"Dinner doesn't normally have a bank."),
  new("Which could be a charge?",["An electrical quantity","A shopping fee","A cavalry advance","A pancake's favorite song"],3,"Pancakes have no documented favorite songs."),
  new("Which can have a pitch?",["A baseball throw","A music note","A sales presentation","A silent shoelace"],3,"A shoelace has no pitch."),
  new("Which can be a draft?",["An early manuscript","A current of air","A sports selection","A sleeping cactus"],3,"Cacti are not drafts."),
  new("Which can have a trunk?",["An elephant","A tree","A car","A teaspoon"],3,"Teaspoons don't have trunks."),
  new("Which might have a crown?",["A monarch","A tooth","A tree","A generic paperclip"],3,"Paperclips have no crowns."),
  new("Which can have a root?",["A tree","An equation","A word","A cloud of perfume"],3,"Perfume clouds don't have roots."),
  new("Which can have a point?",["A pencil","An argument","A compass needle","A round puddle with no corners"],3,"The puddle isn't pointed."),
  new("Which could have a case?",["A lawyer","A phone","A detective","A sunbeam"],3,"Sunbeams don't carry cases."),
  new("Which can be a spring?",["A season","A coiled metal part","A water source","A frozen spreadsheet"],3,"Spreadsheets are seldom natural springs."),
  new("Which can have a page?",["A book","A website","A newspaper","An uncut gemstone"],3,"Gemstones don't normally have pages."),
  new("Which can have a ring?",["A phone","A planet","A wedding ceremony","A shadow on an empty wall by itself"],3,"The shadow by itself has no ring."),
 ];
 private TrickQuestion CurrentQuestion=>questionBank[questionIndex];private int questionIndex,triviaScore,triviaAttempts;private string[] triviaAnswers=["", "", "", ""];private int triviaOdd;private bool answered;private string triviaFeedback="Choose the most out-of-place answer.";
 protected override void OnInitialized(){NewTrivia();}
 private void NewTrivia(){questionIndex=Random.Shared.Next(questionBank.Length);var q=CurrentQuestion;var shuffled=Enumerable.Range(0,4).OrderBy(_=>Random.Shared.Next()).ToArray();triviaAnswers=shuffled.Select(i=>q.Options[i]).ToArray();triviaOdd=Array.IndexOf(shuffled,q.Odd);answered=false;triviaFeedback="Three fit; one really doesn't.";}
 private void ChooseTrivia(int choice){if(answered)return;answered=true;triviaAttempts++;if(choice==triviaOdd){triviaScore++;triviaFeedback="Correct! "+CurrentQuestion.Explanation;}else triviaFeedback="Not quite. "+CurrentQuestion.Explanation;}
 private bool[] bubbles=new bool[25];private int pops,sheets,points,totalBubblePoints,combo;private bool goldBubbles,doublePoints;
 private string bubbleMessage="The bubbles are waiting for your highly important work.";
 private int BaseBubblePoints=>(goldBubbles?2:1)*(doublePoints?2:1);
 private int BubbleMultiplier=>Math.Min(3,1+combo/5);
 private async Task Pop(int index){
  if(bubbles[index])return;
  bubbles[index]=true;pops++;combo++;
  int earned=BaseBubblePoints*BubbleMultiplier;points+=earned;totalBubblePoints+=earned;
  bubbleMessage=$"Pop! +{earned} points"+(BubbleMultiplier>1?$" · ×{BubbleMultiplier} combo!":"");
  await PlaySound("Pop");
  if(bubbles.All(x=>x)){int reward=10;points+=reward;totalBubblePoints+=reward;sheets++;combo=0;bubbleMessage=$"Sheet complete! +{reward} bonus points. Your next sheet is ready.";bubbles=new bool[25];}
 }
 private void NewSheet(){bubbles=new bool[25];combo=0;bubbleMessage="Fresh sheet issued. The excitement is immeasurable.";}
 private void BuyUpgrade(){if(points<30||goldBubbles)return;points-=30;goldBubbles=true;bubbleMessage="Gold bubble coating acquired. Pure luxury.";}
 private void BuyDouble(){if(points<50||doublePoints)return;points-=50;doublePoints=true;bubbleMessage="Double points activated. Your imaginary fortune grows.";}

 private void ClearSlotResult(){slotSpins=0;slotWins=0;slotWinner=false;calcResult="";}

}
