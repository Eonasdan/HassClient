using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public class AssertExtensions
    {
        public static async Task<T> ThrowsAsync<T>(Task code)
            where T : Exception
        {
            Exception caughtException = null;
            try
            {
                await code;
            }
            catch (Exception e)
            {
                caughtException = e;
            }

            Assert.That(caughtException, Is.InstanceOf<T>());

            return caughtException as T;
        }
    }
}
