using System.Collections.Generic;
using UnityEngine;

public class YieldInstructionCache
{
    public static readonly WaitForFixedUpdate m_WaitForFixedUpdate = new WaitForFixedUpdate();
    public static readonly WaitForEndOfFrame m_WaitForEndOfFrame = new WaitForEndOfFrame();
    private static readonly Dictionary<float, WaitForSeconds> m_WaitForSecondsdict = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds WaitForSeconds(float waitTime)
    {
        WaitForSeconds wfs;

        if (m_WaitForSecondsdict.TryGetValue(waitTime, out wfs))
            return wfs;
        else
        {
            wfs = new WaitForSeconds(waitTime);
            m_WaitForSecondsdict.Add(waitTime, wfs);
            return wfs;
        }
    }
}
