using System.Collections.Generic;

public class TutorialTrigger : Service<TutorialTrigger>
{
    private readonly HashSet<string> _triggers = new HashSet<string>();

    public void Set(string trigger)
    {
        _triggers.Add(trigger);
    }

    public bool ContainAndRemove(string trigger)
    {
        if (_triggers.Contains(trigger))
        {
            _triggers.Remove(trigger);
            return true;
        }

        return false;
    }

    public void Clear()
    {
        _triggers.Clear();
    }
}