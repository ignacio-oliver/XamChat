using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamChat.Core;

namespace XamChat.Tests
{
    public static class Test
    {
        public static void SetUp()
        {
            ServiceContainer.Register<IWebService>(() => new FakeWebService { SleepDuration = 0 });

            ServiceContainer.Register<ISettings>(() => new FakeSettings());
        }
    }
}
