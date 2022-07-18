﻿using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hangfire.Redis.Tests
{
    [CleanRedis, Collection("Sequential")]
    public class RedisSubscriptionFacts
    {
        private readonly CancellationTokenSource _cts;
        private readonly RedisStorage _storage;
        private readonly Mock<ISubscriber> _subscriber;

        public RedisSubscriptionFacts()
        {
            _cts = new CancellationTokenSource();

            var options = new RedisStorageOptions() { Db = RedisUtils.GetDb() };
            _storage = new RedisStorage(RedisUtils.GetHostAndPort(), options);

            _subscriber = new Mock<ISubscriber>();
        }
        
        [Fact]
        public void Ctor_ThrowAnException_WhenStorageIsNull()
        {
            Assert.Throws<ArgumentNullException>("storage",
                () => new RedisSubscription(null, _subscriber.Object));
        }

        [Fact]
        public void Ctor_ThrowAnException_WhenSubscriberIsNull()
        {
            Assert.Throws<ArgumentNullException>("subscriber",
                () => new RedisSubscription(_storage, null));
        }
        [Fact]
        public void WaitForJob_WaitForTheTimeout()
        {
            //Arrange
            Stopwatch sw = new Stopwatch();
            var subscription = new RedisSubscription(_storage, RedisUtils.CreateSubscriber());
            var timeout = TimeSpan.FromMilliseconds(100);
            sw.Start();

            //Act
            subscription.WaitForJob(timeout, _cts.Token);

            //Assert
            sw.Stop();
            Assert.InRange(sw.ElapsedMilliseconds, 99, 120);
        }
    }
    
}
