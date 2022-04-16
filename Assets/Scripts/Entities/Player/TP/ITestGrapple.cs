using System;
public interface ITestGrapple
{
    public void StartPull(Action onProximity = null);
}