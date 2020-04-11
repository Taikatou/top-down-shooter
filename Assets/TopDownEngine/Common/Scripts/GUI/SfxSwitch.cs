using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/GUI/SfxSwitch")]
    public class SfxSwitch : MonoBehaviour
    {
        public virtual void On()
        {
            SoundManager.Instance.SfxOn();
        }

        public virtual void Off()
        {
            SoundManager.Instance.SfxOff();
        }
    }
}
