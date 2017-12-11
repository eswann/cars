// Will bring this back when concurrency is properly implemented
//using System;
//
//namespace Cars.EventSource.Exceptions
//{
//    public class ExpectedVersionException<TAggregate> : Exception
//        where TAggregate : IAggregate
//    {
//        public TAggregate Aggregate { get; }
//        public int ExpectedVersion { get; }
//
//        public ExpectedVersionException(TAggregate aggregate, int expectedVersion)
//        {
//            Aggregate = aggregate;
//            ExpectedVersion = expectedVersion;
//        }
//    }
//}