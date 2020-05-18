﻿using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// A class to handle a moving platform in 3D space, moving along a set of nodes
    /// </summary>
    [AddComponentMenu("TopDown Engine/Environment/Moving Platform 3D")]
    public class MovingPlatform3D : MMPathMovement
    {
        public float PushForce = 5f;       
    }
}
