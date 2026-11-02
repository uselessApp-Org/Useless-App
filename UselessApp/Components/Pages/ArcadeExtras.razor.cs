using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using UselessApp.Services;
using System.Security.Cryptography;

namespace UselessApp.Components.Pages;

public partial class ArcadeExtras
{
 [Parameter]public string Game{get;set;}="captcha";
 private string Title=>Game switch{"gym"=>"Password Strength Gym","captcha"=>"Unnecessary CAPTCHA","rps"=>"RPS Championship","platformer"=>"One-Button Platformer",_=>"Missing game"};
 private string Subtitle=>Game switch{"gym"=>"A completely private gym for pretend passwords.","captcha"=>"Show us your humanity. Preferably with potatoes.","rps"=>"Playground warfare, professional presentation.",_=>"A surprisingly playable game with one control."};
 private string? mountedGame;
 protected override async Task OnAfterRenderAsync(bool first){
  if(mountedGame==Game)return;
  try {
   if(mountedGame=="gym") await JS.InvokeVoidAsync("uselessExtras.gym.clear","local-password-gym");
   if(mountedGame=="platformer") await JS.InvokeVoidAsync("uselessExtras.platform.stop","one-button-course");
   if(Game=="gym") await JS.InvokeVoidAsync("uselessExtras.gym.mount","local-password-gym");
   if(Game=="platformer") await JS.InvokeVoidAsync("uselessExtras.platform.start","one-button-course");
   mountedGame=Game;
  }catch(JSException){ /* Script may still be loading; render a retry control. */ }
 }
 public async ValueTask DisposeAsync(){try{if(Game=="gym")await JS.InvokeVoidAsync("uselessExtras.gym.clear","local-password-gym");if(mountedGame=="platformer")await JS.InvokeVoidAsync("uselessExtras.platform.stop","one-button-course");}catch(JSDisconnectedException){}}
 private record Tile(string Icon,string Caption);
 private static readonly (string Prompt,Tile[] Choices,int Odd,string Why)[] Challenges=[
 ("Which cloud has been illegally moonlighting as office furniture?",[new("☁️","Cumulus"),new("🌧️","Rain cloud"),new("🗄️","Filing cabinet with altitude"),new("⛅","Part-time cloud")],2,"The cabinet failed its atmospheric background check."),
 ("Select the goose most likely to be three accountants in a coat.",[new("🪿","Honk"),new("🧥","Honk, audited"),new("🪿","Also honk"),new("🪿","Honk junior")],1,"It asked to deduct the pond as a business expense."),
 ("The moon is holding a sleepover. Who brought the worst pillow?",[new("🛏️","Pillow"),new("🧸","Teddy bear"),new("🧱","Load-bearing brick"),new("☁️","Borrowed cloud")],2,"Excellent structural support. Terrible dreams."),
 ("Which wizard has accidentally summoned a household appliance?",[new("🐉","Dragon"),new("🦄","Unicorn"),new("🧚","Fairy"),new("🧺","Enchanted spin cycle")],3,"The washing machine demands fabric softener as tribute."),
 ("Find the vegetable currently running for mayor of the internet.",[new("🥕","Carrot"),new("🥦","Broccoli"),new("🥔","Potato with a campaign manager"),new("🥬","Lettuce")],2,"Its platform is universal free gravy."),
 ("Which passenger cannot fit in the emotional support hamster carrier?",[new("🐹","Hamster"),new("🐭","Tiny mouse"),new("🐹","Hamster wearing socks"),new("🪐","Saturn")],3,"The rings do not qualify as carry-on luggage."),
 ("Spot the impostor at the secret society of breakfast.",[new("🥞","Pancakes"),new("🧇","Waffle"),new("🍳","Egg"),new("📠","Fax machine demanding syrup")],3,"It keeps faxing the bacon to 1997."),
 ("Which employee has been replacing all the meeting notes with lava?",[new("🧑‍💼","Greg"),new("🧑‍💻","Pat"),new("🌋","Mount Spreadsheet"),new("🧑‍🔧","Sam")],2,"The quarterly eruption was not on the calendar."),
 ("Which potato looks like it has a secret identity?",[new("🥔","Ordinary Potato"),new("🥔","Another Potato"),new("🥸","Potato in disguise"),new("🥔","Responsible Potato")],2,"The mustache was a giveaway."),
 ("Three are harmless lunch items. Which one is the suspicious intruder?",[new("🥪","Sandwich"),new("🌮","Taco"),new("🍎","Apple"),new("🚀","Rocket")],3,"Rockets are notoriously difficult to digest."),
 ("The duck convention admitted one impostor. Identify them.",[new("🦆","Duck One"),new("🦆","Duck Two"),new("🐧","Suspiciously formal duck"),new("🦆","Duck Three")],2,"The tuxedo is an excellent disguise, but that's a penguin."),
 ("Three objects are useful for rain. Which one isn't?",[new("☂️","Umbrella"),new("🧥","Raincoat"),new("🥾","Boots"),new("🍕","Pepperoni Pizza")],3,"Pizza is an unreliable umbrella."),
 ("Three things belong in a library. Identify the odd one.",[new("📚","Books"),new("📖","Novel"),new("🔖","Bookmark"),new("🦖","Tyrannosaurus")],3,"Please stop bringing dinosaurs to the library."),
 ("Three are aquatic. Who cannot swim here?",[new("🐟","Fish"),new("🐬","Dolphin"),new("🐋","Whale"),new("🪑","Office Chair")],3,"The chair insists it took lessons."),
 ("Three belong at a birthday. One made a wrong turn.",[new("🎂","Cake"),new("🎈","Balloon"),new("🎁","Present"),new("🧯","Fire Extinguisher")],3,"Okay, the extinguisher might be useful, but this is suspicious."),
 ("Three are musical instruments. One is confidently pretending.",[new("🎸","Guitar"),new("🎹","Keyboard"),new("🥁","Drums"),new("🧱","Brick")],3,"The brick has excellent percussion aspirations."),
 ("Which object has clearly arrived at the wrong meeting?",[new("📎","Paperclip"),new("🗂️","Folder"),new("🖇️","Binder clips"),new("🦕","Stegosaurus")],3,"Dinosaurs rarely submit quarterly reports."),
 ("Our bakery hired one unqualified pastry. Identify it.",[new("🥐","Croissant"),new("🥯","Bagel"),new("🥨","Pretzel"),new("🔧","Wrench")],3,"The wrench is very difficult to frost."),
 ("Find the animal least qualified to operate a submarine.",[new("🐙","Octopus"),new("🦀","Crab"),new("🐬","Dolphin"),new("🔥","Campfire")],3,"Campfires make poor underwater crewmates."),
 ("These are plausible camping supplies, except for one.",[new("⛺","Tent"),new("🔦","Flashlight"),new("🎒","Backpack"),new("🏢","Office building")],3,"Packing the entire office is inefficient."),
 ("Which guest shouldn't attend a tea party?",[new("🫖","Teapot"),new("🍪","Biscuits"),new("☕","Teacup"),new("🛞","Truck tire")],3,"Tires absorb tea rather badly."),
 ("Choose the least reasonable way to travel two blocks.",[new("🚶","Walking"),new("🚲","Bike"),new("🛴","Scooter"),new("🚀","Orbital rocket")],3,"Reentry is complicated for a two-block trip."),
 ("Which item is unexpectedly missing its culinary credentials?",[new("🥕","Carrot"),new("🥬","Lettuce"),new("🥔","Potato"),new("🎮","Game controller")],3,"A controller is not a vegetable even if you mash its buttons."),
 ("Which office employee can't possibly file paperwork?",[new("🧑‍💼","Manager"),new("👩‍💻","Programmer"),new("🧑‍🔧","Technician"),new("🌋","Volcano")],3,"The volcano only files lava."),
 ("Which thing might ruin a chess tournament?",[new("♟️","Pawn"),new("♜","Rook"),new("♞","Knight"),new("🦈","Live shark")],3,"Sharks refuse to follow the diagonal rules."),
 ("Which candidate is the least useful umbrella?",[new("☂️","Umbrella"),new("🧥","Rain jacket"),new("🏠","Roof"),new("🍝","Loose spaghetti")],3,"The spaghetti leaks."),
 ("Which creature is least likely to lay an egg?",[new("🐔","Chicken"),new("🐢","Turtle"),new("🐧","Penguin"),new("🛋️","Sofa")],3,"Furniture seldom reproduces."),
 ("Which one doesn't belong in an orchestra?",[new("🎻","Violin"),new("🎺","Trumpet"),new("🪈","Flute"),new("🪠","Plunger")],3,"The plunger has stage fright."),
 ("Which astronaut packed the worst equipment?",[new("🧑‍🚀","Space suit"),new("🪖","Helmet"),new("🛰️","Communicator"),new("🍦","Melting ice cream")],3,"Ice cream is not reliable life support."),
 ("Choose the least suitable thing to put in a backpack.",[new("📓","Notebook"),new("✏️","Pencil"),new("🥪","Lunch"),new("🚂","Locomotive")],3,"The locomotive exceeds most carry-on limits."),
 ("Which one is not an ordinary picnic companion?",[new("🧺","Basket"),new("🍇","Grapes"),new("🧃","Juice"),new("🦖","Tyrannosaurus")],3,"T. rex is terrible at sharing snacks."),
 ("Which tool would be least helpful fixing a bicycle?",[new("🔧","Wrench"),new("🪛","Screwdriver"),new("🛠️","Toolkit"),new("🎂","Birthday cake")],3,"Cake frosting is not chain lubricant.")];
 private int captchaLevel,captchaScore,captchaLives=3;private bool captchaAnswered;private string captchaMessage="Humanity evaluation pending.";
 private int[] captchaDeck=[],captchaChoiceOrder=[];private int captchaCursor=-1;
 private Tile[] captchaOptions=[];
 private int CurrentCaptcha=>captchaDeck[captchaCursor];
 private string CaptchaPrompt=>Challenges[CurrentCaptcha].Prompt;
 protected override void OnInitialized(){PrepareCaptcha();}
 private void PrepareCaptcha(){
  if(captchaCursor+1>=captchaDeck.Length){var previous=captchaCursor>=0?captchaDeck[captchaCursor]:-1;captchaDeck=Enumerable.Range(0,Challenges.Length).ToArray();Random.Shared.Shuffle(captchaDeck);if(captchaDeck.Length>1&&captchaDeck[0]==previous)(captchaDeck[0],captchaDeck[1])=(captchaDeck[1],captchaDeck[0]);captchaCursor=-1;}
  captchaCursor++;captchaChoiceOrder=Enumerable.Range(0,4).ToArray();Random.Shared.Shuffle(captchaChoiceOrder);captchaOptions=captchaChoiceOrder.Select(i=>Challenges[CurrentCaptcha].Choices[i]).ToArray();
 }
 private void AnswerCaptcha(int n){if(captchaAnswered)return;captchaAnswered=true;var q=Challenges[CurrentCaptcha];if(captchaChoiceOrder[n]==q.Odd){captchaScore+=10;captchaMessage="Correct! "+q.Why;}else{captchaLives=Math.Max(0,captchaLives-1);captchaMessage="Suspicious. "+q.Why;}}
 private void NextCaptcha(){if(captchaLives==0){RestartCaptcha();return;}captchaLevel++;captchaAnswered=false;captchaMessage="Another thoroughly necessary test awaits.";PrepareCaptcha();}
 private void RestartCaptcha(){captchaLevel=captchaScore=0;captchaLives=3;captchaAnswered=false;captchaMessage="Humanity evaluation pending.";captchaDeck=[];captchaCursor=-1;PrepareCaptcha();}
 private static readonly string[] rpsMoves=["Rock","Paper","Scissors"];private int rpsRound=1,rpsPlayer,rpsCpu,rpsTitles,rpsOpponentIndex;private string rpsMessage="Choose your opening move.",rpsFlavor="This opponent appears overconfident.";private string? previousMove;private string rpsDifficulty="Competitive";private int rpsCareerWins,rpsCareerLosses,rpsStreak,rpsBestStreak;
 private static readonly string[] opponents=["Sir Folds-a-Lot","The Office Printer","Captain Scissors","Rocky Balboa's Pebble","Grandmaster Paperclip"];
 private string RpsOpponentName=>opponents[Math.Min(rpsOpponentIndex,opponents.Length-1)];private static string MoveIcon(string m)=>m switch{"Rock"=>"✊","Paper"=>"✋",_=>"✌️"};
 private void PlayRps(string move){if(rpsRound>5)return;string cpu;if(previousMove!=null&&Random.Shared.Next(100)<(rpsDifficulty switch {"Competitive"=>55,"Unfairly Difficult"=>80,_=>25}))cpu=previousMove switch{"Rock"=>"Paper","Paper"=>"Scissors",_=>"Rock"};else cpu=rpsMoves[RandomNumberGenerator.GetInt32(3)];previousMove=move;int outcome=move==cpu?0:(move=="Rock"&&cpu=="Scissors"||move=="Paper"&&cpu=="Rock"||move=="Scissors"&&cpu=="Paper")?1:-1;
 if(outcome==1)rpsPlayer++;else if(outcome<0)rpsCpu++;rpsMessage=$"{MoveIcon(move)} {move} versus {MoveIcon(cpu)} {cpu}: "+(outcome==0?"Draw!":outcome>0?"You win the round!":"The opponent prevails!");rpsRound++;if(rpsRound>5){if(rpsPlayer>rpsCpu){rpsTitles++;rpsCareerWins++;rpsStreak++;rpsBestStreak=Math.Max(rpsBestStreak,rpsStreak);rpsOpponentIndex=(rpsOpponentIndex+1)%opponents.Length;rpsMessage+=" CHAMPION! The next challenger approaches.";}else {rpsCareerLosses++;rpsStreak=0;rpsMessage+=" Tournament finished. A rematch is available.";}}}
 private void NewRpsTournament(){rpsRound=1;rpsPlayer=rpsCpu=0;previousMove=null;rpsMessage="New championship underway.";}
 private async Task RestartPlatformer(){await JS.InvokeVoidAsync("uselessExtras.platform.start","one-button-course");}

}
