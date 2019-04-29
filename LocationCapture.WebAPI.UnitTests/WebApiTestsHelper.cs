using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Threading;
using Xunit;

namespace LocationCapture.WebAPI.UnitTests
{
    public static class WebApiTestsHelper
    {
        private static object _syncRoot = new object();

        public static TObject ExtractObjectFromActionResult<TActionResult, TObject>(IActionResult actionResult)
            where TActionResult : ObjectResult
            where TObject : class
        {
            Assert.IsType<TActionResult>(actionResult);
            var concreteResult = (TActionResult)actionResult;
            Assert.IsType<TObject>(concreteResult.Value);
            var extractedObject = (TObject)concreteResult.Value;
            return extractedObject;
        }

        public static TCollection ExtractGenericCollectionFromActionResult<TActionResult, TCollection>(IActionResult actionResult)
            where TActionResult : ObjectResult
            where TCollection : IEnumerable
        {
            Assert.IsType<TActionResult>(actionResult);
            var concreteResult = (TActionResult)actionResult;
            Assert.IsAssignableFrom<TCollection>(concreteResult.Value);
            var extractedCollection = (TCollection)concreteResult.Value;
            return extractedCollection;
        }

        public static void Lock()
        {
            Monitor.Enter(_syncRoot);
        }

        public static void Unlock()
        {
            Monitor.Exit(_syncRoot);
        }
    }
}
