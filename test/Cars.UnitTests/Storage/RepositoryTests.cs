using System;
using Cars.EventSource.Storage;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cars.UnitTests.Storage
{
    public class RepositoryTests
    {
        private const string _categoryName = "Unit";
        private const string _categoryValue = "Repository";

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_LoggerFactory()
        {
            var session = Mock.Of<ISession>();

            Action act = () => new Repository(null, session);

            act.ShouldThrowExactly<ArgumentNullException>();
        }

        [Trait(_categoryName, _categoryValue)]
        [Fact]
        public void Cannot_pass_null_instance_of_aggregate()
        {
            Action act = () => new Repository(new LoggerFactory(), null);

            act.ShouldThrowExactly<ArgumentNullException>();
        }


    }
}