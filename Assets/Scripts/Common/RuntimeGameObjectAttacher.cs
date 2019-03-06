﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common
{
    public class RuntimeGameObjectAttacher : MonoBehaviour
    {
        public Transform attachTo;

        void Start()
        {
            if (attachTo != null)
            {
                transform.SetParent(attachTo);
            }
        }
    }
}