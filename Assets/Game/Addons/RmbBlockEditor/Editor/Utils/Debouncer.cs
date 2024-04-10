// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class Debouncer
    {
        private CancellationTokenSource _cancelTokenSource = null;

        public async Task Debounce(Func<Task> method, int milliseconds = 300)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource?.Dispose();

            _cancelTokenSource = new CancellationTokenSource();

            await Task.Delay(milliseconds, _cancelTokenSource.Token);

            await method();
        }
    }
}