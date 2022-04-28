using System;
public interface IGrapple
{
    public void StartPull(Action onProximity = null);
}