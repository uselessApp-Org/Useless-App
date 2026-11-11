namespace UselessApp.Services;

// Holds per-circuit stats collected from pages until MainLayout saves them on navigation.
public class UserStatsService
{
    // Number of ENTER presses since the last save (added to the DB total with $inc).
    public int CalcEnters { get; set; }
    public string LastExpression { get; set; } = string.Empty;
    public string LastResult { get; set; } = string.Empty;
    public bool IsDirty { get; set; }

    public void RecordCalcEnter(string expression, string result)
    {
        CalcEnters++;
        LastExpression = expression;
        LastResult = result;
        IsDirty = true;
    }

    public void Reset()
    {
        CalcEnters = 0;
        LastExpression = "";
        LastResult = "";
        IsDirty = false;
    }
}
