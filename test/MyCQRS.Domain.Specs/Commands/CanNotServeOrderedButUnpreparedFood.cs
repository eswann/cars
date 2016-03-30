﻿using System;
using System.Collections.Generic;
using MyCQRS.Events;
using MyCQRS.Restaurant.Commands;
using MyCQRS.Restaurant.Commands.Handlers;
using MyCQRS.Restaurant.Domain;
using MyCQRS.Restaurant.Domain.Exceptions;
using MyCQRS.Restaurant.Domain.ValueObjects;
using MyCQRS.Restaurant.Events;
using SampleCQRS.TestFramework;
using Xunit;

namespace MyCQRS.Domain.Specs.Commands
{
    public class CanNotServeOrderedButUnpreparedFood :
        CommandTestFixture<MarkFoodServedCommand, MarkFoodServedCommandHandler, TabAggregate>
    {
        private Guid _tabAggregate;
        private OrderedItem _testFood1;

        protected override IEnumerable<IDomainEvent> Given()
        {
            _tabAggregate = Guid.NewGuid();

            _testFood1 = new OrderedItem
            {
                MenuNumber = 16,
                Description = "Beef Noodles",
                Price = 7.50M,
                IsDrink = false
            };

            yield return PrepareDomainEvent.Set(new TabOpenedEvent(_tabAggregate, 1, "Nelson")).ToVersion(1);
            yield return PrepareDomainEvent.Set(new FoodOrderedEvent(_tabAggregate, _testFood1.Description, _testFood1.MenuNumber, _testFood1.Price, _testFood1.Status.ToString())).ToVersion(2);
        }

        protected override MarkFoodServedCommand When()
        {
            return new MarkFoodServedCommand(_tabAggregate, new List<int> {_testFood1.MenuNumber});
        }

        [Fact]
        public void Then_food_not_prepared_exception_should_be_throws()
        {
            Assert.Equal(typeof (FoodNotPrepared), CaughtException.GetType());
        }
    }
}