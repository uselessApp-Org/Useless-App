using UselessApp.Services;
using Xunit;
public class AbsurdFeatureTests {
 [Fact] public void MileToBananasUsesInches(){Assert.Equal(8448,AbsurdConversions.Convert(1,"miles","bananas"),6);}
 [Fact] public void KgToFreedomUnits(){Assert.InRange(AbsurdConversions.Convert(1,"kilograms","Bald Eagles (Freedom Units)"),0.18,0.19);}
 [Fact] public void WhaleToLions(){Assert.InRange(AbsurdConversions.Convert(1,"Blue Whale Masses","Lions"),715,716);}
 [Fact] public void NoLengthToMass(){Assert.Throws<ArgumentException>(()=>AbsurdConversions.Convert(1,"miles","lions"));}
 [Theory][InlineData("cool charm", "skibidi rizz")][InlineData("rizz", "charm or charisma")]
 public void TranslatorIsBidirectional(string text,string expected){Assert.Equal(expected,PersonalityTranslations.Translate(text,text=="rizz"?"Gen Z → English":"Gen Z"));}
 [Fact] public void HasAtLeastThreeHundredDistinctAbsurdUnits(){Assert.True(AbsurdConversions.Units.Length>=300);Assert.Equal(AbsurdConversions.Units.Length,AbsurdConversions.Units.Select(x=>x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());}
 [Fact] public void EiffelTowerExists(){Assert.Contains(AbsurdConversions.Units,x=>x.Name=="Eiffel Towers" && x.Kind=="length");}
 [Fact] public void EmpireStateExists(){Assert.Contains(AbsurdConversions.Units,x=>x.Name=="Empire State Buildings");}
 [Fact] public void PirateIsLocal(){Assert.Equal("ahoy matey",PersonalityTranslations.Translate("hello friend","Pirate"));}
 [Fact] public void NoHtmlInGeneratedExcuse(){Assert.Contains("Regarding",AbsurdText.Excuse("wash car",1));}
}
