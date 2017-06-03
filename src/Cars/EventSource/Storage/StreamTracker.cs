// The MIT License (MIT)
// 
// Copyright (c) 2016 Nelson Corrêa V. Júnior
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cars.EventSource.Storage
{
    public class StreamTracker
    {
        private readonly ConcurrentDictionary<Type, Dictionary<Guid, object>> _track = new ConcurrentDictionary<Type, Dictionary<Guid, object>>();

        public TStream GetById<TStream>(Guid id) where TStream : Stream
        {
            Dictionary<Guid, object> streams;
            if (!_track.TryGetValue(typeof(TStream), out streams))
                return null;

            object stream;
            if (!streams.TryGetValue(id, out stream))
                return null;

            return (TStream)stream;
        }
        
        public void Add<TStream>(TStream streamRoot) where TStream : Stream
        {
            Dictionary<Guid, object> streams;
            if (!_track.TryGetValue(typeof(TStream), out streams))
            {
                streams = new Dictionary<Guid, object>();
                _track.TryAdd(typeof(TStream), streams);
            }

            if (streams.ContainsKey(streamRoot.Id))
                return;

            streams.Add(streamRoot.Id, streamRoot);
        }

        public void Remove(Type streamType, Guid streamId)
        {
            Dictionary<Guid, object> streams;
            if (!_track.TryGetValue(streamType, out streams))
                return;

            streams.Remove(streamId);
        }
        
    }
}