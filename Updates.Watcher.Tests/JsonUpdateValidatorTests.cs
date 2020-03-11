using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Updates.Watcher.Tests
{
    public class JsonUpdateValidatorTests
    {
        private const string FileName = "savedUpdates.json";
        private static readonly JsonUpdateValidator Validator = Create();
        
        [Fact]
        public void CheckIfUpdatesAreSaved()
        {
            Validator.UpdateSent(1, 1);
            
            JsonUpdateValidator newValidator = Create();
            Assert.True(newValidator.WasUpdateSent(1, 1));
        }

        [Fact]
        public void CheckConcurrencyWithDifferentUpdates()
        {
            Task.Run(() => Validator.UpdateSent(1, 1));
            Task.Run(() => Validator.UpdateSent(2, 2));
            
            Thread.Sleep(100);

            Assert.True(Validator.WasUpdateSent(1, 1));
            Assert.True(Validator.WasUpdateSent(2, 2));
        }
        
        [Fact]
        public void CheckConcurrencyWithIdenticalUpdate()
        {
            Task.Run(() => Validator.UpdateSent(1, 1));
            Task.Run(() => Validator.UpdateSent(1, 2));
            
            Thread.Sleep(100);

            Assert.True(Validator.WasUpdateSent(1, 1));
            Assert.True(Validator.WasUpdateSent(1, 2));
        }
        
        private static JsonUpdateValidator Create() => new JsonUpdateValidator(FileName);
    }
}
