using CookiesAuthorization.Services;
using NUnit.Framework;
using NSubstitute;

namespace CookiesNUnit
{
    public class HashingTest
    {
        private IHashingService _hashingService;

        [SetUp]
        public void Setup()
        {
            _hashingService = Substitute.For<IHashingService>();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}