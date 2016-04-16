﻿#region License
/* Procedural planet generator.
 *
 * Copyright (C) 2015-2016 Denis Ovchinnikov
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the copyright holders nor the names of its
 *    contributors may be used to endorse or promote products derived from
 *    this software without specific prior written permission.

 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION)HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

using UnityEngine;

public sealed class DebugDrawSwitcher : MonoBehaviour, IDebugSwitcher
{
    public DebugDraw[] GUIs;

    public int state = 0;

    private void Start()
    {
        ToogleAll(GUIs, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (state == GUIs.Length)
            {
                state = 0;
                ToogleAll(GUIs, false);
                return;
            }

            ToogleAll(GUIs, false);
            state++;
            ToogleAt(GUIs, true, state);
        }
    }

    public void Toogle(DebugDraw GUI, bool state)
    {
        GUI.enabled = state;
    }

    public void ToogleAt(DebugDraw[] GUIs, bool state, int index)
    {
        GUIs[index - 1].enabled = state;
    }

    public void ToogleAll(DebugDraw[] GUIs, bool state)
    {
        for (int i = 0; i < GUIs.Length; i++)
        {
            GUIs[i].enabled = false;
        }
    }
}